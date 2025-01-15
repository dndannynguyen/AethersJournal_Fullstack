using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AethersJournal.Pages.Shared
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string? FirstName { get; set; }
        [BindProperty]
        public string? LastName { get; set; }
        [BindProperty]
        public string? Email { get; set; }
        [BindProperty]
        public string? Password { get; set; }
        [BindProperty]
        public string? ConfirmPassword { get; set; }

        public void OnGet()
        {
        }
    }
}
