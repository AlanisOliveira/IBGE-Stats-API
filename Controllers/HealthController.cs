using Microsoft.AspNetCore.Mvc;

namespace IbgeStats.Controllers
{
    [ApiController]
    [Route("/")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public ActionResult<object> GetHealth()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Message = "API funcionando!"
            });
        }
    }
}