using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLipsum.Core;
using Quests.DomainModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Quests.Tests
{
    public class TestsFixture : IDisposable
    {
        public readonly TestServer ApiServer;
        public readonly HttpClient ApiClient;
        public readonly TestServer AppServer;
        public readonly HttpClient AppClient;
        public readonly Repositories.QuestionStore RepQuestionStore;
        public readonly Repositories.AnswerStore RepAnswerStore;
        public readonly Repositories.VoteStore RepVoteStore;
        public readonly App.Services.QuestionStore SvcQuestionStore;
        public readonly App.Services.AnswerStore SvcAnswerStore;
        public readonly App.Services.VoteStore SvcVoteStore;
        public LipsumGenerator Lipsum;
        public string[] UserNames;
        public List<TestQuestionViewModel> Questions;
        public List<TestAnswerViewModel> Answers;
        public List<TestVoteViewModel> Votes;

        private void ConfigureServicesApp(IServiceCollection services)
        {
            services.AddSingleton(ApiClient);
        }

        private void Prepare()
        {
            Lipsum = new LipsumGenerator();

            UserNames = ParallelEnumerable.Range(0, 3)
                .Select(_ => string.Join('.', Lipsum.GenerateWords(2)))
                .ToArray();

            Questions =
            ParallelEnumerable.Range(0, 50)
                .Select(_ => new TestQuestionViewModel
                {
                    FormattedText = string.Join("\n\n----------------\n\n", Lipsum.GenerateParagraphs(2)),
                    User = UserNames.RandomGet()
                }).ToList();

            Answers =
            ParallelEnumerable.Range(0, 150)
                .Select(_ => new TestAnswerViewModel
                {
                    FormattedText = string.Join("\n\n----------------\n\n", Lipsum.GenerateParagraphs(2)),
                    User = UserNames.RandomGet()
                }).ToList();

            Votes =
            ParallelEnumerable.Range(0, 300)
                .Select(_ => new TestVoteViewModel
                {
                    Type = new[] { VoteType.Negative, VoteType.Positive }.RandomGet(),
                    User = UserNames.RandomGet()
                }).ToList();
        }

        public TestsFixture()
        {
            ApiServer = new TestServer(new WebHostBuilder().ConfigureAppConfiguration(builder =>
                builder.AddJsonFile("apisettings.json")
                .AddEnvironmentVariables())
                .UseEnvironment("Tests")
                .UseStartup<Api.Startup>());
            ApiClient = ApiServer.CreateClient();

            AppServer = new TestServer(new WebHostBuilder().ConfigureAppConfiguration(builder =>
                builder.AddJsonFile("appsettings.json").AddEnvironmentVariables())
                .UseEnvironment("Tests")
                .ConfigureTestServices(ConfigureServicesApp)
                .UseStartup<App.Startup>());
            AppClient = AppServer.CreateClient();

            RepQuestionStore = ApiServer.Services.GetRequiredService<Repositories.QuestionStore>();
            RepAnswerStore = ApiServer.Services.GetRequiredService<Repositories.AnswerStore>();
            RepVoteStore = ApiServer.Services.GetRequiredService<Repositories.VoteStore>();

            SvcQuestionStore = AppServer.Services.GetRequiredService<App.Services.QuestionStore>();
            SvcAnswerStore = AppServer.Services.GetRequiredService<App.Services.AnswerStore>();
            SvcVoteStore = AppServer.Services.GetRequiredService<App.Services.VoteStore>();

            Prepare();
        }

        public async Task WaitReady()
        {
            while(true)
            {
                try
                {
                    var _ = await RepQuestionStore.List();
                    break;
                }
                catch
                {
                    await Task.Delay(100);
                }
            }
        }

        public void Dispose()
        {
            foreach (var proc in Api.Startup.Processes)
            {
                proc.Kill(true);
            }

            try
            {
                Directory.Delete("AppData", true);
                File.Delete("schema.dgraph");
            }
            catch { }
        }
    }
}
