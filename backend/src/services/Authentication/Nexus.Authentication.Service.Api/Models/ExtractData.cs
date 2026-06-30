using Microsoft.AspNetCore.Mvc;

namespace Nexus.Authentication.Service.Api.Models
{
   public sealed class ExtractData() 
    {
        public Guid UserId { get; set; }
        public string AccessToken { get; set; } = null!;
        public IActionResult Result { get; set; } = null!;
    }
}