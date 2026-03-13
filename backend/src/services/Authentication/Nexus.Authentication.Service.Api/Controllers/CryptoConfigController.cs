using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Nexus.Authentication.Service.Api.Controllers
{
    [ApiController]
    [Route("api/auth-config")]
    public class CryptoConfigController : Controller
    {
        private readonly IConfiguration _configuration;

        public CryptoConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("public-key")]
        [AllowAnonymous]
        public IActionResult GetPublicKey()
        {
            var publicKey = _configuration["Crypto:RsaPublicKey"];
            return Ok(new 
            {
                PublicKey = publicKey
            });
        }
    }
}