using Dgraph4Net;
using Dgraph4Net.Services;
using Google.Protobuf;
using Markdig;
using Newtonsoft.Json;
using Quests.DomainModels;
using Quests.DomainModels.ViewModels;
using Quests.Repositories.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Quests.Repositories
{
    public class AnswerStore : ACommonStore
    {
        public const string ContentQuery = @"{
            uid
            dgraph.type
            formatted_text
            text
            user
            creation_date
            votes {
                uid
                type
                dgraph.type
                user
                creation_date
            }
        }";

        public AnswerStore(Dgraph4NetClient client) : base(client)
        {
        }

        public async Task<Answer> Get(Uid questionId, Uid answerId)
        {
            await using var txn = Client.NewTransaction(true, true, CancellationToken.None);

            var answers = await txn.QueryWithVars<Answer>("answers",
                $@"query Q($questionId: string, $answerId: string) {{
                    var(func: type(Question)) @filter(uid($questionId)) @cascade {{
                        answers {{
                            aid as uid
                        }}
                    }}
                    answers(func: uid(aid)) {ContentQuery}
                }}", new Dictionary<string, string> {
                { "$questionId", questionId },
                { "$answerId", answerId }
            });

            return answers.FirstOrDefault();
        }

        public async Task<List<Answer>> List(Uid questionId)
        {
            await using var txn = Client.NewTransaction(true, true, CancellationToken.None);

            return await txn.QueryWithVars<Answer>("answers",
                $@"query Q($questionId: string) {{
                    var(func: type(Question)) @filter(uid($questionId)) @cascade {{
                        answers {{
                            aid as uid
                        }}
                    }}
                    answers(func: uid(aid)) {ContentQuery}
                }}", new Dictionary<string, string> {
                { "$questionId", questionId }
            });
        }

        public async Task<Answer> Create(Uid questionId, AnswerViewModel viewModel)
        {
            await using var txn = Client.NewTransaction(cancellationToken: CancellationToken.None);

            var answer = new Answer
            {
                User = viewModel.User,
                FormattedText = viewModel.FormattedText,
                PureText = Markdown.ToPlainText(viewModel.FormattedText)
            };

            var question = new
            {
                uid = questionId,
                answers = new[] { answer }
            };

            var response = await txn.Mutate(new Mutation
            {
                CommitNow = true,
                SetJson = ByteString.CopyFromUtf8(JsonConvert.SerializeObject(question))
            });

            answer.Id = response.Uids[answer.Id.ToString()[2..]];

            return answer;
        }

        public async Task Delete(Uid questionId, Uid answerId)
        {
            await using var txn = Client.NewTransaction(cancellationToken: CancellationToken.None);

            var answer = await Get(questionId, answerId);

            if (answer is null)
                return;

            var mu = new Mutation
            {
                CommitNow = true
            };

            var nquads = new StringBuilder();
            nquads.AppendLine($"<{questionId}> <answers> <{answerId}> .");
            nquads.AppendLine($"<{answerId}> * * .");

            foreach (var vote in answer.Votes)
            {
                nquads.AppendLine($"<{vote.Id}> * * .");
            }

            mu.DelNquads = ByteString.CopyFromUtf8(nquads.ToString());

            await txn.Mutate(mu);
        }
    }
}
