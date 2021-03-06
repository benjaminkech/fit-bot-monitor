using System.Threading.Tasks;
using FitBot.Models;
using FitBot.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace FitBot.ActivityFunctions
{
    public class GetIsPlaceAvailable
    {
        private readonly IGymApiService _service;

        public GetIsPlaceAvailable(IGymApiService service)
        {
            _service = service;
        }
        [FunctionName("GetIsPlaceAvailable")]
        public async Task<bool> Run([ActivityTrigger] CourseRequestDto requestDto)
        {
            CourseCondition currentConditions = await _service.GetCurrentConditionsAsync(requestDto.CourseId, requestDto.UserId);
            return currentConditions.Equals(CourseCondition.Available);
        }
    }
}