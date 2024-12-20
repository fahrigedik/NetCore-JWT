using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniApp1.API.Controllers
{

    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StockController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetStock()
        {
            var userName = HttpContext.User.Identity.Name;

            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Ok($" admin / Stocks => username : {userId} , userId : {userId}");
        }

        [Authorize(Roles = "manager")]
        [HttpGet]
        public IActionResult GetStockByManagerRoles()
        {
            var userName = HttpContext.User.Identity.Name;

            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Ok($" admin / Stocks => username : {userId} , userId : {userId}");
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult GetStockByAdminRoles()
        {
            var userName = HttpContext.User.Identity.Name;

            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Ok($" admin / Stocks => username : {userId} , userId : {userId}");
        }
    }
}
