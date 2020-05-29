using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Quests.App.Services;
using Quests.DomainModels;
using Quests.DomainModels.ViewModels;

namespace Quests.App.Pages
{
    public class QuestionModel : PageModel
    {
        [BindUid(SupportsGet = true, Name = "questionId")]
        public Uid QuestionId { get; set; }

        protected QuestionStore QuestionStore { get; }

        protected AnswerStore AnswerStore { get; }

        protected VoteStore VoteStore { get; }

        public QuestionModel(QuestionStore questionStore, AnswerStore answerStore, VoteStore voteStore)
        {
            QuestionStore = questionStore;
            AnswerStore = answerStore;
            VoteStore = voteStore;
        }

        public Question Question { get; set; }

        public Task OnGetAsync()
        {
            Answer = string.Empty;

            return GetQuestion();
        }

        private async Task GetQuestion()
        {
            Question = await QuestionStore.Get(QuestionId);
            Question.FormattedText = Markdown.ToHtml(Question.FormattedText);
        }

        [BindProperty]
        [JsonProperty("formatted_text")]
        [MaxLength(2048)]
        [Required(AllowEmptyStrings = false)]
        public string Answer { get; set; }

        public async Task OnPostAnswerAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await AnswerStore.Create(QuestionId, new AnswerViewModel
                    {
                        User = User.Identity.Name,
                        FormattedText = Answer
                    });

                    Answer = string.Empty;
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("Answer", e.Message);
                }
            }

            await Task.Delay(1000);

            await GetQuestion();
        }

        [BindUid(Name = "answerId", SupportsGet = true)]
        public Uid AnswerId { get; set; }

        [BindUid(Name = "voteId", SupportsGet = true)]
        public Uid? VoteId { get; set; }

        [BindProperty(Name = "voteType", SupportsGet = true)]
        public VoteType VoteType { get; set; }

        public async Task OnPostDeleteAnswerAsync()
        {
            await AnswerStore.Delete(QuestionId, AnswerId);
            
            await Task.Delay(1000);

            ModelState.Clear();

            await GetQuestion();
        }

        public async Task OnPostToggleVoteAsync()
        {
            if (!VoteId.HasValue)
            {
                await VoteStore.Create(QuestionId, AnswerId, new VoteViewModel
                {
                    Type = VoteType,
                    User = User.Identity.Name
                });
            }
            else
            {
                await VoteStore.Delete(QuestionId, AnswerId, VoteId.Value);
            }

            await Task.Delay(1000);

            ModelState.Clear();

            await GetQuestion();
        }
    }
}