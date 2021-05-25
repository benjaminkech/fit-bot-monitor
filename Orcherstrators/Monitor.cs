using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FitBot.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace FitBot.Orchestrators
{
    public static class Monitor
    {
        [FunctionName("Monitor")]
        public static async Task Run([OrchestrationTrigger] IDurableOrchestrationContext monitorContext, ILogger log)
        {
            MonitorRequest input = monitorContext.GetInput<MonitorRequest>();
            if (!monitorContext.IsReplaying) { log.LogInformation($"Received monitor request. Course id: {input?.Id}. Phone: {input?.Phone}."); }

            VerifyRequest(input);
            List<User> users = await monitorContext.CallActivityAsync<List<User>>("GetUsers", null);
            User user = VerifyUser(input.Phone, users);
            Course course = await monitorContext.CallActivityAsync<Course>("GetCourse", input.Id);
            VerifyCourse(course);
            string dateString, format;
            DateTime endTime = monitorContext.CurrentUtcDateTime.AddHours(6);
            CultureInfo provider = CultureInfo.InvariantCulture;
            dateString = course.Date + " " + course.TimeStart + " " + "+02:00";
            format = "yyyy-MM-dd HH:mm zzz";
            MessageDto msg = new MessageDto() { User = user, Course = course };

            try
            {
                endTime = DateTime.ParseExact(dateString, format, provider).ToUniversalTime();
            }
            catch (FormatException)
            {
                log.LogWarning("{0} is not in the correct format.", dateString);
            }

            // Send initial confirmation
            await monitorContext.CallActivityAsync("SendConfirmationAlert", msg);

            if (!monitorContext.IsReplaying)
            {
                log.LogInformation($"Instantiating monitor for {input.Id}. Expires: {endTime}.");
            }

            while (monitorContext.CurrentUtcDateTime < endTime)
            {
                // Check the course availability
                if (!monitorContext.IsReplaying)
                {
                    log.LogInformation($"Checking current course conditions for {input.Id} at {monitorContext.CurrentUtcDateTime}.");
                }

                bool isPlaceAvailable = await monitorContext.CallActivityAsync<bool>("GetIsPlaceAvailable", input.Id);

                if (isPlaceAvailable)
                {
                    // Place available
                    if (!monitorContext.IsReplaying)
                    {
                        log.LogInformation($"Detected place available for {course.Title}. Notifying {input.Phone}.");
                    }

                    await monitorContext.CallActivityAsync("SendPlaceAvailableAlert", msg);
                    break;
                }
                else
                {
                    // Wait for the next checkpoint
                    int interval = await monitorContext.CallActivityAsync<int>("GetInterval", null);
                    DateTime nextCheckpoint = monitorContext.CurrentUtcDateTime.AddMinutes(interval);
                    if (!monitorContext.IsReplaying) { log.LogInformation($"Next check for {course.Title} at {nextCheckpoint}."); }

                    await monitorContext.CreateTimer(nextCheckpoint, CancellationToken.None);
                }

            }

            log.LogInformation($"Monitor expiring.");
        }

        [Deterministic]
        private static void VerifyRequest(MonitorRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "An input object is required.");
            }

            if (string.IsNullOrEmpty(request.Id))
            {
                throw new ArgumentNullException(nameof(request.Id), "A id input is required.");
            }

            if (string.IsNullOrEmpty(request.Phone))
            {
                throw new ArgumentNullException(nameof(request.Phone), "A phone number input is required.");
            }
        }

        [Deterministic]
        private static User VerifyUser(string phone, List<User> contacts)
        {
            User user = contacts.Find(c => c.CallMeBotSettings.Phone.Equals(phone));
            return user ?? throw new Exception($"No user with phone number {phone} found.");
        }

        [Deterministic]
        private static void VerifyCourse(Course course)
        {
            if (course == null)
            {
                throw new ArgumentNullException(nameof(course), "An course object is required.");
            }

            if (string.IsNullOrEmpty(course.Title))
            {
                throw new ArgumentNullException(nameof(course.Title), "A title is required.");
            }

            if (string.IsNullOrEmpty(course.Date))
            {
                throw new ArgumentNullException(nameof(course.Date), "A date is required.");
            }

            if (string.IsNullOrEmpty(course.TimeStart))
            {
                throw new ArgumentNullException(nameof(course.TimeStart), "A timeStart is required.");
            }

            if (string.IsNullOrEmpty(course.TimeEnd))
            {
                throw new ArgumentNullException(nameof(course.TimeEnd), "A timeEnd is required.");
            }
        }
    }
}