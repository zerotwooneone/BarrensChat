using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendServer.Publish;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BackendServer.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IPublishService _publishService;

        public IndexModel(IPublishService publishService)
        {
            _publishService = publishService;
        }
        public void OnGet()
        {
        }
    }
}
