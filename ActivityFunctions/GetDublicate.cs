using System.Threading;
using System.Threading.Tasks;
using FitBot.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace FitBot.ActivityFunctions
{
    public class GetDublicate
    {
        [FunctionName("GetDublicate")]
        public static async Task<bool> Run(
            [ActivityTrigger] DublicateDto dublicateDto,
            [DurableClient] IDurableOrchestrationClient client)
        {
            OrchestrationStatusQueryCondition queryFilter = new OrchestrationStatusQueryCondition
            {
                RuntimeStatus = new[]
                {
                    OrchestrationRuntimeStatus.Pending,
                    OrchestrationRuntimeStatus.Running,
                },
            };
            OrchestrationStatusQueryResult result = await client.ListInstancesAsync(
                queryFilter,
                CancellationToken.None);

            foreach (DurableOrchestrationStatus instance in result.DurableOrchestrationState)
            {
                MonitorRequest input = GetInput(instance);

                if (instance.InstanceId != dublicateDto.InstanceId && input.Equals(dublicateDto.Request))
                {
                    return true;
                }

            }

            return false;

            // Note: ListInstancesAsync only returns the first page of results.
            // To request additional pages provide the result.ContinuationToken
            // to the OrchestrationStatusQueryCondition's ContinuationToken property.
        }

        private static MonitorRequest GetInput(DurableOrchestrationStatus instance)
        {
            return JsonConvert.DeserializeObject<MonitorRequest>(JsonConvert.SerializeObject(instance.Input));
        }
    }
}