using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitBot.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace FitBot.ActivityFunctions
{
    public static class GetUsers
    {
        [FunctionName("GetUsers")]
        public static async Task<List<User>> Run([ActivityTrigger] IDurableActivityContext context)
        {
            string value = Environment.GetEnvironmentVariable("USERS");
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException("The USERS environment variable was not set.");
            }

            List<User> contacts = JsonConvert.DeserializeObject<List<User>>(value);
            return await Task.FromResult(contacts);
        }
    }
}