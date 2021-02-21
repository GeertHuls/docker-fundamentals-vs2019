using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Globomantics.Core.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {

            await Task.CompletedTask;

            // Verify whether idp can be reached from the UI project:
            using var client = new HttpClient();

            var url = "https://id-local.globomantics.com:44395/.well-known/openid-configuration";
            var result = await client.GetAsync(url);

            var url2 = "http://globomantics.identityserver:5000/.well-known/openid-configuration";
            var result2 = await client.GetAsync(url2);
        }
    }
}
