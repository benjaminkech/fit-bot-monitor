using System.Threading.Tasks;
using FitBot.Models;
using FitBot.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace FitBot.ActivityFunctions
{
    public class SendPlaceAvailableAlert
    {
        private readonly ICallMeBotService _service;

        public SendPlaceAvailableAlert(ICallMeBotService service)
        {
            _service = service;
        }

        [FunctionName("SendPlaceAvailableAlert")]
        public async Task Run(
            [ActivityTrigger] MessageDto msg)
        {
            string message = $"Hi {msg.User.Name},\nGood news!\nThe course {msg.Course.Title} has a place available💪🏻\nCheers your FIT BOT🤖\n\nCourse Details:\n{msg.Course}";
            CallMeBotDto dto = new CallMeBotDto
            {
                Text = message,
                Phone = msg.User.CallMeBotSettings.Phone,
                ApiKey = msg.User.CallMeBotSettings.ApiKey,
                Messenger = msg.User.CallMeBotSettings.Messenger
            };

            await _service.SendAlertAsync(dto);
        }
    }
}