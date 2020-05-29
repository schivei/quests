using Dgraph4Net;
using Dgraph4Net.Services;
using Google.Protobuf;
using Newtonsoft.Json;
using Quests.DomainModels;
using Quests.DomainModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quests.Repositories
{
    public class VoteStore : ACommonStore
    {
        public VoteStore(Dgraph4NetClient client) : base(client)
        {
        }

        public const string ContentQuery = @"{
            uid
            dgraph.type
            type
            creation_date
            user
        }";

        public async Task<List<Vote>> List(Uid questionId, Uid answerId)
        {
            await using var txn = Client.NewTransaction(true, true, CancellationToken.None);

            return await txn.QueryWithVars<Vote>("votes",
                $@"query Q($questionId: string, $answerId: string) {{
                    var(func: type(Question)) @filter(uid($questionId)) @cascade {{
                        answers @filter(uid($answerId)) {{
                            votes {{
                                vid as uid
                            }}
                        }}
                    }}
                    votes(func: uid(vid)) {ContentQuery}
                }}", new Dictionary<string, string> {
                { "$questionId", questionId },
                { "$answerId", answerId }
            });
        }

        public async Task<Vote> Create(Uid questionId, Uid answerId, VoteViewModel viewModel)
        {
            var votes = await List(questionId, answerId);

            foreach (var v in votes.Where(vt => vt.User == viewModel.User))
            {
                await Delete(answerId, v.Id);
            }

            await using var txn = Client.NewTransaction(cancellationToken: CancellationToken.None);

            var vote = new Vote
            {
                User = viewModel.User,
                VoteType = viewModel.Type
            };

            var question = new
            {
                uid = questionId,
                answers = new[] {
                    new {
                        uid = answerId,
                        votes = new [] { vote }
                    }
                }
            };

            var response = await txn.Mutate(new Mutation
            {
                CommitNow = true,
                SetJson = ByteString.CopyFromUtf8(JsonConvert.SerializeObject(question))
            });

            vote.Id = response.Uids[vote.Id.ToString()[2..]];

            return vote;
        }

        public async Task Delete(Uid answerId, Uid voteId)
        {
            await using var txn = Client.NewTransaction(cancellationToken: CancellationToken.None);

            var mu = new Mutation
            {
                CommitNow = true
            };

            var nquads = new StringBuilder();
            nquads.AppendLine($"<{answerId}> <votes> <{voteId}> .");
            nquads.AppendLine($"<{voteId}> * * .");
            
            mu.DelNquads = ByteString.CopyFromUtf8(nquads.ToString());

            await txn.Mutate(mu);
        }
    }
}
