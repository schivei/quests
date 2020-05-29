using Quests.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StoreRegister
    {
        public static void AddStores(this IServiceCollection services)
        {
            services.AddTransient<AnswerStore>();
            services.AddTransient<QuestionStore>();
            services.AddTransient<VoteStore>();
        }
    }
}
