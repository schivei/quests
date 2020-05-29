using Microsoft.AspNetCore.Mvc;
using Quests.DomainModels;
using Quests.DomainModels.ViewModels;
using Quests.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quests.Api.Controllers
{
    /// <summary>
    /// Endpoints for votes
    /// </summary>
    [ApiController]
    public class VotesController : ControllerBase
    {
        /// <summary>
        /// List votes of a question answer
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="answerId"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        [HttpGet("~/api/questions/{questionId}/answers/{answerId}/votes", Name = "listVotes")]
        [ProducesResponseType(typeof(List<Vote>), 200)]
        public async Task<IActionResult> Get([FromRoute] string questionId, [FromRoute] string answerId,
            [FromServices] VoteStore store)
        {
            var votes = await store.List(questionId, answerId);

            return Ok(votes);
        }

        /// <summary>
        /// Vote in a question answer
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="answerId"></param>
        /// <param name="viewModel"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        [HttpPost("~/api/questions/{questionId}/answers/{answerId}/votes", Name = "voteAnswer")]
        [ProducesResponseType(typeof(Vote), 200)]
        public async Task<IActionResult> Post([FromRoute] string questionId, [FromRoute] string answerId,
            [FromBody] VoteViewModel viewModel,
            [FromServices] VoteStore store)
        {
            if (ModelState.IsValid)
            {
                var vote = await store.Create(questionId, answerId, viewModel);

                return Ok(vote);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Delete a vote from question answer
        /// </summary>
        /// <param name="answerId"></param>
        /// <param name="voteId"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        [HttpDelete("~/api/questions/{questionId}/answers/{answerId}/votes/{voteId}", Name = "deleteVote")]
        [ProducesResponseType(202)]
        public async Task<IActionResult> Delete([FromRoute] string answerId,
            [FromRoute] string voteId, [FromServices] VoteStore store)
        {
            await store.Delete(answerId, voteId);

            return Accepted();
        }
    }
}
