using System.Threading.Tasks;
using FitBot.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace FitBot.ActivityFunctions
{
    public class TerminateInstance
    {
        [FunctionName("TerminateInstance")]
        public static Task Run(
            [DurableClient] IDurableOrchestrationClient client,
            [ActivityTrigger] TerminateDto terminate)
        {
            return client.TerminateAsync(terminate.InstanceId, terminate.Reason);
        }
    }
}