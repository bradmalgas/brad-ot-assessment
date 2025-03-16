using Microsoft.AspNetCore.Mvc;
using OT.Assessment.App.Services.Interfaces;
using OT.Assessment.Shared.Data.Interfaces;
using OT.Assessment.Shared.Models;
namespace OT.Assessment.App.Controllers
{
    [ApiController]
    [Route("api/player")]
    public class PlayerController : ControllerBase
    {
        private readonly ICasinoWagerApiService _apiService;
        public PlayerController(ICasinoWagerApiService apiService)
        {
            _apiService = apiService;
        }

        //POST api/player/casinowager
        [HttpPost("casinowager")]
        public async Task<IActionResult> PublishCasinoWager([FromBody] CasinoWagerEventDTO wager)
        {
            await _apiService.PublishAsync(wager);
            return Ok(new { Message = "Casino wager event published successfully." });
        }

        //GET api/player/{playerId}/wagers
        [HttpGet("{playerId}/casino")]
        public async Task<IActionResult> GetWagersByPlayer([FromRoute] Guid playerId, [FromQuery] int pageSize, [FromQuery] int page)
        {
            if (pageSize < 0 || page < 0)
                return BadRequest("Please enter a valid page and page size.");

            var result = await _apiService.GetWagersByPlayerAsync(playerId: playerId, pageSize: pageSize, page: page);

            return Ok(result);

        }

        //GET api/player/topSpenders?count=10
        [HttpPost("topSpenders")]
        public async Task<IActionResult> GetTopSpenders([FromQuery] int count)
        {
            if (count == 0)
                return BadRequest("Invalid wager data.");

            var result = await _apiService.GetTopSpendersAsync(count);

            return Ok(result);

        }
    }
}
