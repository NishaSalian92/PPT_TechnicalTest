// Controllers/ImageController.cs
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImageBackend.Data;
using ImageBackend.Models;

namespace ImageBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ImageContext _context;
        private static readonly HttpClient client = new HttpClient();

        public ImageController(ImageContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetImage([FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User identifier is required");
            }

            char lastChar = userId[userId.Length - 1];
            int lastDigit = lastChar - '0';
            bool hasVowel = userId.IndexOfAny(new char[] { 'a', 'e', 'i', 'o', 'u' }) >= 0;
            bool hasNonAlphanumeric = userId.Any(ch => !char.IsLetterOrDigit(ch));

            string imageUrl = "https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150";

            if ("6789".Contains(lastChar))
            {
                // Rule 1: Last digit 6-9
                try
                {
                    var response = await client.GetStringAsync($"https://my-json-server.typicode.com/ck-pacificdev/tech-test/images/{lastDigit}");
                    var image = Newtonsoft.Json.JsonConvert.DeserializeObject<Image>(response);
                    imageUrl = image.Url;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching image from external server: {ex.Message}");
                }
            }
            else if ("12345".Contains(lastChar))
            {
                // Rule 2: Last digit 1-5
                var image = await _context.Images.FindAsync(lastDigit);
                if (image != null)
                {
                    imageUrl = image.Url;
                }
            }
            else if (hasVowel)
            {
                // Rule 3: Contains vowel
                imageUrl = "https://api.dicebear.com/8.x/pixel-art/png?seed=vowel&size=150";
            }
            else if (hasNonAlphanumeric)
            {
                // Rule 4: Contains non-alphanumeric
                var random = new Random();
                int randomNumber = random.Next(1, 6);
                imageUrl = $"https://api.dicebear.com/8.x/pixel-art/png?seed={randomNumber}&size=150";
            }

            return Ok(new { imageUrl });
        }
    }
}
