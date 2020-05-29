using Quests.DomainModels;
using Quests.DomainModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Quests.App.Services
{
    public class VoteStore : ACommonStore
    {
        public VoteStore(HttpClient client) : base(client)
        {
        }

        public Task<List<Vote>> List(Uid questionId, Uid answerId) =>
            GetAsync<List<Vote>>($"/api/questions/{questionId}/answers/{answerId}/votes");

        public Task<Vote> Create(Uid questionId, Uid answerId, VoteViewModel viewModel) =>
            PostAsync<Vote>($"/api/questions/{questionId}/answers/{answerId}/votes", viewModel);

        public Task Delete(Uid questionId, Uid answerId, Uid voteId) =>
            DeleteAsync($"/api/questions/{questionId}/answers/{answerId}/votes/{voteId}");
    }
}
