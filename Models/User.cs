using FitBot.Services;

namespace FitBot.Models
{
    public class User
    {
        public string Name { get; set; }

        public string UserId { get; set; }
        public CallMeBotSettings CallMeBotSettings { get; set; }
    }
}