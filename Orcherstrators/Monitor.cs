using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FitBot.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FitBot.Orchestrators
{
    public static class Monitor
    {
        [FunctionName("Monitor")]
        public static async Task Run([OrchestrationTrigger] IDurableOrchestrationContext monitorContext, ILogger log)
        {
            RetryOptions retryOptions = new RetryOptions(
                        firstRetryInterval: TimeSpan.FromMinutes(1),
                        maxNumberOfAttempts: 10)
            {
                BackoffCoefficient = 2.0
            };

            MonitorRequest input = monitorContext.GetInput<MonitorRequest>();
            if (!monitorContext.IsReplaying) { log.LogInformation($"Received monitor request. Course id: {input?.CourseId}. User id: {input?.UserId}."); }

            VerifyRequest(input);
            List<User> users = await monitorContext.CallActivityAsync<List<User>>("GetUsers", null);
            User user = VerifyUser(input.UserId, users);

            CourseRequestDto courseRequestDto = new CourseRequestDto
            {
                CourseId = input.CourseId,
                UserId = user.UserId
            };
            Course course = await monitorContext.CallActivityAsync<Course>("GetCourse", courseRequestDto);
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

            // Check if request is a dublicate
            DublicateDto dublicateDto = new DublicateDto { InstanceId = monitorContext.InstanceId, Request = input };
            bool dublicate = await monitorContext.CallActivityAsync<bool>("GetDublicate", dublicateDto);

            if (dublicate)
            {
                string reason = $"The request {JsonConvert.SerializeObject(input)} already exists.";
                TerminateDto terminateDto = new TerminateDto { InstanceId = monitorContext.InstanceId, Reason = reason };
                await monitorContext.CallActivityAsync("TerminateInstance", terminateDto);
            }
            else
            {

                if (monitorContext.CurrentUtcDateTime < endTime)
                {
                    // Send initial confirmation
                    await monitorContext.CallActivityAsync("SendConfirmationAlert", msg);
                }

                if (!monitorContext.IsReplaying)
                {
                    log.LogInformation($"Instantiating monitor for {input.CourseId}. Expires: {endTime}.");
                }

                while (monitorContext.CurrentUtcDateTime < endTime)
                {
                    // Check the course availability
                    if (!monitorContext.IsReplaying)
                    {
                        log.LogInformation($"Checking current course conditions for {input.CourseId} at {monitorContext.CurrentUtcDateTime}.");
                    }

                    bool isPlaceAvailable = await monitorContext.CallActivityWithRetryAsync<bool>("GetIsPlaceAvailable", retryOptions, courseRequestDto);

                    if (isPlaceAvailable)
                    {
                        // Place available
                        if (!monitorContext.IsReplaying)
                        {
                            log.LogInformation($"Detected place available for {course.Title}. Notifying {input.UserId}.");
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
        }

        [Deterministic]
        private static void VerifyRequest(MonitorRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "An input object is required.");
            }

            if (string.IsNullOrEmpty(request.CourseId))
            {
                throw new ArgumentNullException(nameof(request.CourseId), "A Course id input is required.");
            }

            if (string.IsNullOrEmpty(request.UserId))
            {
                throw new ArgumentNullException(nameof(request.UserId), "A User id input is required.");
            }
        }

        [Deterministic]
        private static User VerifyUser(string userId, List<User> contacts)
        {
            User user = contacts.Find(c => c.UserId.Equals(userId));
            return string.IsNullOrEmpty(userId)
                ? throw new ArgumentNullException(nameof(userId), "A UserId is required.")
                : user ?? throw new Exception($"No user with userId {userId} found.");
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