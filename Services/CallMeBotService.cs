using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace FitBot.Services
{
    public class CallMeBotService : ICallMeBotService
    {
        private readonly HttpClient _client;

        public CallMeBotService(HttpClient httpClient)
        {
            _client = httpClient;
        }
        public async Task SendAlertAsync(CallMeBotDto callMeBot)
        {
            const string baseUrl = "https://api.callmebot.com";
            string path = GetPath(callMeBot.Messenger);
            string api = baseUrl + "/" + path;
            string msg = HttpUtility.UrlEncode(callMeBot.Text);

            string url = string.Format($"{api}?phone={callMeBot.Phone}&apikey={callMeBot.ApiKey}&text={msg}");
            HttpResponseMessage response = await _client.GetAsync(url);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ArgumentException($"Failed to send alert: {response.StatusCode}: {response.Content}");
            }
        }

        private static string GetPath(Messenger messenger)
        {
            return messenger switch
            {
                Messenger.Whatsapp => "whatsapp.php",
                Messenger.Signal => "signal/send.php",
                Messenger.Telegram => "text.php",
                _ => throw new Exception("Messenger not found"),
            };
        }
    }
    public class CallMeBotDto : CallMeBotSettings
    {
        public string Text { get; set; }
    }

    public class CallMeBotSettings
    {
        public string Phone { get; set; }
        public string ApiKey { get; set; }
        public Messenger Messenger { get; set; }
    }

    public enum Messenger
    {
        Whatsapp,
        Signal,
        Telegram
    }
}