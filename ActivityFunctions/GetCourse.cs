using System.Threading.Tasks;
using FitBot.Models;
using FitBot.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace FitBot.ActivityFunctions
{
    public class GetCourse
    {
        private readonly IGymApiService _service;

        public GetCourse(IGymApiService service)
        {
            _service = service;
        }

        [FunctionName("GetCourse")]
        public async Task<Course> Run([ActivityTrigger] CourseRequestDto requestDto)
        {
            CourseResponse response = await _service.GetCurrentCourseAsync(requestDto.CourseId, requestDto.UserId);
            Course course = new Course
            {
                Title = response.Title,
                Instructor = response.Instructor,
                Date = response.Date,
                TimeStart = response.TimeStart,
                TimeEnd = response.TimeEnd
            };
            return course;
        }
    }
}