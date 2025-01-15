using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AethersJournal.Pages
{
    public class JournalModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;

        public JournalModel()
        {
            _cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = _cookieContainer,
            };
            _httpClient = new HttpClient(handler);
        }
        public List<DateTime>? Dates { get; private set; }
        public DateTime pageDateTime { get; private set; }

        [FromQuery]
        public string? date { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var response = await _httpClient.PostAsync("https://aethersjournaldatabase.azurewebsites.net/api/session/validateSession", null);
            // if (!response.IsSuccessStatusCode)
            // {
            //     return RedirectToPage("/Login");
            // }

            if (date == null)
            {
                pageDateTime = DateTime.Today;
                date = DateTime.Today.ToString("yyyy-MM-dd");
            }
            pageDateTime = DateTime.Parse(date!);

            Dates = new List<DateTime>();
            DateTime startDate = DateTime.Today.AddMonths(-1);
            DateTime endDate = DateTime.Today;

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                Dates.Add(date);
            }

            Dates.Reverse();
            return Page();
        }
    }
}
