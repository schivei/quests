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
    public class QuestionStore : ACommonStore
    {
        public QuestionStore(Dgraph4NetClient client) : base(client)
        {
        }

        public const string ContentQuery = @"{
            uid
            dgraph.type
            formatted_text
            text
            creation_date
            user
            answers {
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
            }
        }";

        public async Task<List<Question>> List()
        {
            await using var txn = Client.NewTransaction(true, true, CancellationToken.None);

            return await txn.Query<Question>("questions",
            $@"{{
                questions(func: type(Question)) {ContentQuery}
            }}");
        }

        public async Task<Question> Get(Uid questionId)
        {
            await using var txn = Client.NewTransaction(true, true, CancellationToken.None);

            var questions = await txn.QueryWithVars<Question>("questions",
            $@"query Q($questionId: string){{
                questions(func: type(Question)) @filter(uid($questionId)) {ContentQuery}
            }}", new Dictionary<string, string> {
                { "$questionId", questionId }
            });

            return questions.FirstOrDefault();
        }

        public Task<PaginationViewModel<Question>> Paginate(int page, string search, string orderBy)
        {
            const string simpleContent = @"{
                uid
                dgraph.type
                formatted_text
                text
                creation_date
                user
                answers {
                    uid
                    votes {
                        type
                    }
                }
            }";

            var isDesc = orderBy?.Split(':')[1] != "asc";
            orderBy = orderBy?.Split(':')[0] ?? "creation_date";

            var byCount = "";

            if (orderBy == "answers")
            {
                orderBy = "val(num_of_answers)";
                byCount = @"{
                    num_of_answers as count(answers)
                }";
            }

            var order = $"order{(!isDesc ? "asc" : "desc")}";

            var pageSize = page - 1;

            long offset = 10 * pageSize;

            var searchColumns = new Question().GetSearchColumns();

            if (searchColumns.Length > 0 && !string.IsNullOrEmpty(search?.Trim()))
            {
                var searchFor = string.Join(" OR ", searchColumns.SelectMany(sc => new[] { $"anyoftext({sc}, $search)", $"alloftext({sc}, $search)", $"regexp({sc}, /{Regex.Escape(search)}/i)" }));

                var searchVars = new Dictionary<string, string> { { "$search", search } };

                return Client.NewTransaction(true, true, CancellationToken.None)
                    .PaginateWithVars<Question>("items", $@"query Q($search: string){{
                    c as var(func: type(Question)) @filter({searchFor}){byCount}
                    items(func: uid(c), {order}: {orderBy}, offset: {offset}, first: {pageSize}) {simpleContent}
                    it(func: uid(c)) {{
                        count: count(uid)
                    }}
                }}", searchVars);
            }
            else
            {
                return Client.NewTransaction(true, true)
                    .Paginate<Question>("items", $@"{{
                    c as var(func: type(Question)){byCount}
                    items(func: uid(c), {order}: {orderBy}, offset: {offset}, first: {pageSize}) {simpleContent}
                    it(func: uid(c)) {{
                        count: count(uid)
                    }}
                }}");
            }
        }

        public async Task<Question> Create(QuestionViewModel viewModel)
        {
            await using var txn = Client.NewTransaction(cancellationToken: CancellationToken.None);

            var question = new Question
            {
                User = viewModel.User,
                FormattedText = viewModel.FormattedText,
                PureText = Markdown.ToPlainText(viewModel.FormattedText)
            };

            var response = await txn.Mutate(new Mutation
            {
                CommitNow = true,
                SetJson = ByteString.CopyFromUtf8(JsonConvert.SerializeObject(question))
            });

            question.Id = response.Uids[question.Id.ToString()[2..]];

            return question;
        }

        public async Task Delete(Uid questionId)
        {
            await using var txn = Client.NewTransaction(cancellationToken: CancellationToken.None);

            var question = await Get(questionId);

            if (question is null)
                return;

            var mu = new Mutation
            {
                CommitNow = true
            };

            var nquads = new StringBuilder();
            nquads.AppendLine($"<{questionId}> * * .");

            foreach (var answer in question.Answers)
            {
                nquads.AppendLine($"<{answer.Id}> * * .");

                foreach (var vote in answer.Votes)
                {
                    nquads.AppendLine($"<{vote.Id}> * * .");
                }
            }

            mu.DelNquads = ByteString.CopyFromUtf8(nquads.ToString());

            await txn.Mutate(mu);
        }
    }
}
