using Microsoft.AspNetCore.Mvc;
using Quests.DomainModels;
using Quests.DomainModels.ViewModels;
using Quests.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quests.Api.Controllers
{
    /// <summary>
    /// Endpoints for questions
    /// </summary>
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        /// <summary>
        /// Get all questions
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        [HttpGet("~/api/questions", Name = "listQuestions")]
        [ProducesResponseType(typeof(List<Question>), 200)]
        public async Task<IActionResult> Get([FromServices] QuestionStore store)
        {
            var questions = await store.List();

            return Ok(questions);
        }

        /// <summary>
        /// Get a question
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        [HttpGet("~/api/questions/{questionId}", Name = "getQuestion")]
        [ProducesResponseType(typeof(Question), 200)]
        public async Task<IActionResult> Get([FromRoute] string questionId, [FromServices] QuestionStore store)
        {
            var question = await store.Get(questionId);

            return Ok(question);
        }

        /// <summary>
        /// Paginate questions results
        /// </summary>
        /// <param name="page">page number</param>
        /// <param name="search">passing q query value <code>~/api/questions/paginate/{page:int}?q=search+some+thing</code></param>
        /// <param name="orderBy">order by clause <code>~/api/questions/paginate/{page:int}?sort=field:asc</code></param>
        /// <param name="store"></param>
        /// <returns>Only questions - without children data</returns>
        [HttpGet("~/api/questions/paginate/{page:int}", Name = "paginateQuestions")]
        [ProducesResponseType(typeof(PaginationViewModel<Question>), 200)]
        public async Task<IActionResult> Get([FromRoute] int page, [FromQuery(Name = "q")] string search,
            [FromQuery(Name = "sort")] string orderBy, [FromServices] QuestionStore store)
        {
            var questions = await store.Paginate(page, search, orderBy);

            return Ok(questions);
        }

        /// <summary>
        /// Create a question
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        [HttpPost("~/api/questions", Name = "createQuestion")]
        [ProducesResponseType(typeof(Question), 200)]
        public async Task<IActionResult> Post([FromBody] QuestionViewModel viewModel,
            [FromServices] QuestionStore store)
        {
            if (ModelState.IsValid)
            {
                var question = await store.Create(viewModel);

                return Ok(question);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Delete a question
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        [HttpDelete("~/api/questions/{questionId}", Name = "deleteQuestion")]
        [ProducesResponseType(202)]
        public async Task<IActionResult> Delete([FromRoute] string questionId,
            [FromServices] QuestionStore store)
        {
            await store.Delete(questionId);

            return Accepted();
        }
    }
}
