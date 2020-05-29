using Quests.DomainModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Priority;

namespace Quests.Tests
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class ApplicationTests : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture _fixture;

        public ApplicationTests(TestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Check for dgraph ready in 60 seconds"), Priority(-100)]
        public async Task Waiter()
        {
            await _fixture.WaitReady().TimedOutAsync(60000);
            Assert.True(true);
        }

        [Fact(DisplayName = "Create Questions"), Priority(0)]
        public async Task Unit_CreateQuestions_Test()
        {
            await Task.Delay(1000);
            foreach (var question in _fixture.Questions)
            {
                var q = await _fixture.RepQuestionStore.Create(question);
                Assert.NotNull(q);
                question.Id = q.Id;
            }
        }

        [Fact(DisplayName = "Create Answers"), Priority(1)]
        public async Task Unit_CreateAnswers_Test()
        {
            await Task.Delay(1000);
            foreach (var answer in _fixture.Answers)
            {
                var question = _fixture.Questions.RandomGet();
                var q = await _fixture.RepAnswerStore.Create(question.Id, answer);
                Assert.NotNull(q);
                answer.Id = q.Id;
                answer.QuestionId = question.Id;
            }
        }

        [Fact(DisplayName = "Create Votes"), Priority(2)]
        public async Task Unit_CreateVotes_Test()
        {
            await Task.Delay(1000);
            foreach (var vote in _fixture.Votes)
            {
                var answer = _fixture.Answers.RandomGet();
                var q = await _fixture.RepVoteStore.Create(answer.QuestionId, answer.Id, vote);
                Assert.NotNull(q);
                vote.Id = q.Id;
                vote.AnswerId = answer.Id;
                vote.QuestionId = answer.QuestionId;
            }
        }

        [Fact(DisplayName = "List All Questions")]
        [Priority(3)]
        public async Task Unit_ListQuestions_Test()
        {
            await Task.Delay(1000);
            var questionIds = _fixture.Questions.Select(q => q.Id.ToString()).ToArray();

            var questions = await _fixture.RepQuestionStore.List();
            var qids = questions.Select(q => q.Id).ToArray();

            Assert.All(qids, id => questionIds.Contains(id.ToString()));
        }

        [Fact(DisplayName = "List All Answers")]
        [Priority(4)]
        public async Task Unit_ListAnswers_Test()
        {
            await Task.Delay(1000);
            var answerIds = _fixture.Answers.Select(q => q.Id.ToString()).ToArray();

            var cAnswers = new List<Answer>();

            foreach (var question in _fixture.Questions)
            {
                var answers = await _fixture.RepAnswerStore.List(question.Id);
                cAnswers.AddRange(answers.ToArray());
            }

            var aids = cAnswers.Select(q => q.Id).ToArray();

            Assert.All(aids, id => answerIds.Contains(id.ToString()));
        }

        [Fact(DisplayName = "List All Votes")]
        [Priority(5)]
        public async Task Unit_ListVotes_Test()
        {
            await Task.Delay(1000);
            var answerIds = _fixture.Answers.Select(q => q.Id.ToString()).ToArray();

            var cAnswers = new List<Answer>();

            foreach (var question in _fixture.Questions)
            {
                var answers = await _fixture.RepAnswerStore.List(question.Id);
                cAnswers.AddRange(answers.ToArray());
            }

            var aids = cAnswers.Select(q => q.Id.ToString()).ToArray();

            Assert.All(aids, id => answerIds.Contains(id));
        }

        [Fact(DisplayName = "Delete All Votes")]
        [Priority(6)]
        public async Task Unit_DeleteVotes_Test()
        {
            await Task.Delay(1000);
            var tasks = new List<Task>();
            foreach (var vote in _fixture.Votes)
                tasks.Add(_fixture.RepVoteStore.Delete(vote.AnswerId, vote.Id));

            await Task.WhenAll(tasks);

            await Task.Delay(2000);

            foreach (var answer in _fixture.Answers)
            {
                var votes = await _fixture.RepVoteStore.List(answer.QuestionId, answer.Id);
                Assert.Empty(votes);
            }
        }

        [Fact(DisplayName = "Delete All Answers")]
        [Priority(7)]
        public async Task Unit_DeleteAnswers_Test()
        {
            await Task.Delay(1000);
            var tasks = new List<Task>();
            foreach (var answer in _fixture.Answers)
                tasks.Add(_fixture.RepAnswerStore.Delete(answer.QuestionId, answer.Id));

            await Task.WhenAll(tasks);

            await Task.Delay(2000);

            foreach (var question in _fixture.Questions)
            {
                var answers = await _fixture.RepAnswerStore.List(question.Id);
                Assert.Empty(answers);
            }
        }

        [Fact(DisplayName = "Delete All Questions")]
        [Priority(8)]
        public async Task Unit_DeleteQuestions_Test()
        {
            await Task.Delay(1000);
            var tasks = new List<Task>();
            foreach (var question in _fixture.Questions)
                tasks.Add(_fixture.RepQuestionStore.Delete(question.Id));

            await Task.WhenAll(tasks);

            await Task.Delay(2000);

            var questions = await _fixture.RepQuestionStore.List();
            Assert.Empty(questions);
        }
    }
}
