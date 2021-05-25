using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace FitBot.ActivityFunctions
{
    public static class GetInterval
    {
        [FunctionName("GetInterval")]
        public static async Task<int> Run([ActivityTrigger] IDurableActivityContext context)
        {
            string interval = Environment.GetEnvironmentVariable("INTERVAL");
            return string.IsNullOrEmpty(interval)
                ? throw new InvalidOperationException("The INTERVAL environment variable was not set.")
                : await Task.FromResult(Int32.Parse(interval));
        }
    }
}