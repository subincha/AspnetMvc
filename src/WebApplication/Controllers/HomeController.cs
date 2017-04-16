using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using WebApplication.Models;
using WebApplication.Data;
using WebApplication.Services.EmployeeServices;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly EmployeeService eService;

        public HomeController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            eService = new EmployeeService(context);
        }

        public IActionResult Index()
        {
            if(_signInManager.IsSignedIn(User))
            {
                var usr = _context.Users.Where(u => u.Id == _userManager.GetUserId(User)).Single();
                TempData["EmployeeLoginId"] = usr.EmployeeId;
                var isSupervisor = eService.isSupervisor(usr.EmployeeId);
                if(isSupervisor)
                {
                    TempData["EmployeeLoginUserType"] = isSupervisor;
                }    
            }
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            if (_signInManager.IsSignedIn(User))
            {
                var usr = _context.Users.Where(u => u.Id == _userManager.GetUserId(User)).Single();
                TempData["EmployeeLoginId"] = usr.EmployeeId;
                var isSupervisor = eService.isSupervisor(usr.EmployeeId);
                if (isSupervisor)
                {
                    TempData["EmployeeLoginUserType"] = isSupervisor;
                }
            }
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            if (_signInManager.IsSignedIn(User))
            {
                var usr = _context.Users.Where(u => u.Id == _userManager.GetUserId(User)).Single();
                TempData["EmployeeLoginId"] = usr.EmployeeId;
                var isSupervisor = eService.isSupervisor(usr.EmployeeId);
                if (isSupervisor)
                {
                    TempData["EmployeeLoginUserType"] = isSupervisor;
                }
            }
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
