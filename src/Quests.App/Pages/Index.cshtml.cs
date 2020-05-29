using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quests.App.Services;
using Quests.DomainModels;
using Quests.DomainModels.ViewModels;

namespace Quests.App.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        protected QuestionStore Client { get; }

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, QuestionStore client)
        {
            _logger = logger;
            Client = client;
        }

        [BindProperty]
        [Range(1, int.MaxValue)]
        public int PageIndex { get; set; }

        [BindProperty]
        public string Search { get; set; }

        [BindProperty]
        [RegularExpression("^([a-z][a-z0-9_]+:(asc|desc))$")]
        public string OrderBy { get; set; }

        public PaginationViewModel<Question> Questions { get; set; }

        [BindProperty]
        [JsonProperty("formatted_text")]
        [MaxLength(2048)]
        [Required(AllowEmptyStrings = false)]
        public string Question { get; set; }

        public IEnumerable<SelectListItem> OrderItems { get; set; }

        public bool Initial { get; set; }

        public async Task OnPostCreateQuestion()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await Client.Create(new QuestionViewModel
                    {
                        User = User.Identity.Name,
                        FormattedText = Question
                    });

                    Question = string.Empty;
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("Question", e.Message);
                }
            }

            await Task.Delay(1000);

            await SearchQuestions();
        }

        private async Task SearchQuestions()
        {
            var orderItems = new List<SelectListItem> {
                new SelectListItem("Data Decrescente", "creation_date:desc"),
                new SelectListItem("Data Crescente", "creation_date:asc"),
                new SelectListItem("Qtd. Respostas Decrescente", "answers:desc"),
                new SelectListItem("Qtd. Respostas Crescente", "answers:asc"),
            };

            orderItems.ForEach(oi => oi.Selected = oi.Value == OrderBy);

            OrderItems = orderItems;

            var questions = await Client.Paginate(PageIndex, Search, OrderBy);
            questions.Items = questions.Items.Select(q =>
            {
                q.FormattedText = Markdown.ToHtml(q.FormattedText);
                return q;
            }).ToList();

            Questions = questions;
        }

        public async Task OnGetAsync()
        {
            try
            {
                OrderBy = "creation_date:desc";
                PageIndex = 1;

                Question = string.Empty;

                Initial = true;

                await SearchQuestions();
            }
            catch (Exception e)
            {
                Questions = new PaginationViewModel<Question>
                {
                    Items = new List<Question>()
                };
                ModelState.AddModelError("Question", e.Message);
            }
        }

        public async Task OnPostNextPageAsync()
        {
            PageIndex++;
            await OnPostSearchAsync();
        }

        public async Task OnPostPreviousPageAsync()
        {
            PageIndex--;
            await OnPostSearchAsync();
        }

        public async Task OnPostSearchAsync()
        {
            Initial = false;

            await SearchQuestions();

            ModelState.Clear();
        }

        [BindUid(SupportsGet = true, Name = "questionId")]
        public Uid QuestionId { get; set; }

        public async Task OnPostDeleteQuestionAsync()
        {
            await Client.Delete(QuestionId);
            
            await Task.Delay(1000);

            ModelState.Clear();

            await SearchQuestions();
        }
    }
}
