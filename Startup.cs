using FitBot.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(FitBot.Startup))]

namespace FitBot
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            _ = builder.Services.AddHttpClient();

            _ = builder.Services.AddSingleton<IGymApiService, GymApiService>();
            _ = builder.Services.AddSingleton<ICallMeBotService, CallMeBotService>();
        }
    }
}