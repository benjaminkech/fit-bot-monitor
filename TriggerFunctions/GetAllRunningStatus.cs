using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace FitBot.TriggerFunctions
{
    public static class GetAllRunningStatus
    {
        [FunctionName("GetAllRunningStatus")]
        public static async Task<OrchestrationStatusQueryResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient client)
        {
            OrchestrationStatusQueryCondition queryFilter = new OrchestrationStatusQueryCondition
            {
                RuntimeStatus = new[]
                {
                    OrchestrationRuntimeStatus.Running,
                },
            };
            OrchestrationStatusQueryResult result = await client.ListInstancesAsync(
                queryFilter,
                CancellationToken.None);

            return result;
            // Note: ListInstancesAsync only returns the first page of results.
            // To request additional pages provide the result.ContinuationToken
            // to the OrchestrationStatusQueryCondition's ContinuationToken property.
        }
    }
}