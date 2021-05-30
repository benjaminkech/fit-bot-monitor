using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FitBot.Models;

namespace FitBot.Services
{
    public class GymApiService : IGymApiService
    {
        private readonly HttpClient _client;

        public GymApiService(HttpClient httpClient)
        {
            _client = httpClient;
        }
        public async Task<CourseCondition> GetCurrentConditionsAsync(string id)
        {
            CourseResponse response = await GetCurrentCourseAsync(id);
            return MapToCoursePlaceCondition(response.AvailablePlaces, response.Enrolment);
        }

        public async Task<CourseResponse> GetCurrentCourseAsync(string id)
        {
            string api = Environment.GetEnvironmentVariable("GYM_API");
            if (string.IsNullOrEmpty(api))
            {
                throw new InvalidOperationException("The GYM_API environment variable was not set.");
            }
            string userid = Environment.GetEnvironmentVariable("GYM_USERID");
            if (string.IsNullOrEmpty(api))
            {
                throw new InvalidOperationException("The GYM_USERID environment variable was not set.");
            }
            string callString = $"{api}/{id}/{userid}";
            HttpResponseMessage response = await _client.GetAsync(callString);
            CourseResponse content = await response.Content.ReadAsAsync<CourseResponse>();

            content = TrimContent(content);

            return !response.IsSuccessStatusCode
                ? throw new InvalidOperationException($"API returned an error: {response.ReasonPhrase}.")
                : content.Description == string.Empty ? throw new ArgumentException("Could not find a course with this id") : content;
        }

        private static CourseCondition MapToCoursePlaceCondition(int availablePlaces, bool enrolment)
        {
            return availablePlaces > 0 && enrolment ? CourseCondition.Available : CourseCondition.NotAvailable;
        }

        private static CourseResponse TrimContent(CourseResponse content)
        {
            IEnumerable<System.Reflection.PropertyInfo> stringProperties = content.GetType().GetProperties()
              .Where(p => p.PropertyType == typeof(string));

            foreach (System.Reflection.PropertyInfo stringProperty in stringProperties)
            {
                string currentValue = (string)stringProperty.GetValue(content, null);
                stringProperty.SetValue(content, currentValue.TrimStart(), null);
            }

            return content;
        }
    }
    public class CourseResponse : Course
    {
        public string Description { get; set; }
        public string Media { get; set; }
        public string ICal { get; set; }
        public string ExternalLink { get; set; }
        public string Center { get; set; }
        public string Id { get; set; }
        public int Daytime { get; set; }
        public int Weekday { get; set; }
        public List<object> Types { get; set; }
        public List<object> Topics { get; set; }
        public string Location { get; set; }
        public bool Enrolment { get; set; }
        public bool Booked { get; set; }
        public int AvailablePlaces { get; set; }
        public int MaxPlaces { get; set; }
        public int CenterId { get; set; }
        public string EnrolmentId { get; set; }
        public int TitleId { get; set; }
        public bool Marked { get; set; }
        public string ReservationDescription { get; set; }
    }
}