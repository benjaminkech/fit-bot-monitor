using System.Threading.Tasks;
using FitBot.Models;

namespace FitBot.Services
{
    public interface IGymApiService
    {
        Task<CourseCondition> GetCurrentConditionsAsync(string courseId, string userId);
        Task<CourseResponse> GetCurrentCourseAsync(string courseId, string userId);
    }
}