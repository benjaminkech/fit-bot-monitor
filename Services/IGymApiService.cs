using System.Threading.Tasks;
using FitBot.Models;

namespace FitBot.Services
{
    public interface IGymApiService
    {
        Task<CourseCondition> GetCurrentConditionsAsync(string id);
        Task<CourseResponse> GetCurrentCourseAsync(string id);
    }
}