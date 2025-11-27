using Chirp.Core.DTO;
using Chirp.Core.Services;
using Chirp.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Controllers
{
    public class PublicController : PageModel
    {
        [BindProperty]
        public string? Text { get; set; }
        
        private readonly ICheepService _service;
        private readonly UserManager<Author> _userManager;

        public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
        public int CurrentPage { get; set; }
        
        public string? CurrentAuthorName { get; set; }

        public PublicController(ICheepService service, UserManager<Author> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync([FromQuery] int page = 1)
        {
            if (page < 1) page = 1;

            CurrentPage = page;
            Cheeps = _service.GetCheeps(page);

            var user = await _userManager.GetUserAsync(User);
            CurrentAuthorName = user?.Name; 

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync([FromQuery] int page = 1)
        {
            if (page < 1) page = 1;

            if (!string.IsNullOrWhiteSpace(Text))
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    CurrentAuthorName = user.Name;

                    _service.MakeCheep(new CheepDTO
                    {
                        Author = user.Name,
                        Message = Text,
                        CreatedDate = DateTime.Now.ToString()
                    });
                } 
            }
            else
            {
                // Even if nothing is posted, still set the name for the view
                var user = await _userManager.GetUserAsync(User);
                CurrentAuthorName = user?.Name;
            }

            CurrentPage = page;
            Cheeps = _service.GetCheeps(page);

            return Page();
        }
    }
}
