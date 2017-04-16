using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplication.Models;
using WebApplication.Data;
using WebApplication.Services.EmployeeServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication.Controllers
{
    [Authorize(Roles = "User")]
    public class EmployeeUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly ApplicationDbContext _context;
        private EmployeeService eService;

        public EmployeeUserController(ApplicationDbContext context, UserManager<ApplicationUser> userManger)
        {
            _context = context;
            _userManger = userManger;
            eService = new EmployeeService(_context);
        }

        public IActionResult Index()
        {
            return View();
        }

       
        public ActionResult EditEmployeeByUser(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "User not found";
                return View("Error");
            }
            //var editViewModel = eService.getEditViewModel(int id);
            var usr = _context.Users.Where(u => u.Id == _userManger.GetUserId(User)).Single();
            if (eService.isValidEmployee((int)id) && usr.EmployeeId == id)
            {
                var edVM = eService.getEditDetailsViewModel(id);
                ViewBag.EmployeeId = edVM.EmployeeId;
                ViewBag.DepartmentId = eService.editAllDept(edVM.DepartmentId);
                ViewBag.WorkSchedualId = new SelectList(eService.getAllWorkSchedual(), "Id", "WorkDays", edVM.WorkSchedualId);
                return View("~/Views/Employee/EditEmployee.cshtml",edVM);
            }
            else
            {
                ViewBag.Error = "Permission not Granted";
                return View("Error");
            }
        }

        public void dummyFunction()
        {
            
        }
    }
}