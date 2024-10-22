using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WindTalkerMessenger.Models.DataLayer;
using WindTalkerMessenger.Services;

namespace WindTalkerMessenger.Controllers
{
    public class ValidationController : Controller
    {

        private readonly IContextService _services;
        private readonly OnlineUsersLists _onlineUsersLists;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ValidationController(IContextService contextservice,UserManager<ApplicationUser> userManager,
                                    ApplicationDbContext context) 
        {
            _userManager = userManager;
            _services = contextservice;
            _context = context;

        }



        
    }
}
