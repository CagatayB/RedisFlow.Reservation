using Microsoft.AspNetCore.Mvc;
using ReservationSystem.src.Application.Commands;
using ReservationSystem.src.Application.Services;

namespace ReservationSystem.src.API.Controllers
{
    [ApiController]
    [Route("reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly ReservationService _service;

        public ReservationsController(ReservationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateReservationCommand command,
            [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
        {
            if (string.IsNullOrEmpty(idempotencyKey))
                return BadRequest("Idempotency-Key header required");

            try
            {
                var id = await _service.CreateReservationAsync(command, idempotencyKey);

                return Ok(new { ReservationId = id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
