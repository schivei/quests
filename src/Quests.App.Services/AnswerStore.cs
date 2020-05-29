using Quests.DomainModels;
using Quests.DomainModels.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Quests.App.Services
{
    public class AnswerStore : ACommonStore
    {
        public AnswerStore(HttpClient client) : base(client)
        {
        }

        public Task<List<Answer>> List(string questionId) =>
            GetAsync<List<Answer>>($"/api/questions/{questionId}/answers");

        public Task<Answer> Create(string questionId, AnswerViewModel viewModel) =>
            PostAsync<Answer>($"/api/questions/{questionId}/answers", viewModel);

        public Task Delete(string questionId, string answerId) =>
            DeleteAsync($"/api/questions/{questionId}/answers/{answerId}");
    }
}
