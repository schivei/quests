using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quests.App.Services;
using System;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StoreRegister
    {
        public static void AddStores(this IServiceCollection services)
        {
            services.TryAddSingleton(sp => new HttpClient
            {
                BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["ApiUrl"])
            });

            services.AddTransient<AnswerStore>();
            services.AddTransient<QuestionStore>();
            services.AddTransient<VoteStore>();
        }
    }
}
