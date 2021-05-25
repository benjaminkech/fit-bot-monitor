using System.Threading.Tasks;

namespace FitBot.Services
{
    public interface ICallMeBotService
    {
        Task SendAlertAsync(CallMeBotDto callMeBot);
    }
}