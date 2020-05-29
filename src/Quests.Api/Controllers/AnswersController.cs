using Microsoft.AspNetCore.Mvc;
using Quests.DomainModels;
using Quests.DomainModels.ViewModels;
using Quests.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quests.Api.Controllers
{
    /// <summary>
    /// Endpoints for answers
    /// </summary>
    [ApiController]
    public class AnswersController : ControllerBase
    {
        /// <summary>
        /// Get answers of a question
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        [HttpGet("~/api/questions/{questionId}/answers", Name = "listAnswers")]
        [ProducesResponseType(typeof(List<Answer>), 200)]
        public async Task<IActionResult> Get([FromRoute] string questionId,
            [FromServices] AnswerStore store)
        {
            var answers = await store.List(questionId);

            return Ok(answers);
        }

        /// <summary>
        /// Post answer to question
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="viewModel"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        [HttpPost("~/api/questions/{questionId}/answers", Name = "createAnswer")]
        [ProducesResponseType(typeof(Answer), 200)]
        public async Task<IActionResult> Post([FromRoute] string questionId,
            [FromBody] AnswerViewModel viewModel,
            [FromServices] AnswerStore store)
        {
            if (ModelState.IsValid)
            {
                var answer = await store.Create(questionId, viewModel);

                return Ok(answer);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Delete a question answer
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="answerId"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        [HttpDelete("~/api/questions/{questionId}/answers/{answerId}", Name = "deleteAnswer")]
        [ProducesResponseType(202)]
        public async Task<IActionResult> Delete([FromRoute] string questionId, [FromRoute] string answerId,
            [FromServices] AnswerStore store)
        {
            await store.Delete(questionId, answerId);

            return Accepted();
        }
    }
}
