using Microsoft.AspNetCore.Http;
using Quests.DomainModels;
using Quests.DomainModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Quests.App.Services
{
    public class QuestionStore : ACommonStore
    {
        public QuestionStore(HttpClient client) : base(client)
        {
        }

        public Task<List<Question>> List() =>
            GetAsync<List<Question>>($"/api/questions");

        public Task<PaginationViewModel<Question>> Paginate(int page, string search, string orderBy) =>
            GetAsync<PaginationViewModel<Question>>(
                PathString.FromUriComponent($"/api/questions/paginate/{page}")
                .Add(QueryString.Create("q", search)
                .Add(QueryString.Create("sort", orderBy))));

        public Task<Question> Get(Uid questionId) =>
            GetAsync<Question>($"/api/questions/{questionId}");

        public Task<Question> Create(QuestionViewModel viewModel) =>
            PostAsync<Question>($"/api/questions", viewModel);

        public Task Delete(string questionId) =>
            DeleteAsync($"/api/questions/{questionId}");
    }
}
