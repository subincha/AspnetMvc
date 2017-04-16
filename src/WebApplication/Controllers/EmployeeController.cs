using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.Services.EmployeeServices;
using WebApplication.Models.EmployeeViewModels;
using WebApplication.Models.EmployeeModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using WebApplication.Models;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using WebApplication.Services;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System;
using WebApplication.Services.CSVFormatters;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace WebApplication.Controllers
{

    public class EmployeeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly ApplicationDbContext _context;
        private EmployeeService eService;
        private readonly IHostingEnvironment _environment;

        public EmployeeController(ApplicationDbContext context, UserManager<ApplicationUser> userManger, IHostingEnvironment environment)
        {
            _context = context;
            _userManger = userManger;
            eService = new EmployeeService(_context);
            _environment = environment;
        }

        [Authorize(Roles = "Admin")]
        // GET: Employees
        public ActionResult Index()
        {
            var applicationDbContext = eService.getAllEmployees().ToList();
            var indexWithImages = eService.getAllEmployeesWithImage();
            ViewBag.DepartmentId = eService.getAllDeptObjs();
            var usr = _context.Users.Where(u => u.Id == _userManger.GetUserId(User)).Single();
            if ((eService.isValidEmployee(usr.EmployeeId)))
            {
                ViewBag.ImageUrl = eService.getPersonalInfoById(usr.EmployeeId).ImageUrl;
            }
            //string fileName = string.Format("{0}\\test.csv", System.AppContext.BaseDirectory);
            //CsvWriter csvWriter = new CsvWriter();
            //csvWriter.Write(applicationDbContext, fileName, true);
            var edVM = from e in _context.Employees
                       join d in _context.Departments on e.DepartmentId equals d.Id
                       select new
                       {
                           FirstName = e.FirstName,
                           MiddleName = e.MiddleName,
                           LastName = e.LastName,
                           DepartmentName = d.DepartmentName,
                           OfficeNumber = e.OfficeNumber,
                           ExtensionNumber = e.ExtensionNumber,
                           Status = e.Status
                       };
            Console.WriteLine(edVM);
            return View(indexWithImages);
        }

        [Authorize(Roles = "Admin, User")]
        // GET: Employees
        public ActionResult Supervisor(int id)
        {
            var usr = _context.Users.Where(u => u.Id == _userManger.GetUserId(User)).Single();

            if (usr.EmployeeId != id)
            {
                ViewBag.Error = "Permission not Granted";
                return View("Error");
            }
            TempData["EmployeeLoginId"] = usr.EmployeeId;
            var isSupervisor = eService.isSupervisor(usr.EmployeeId);
            if (isSupervisor)
            {
                TempData["EmployeeLoginUserType"] = isSupervisor;
                var emp = eService.getEmployeeById(usr.EmployeeId);
                string empName;
                if (emp.MiddleName != null)
                {
                    empName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;

                }
                else
                {
                    empName = emp.FirstName + " " + emp.LastName;
                }
                ViewBag.SupervisorName = empName;
                ViewBag.SupervisorPosition = eService.getJobDetailById(usr.EmployeeId).Title;
                var image = eService.getPersonalInfoById(usr.EmployeeId);
                if (image != null)
                {
                    ViewBag.SupervisorImage = image.ImageUrl;
                }
                //var applicationDbContext = eService.getAllEmployeesOfSupervisor(empName);
                var indexWithImages = eService.getAllEmployeesOfSupervisorWithImage(empName);
                ViewBag.DepartmentId = eService.getAllDeptObjs();
                return View(indexWithImages);
            }
            else
            {
                ViewBag.Error = "Permission not Granted";
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult CreateEmployeeAndJobDetail()
        {
            setDropDowns();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEmployeeAndJobDetail(EmployeeDetailViewModel employeeDetailViewModel)
        {
            setDropDowns();
            if (ModelState.IsValid)
            {
                if (eService.getEmployeeByEmail(employeeDetailViewModel.Email) == null)
                {
                    int employeeId = eService.addEmployeeDetails(employeeDetailViewModel);
                    eService.addJobDetails(employeeDetailViewModel, employeeId);
                    TempData["EmployeeId"] = employeeId;
                    return RedirectToAction("EditContract", new { id = employeeId });
                } else
                {
                    ModelState.AddModelError("Email", "Email already exits");
                }
            }
            return View(employeeDetailViewModel);
        }


        [HttpPost]
        public ActionResult AddDepartment(string deptName)
        {
            int id = eService.addDepartment(deptName);
            if (id == 0)
            {
                return Json(new { view = "<p> Department name already exits in database</p>", id = id });
            }
            //ViewBag.DepartmentId = eService.getAllDept();
            return Json(new { view = "<p> Successfully added</p>", id = id, item = deptName });
        }


        [HttpPost]
        public ActionResult AddExpectedWorkSchedual(string workDays, string workHours)
        {
            int id = eService.addExpectedWorkSchedual(workDays, workHours);
            if (id == 0)
            {
                return Json(new { view = "<p> Work schedule already exits in database</p>", id = id });
            }
            //ViewBag.DepartmentId = eService.getAllDept();
            return Json(new { view = "<p> Successfully added</p>", id = id, item = workDays + " " + workHours });
        }

        [Authorize(Roles = "Admin")]
        // GET: Employees/Edit/5
        public ActionResult EditEmployee(int? id)
        {
            var usr = _context.Users.Where(u => u.Id == _userManger.GetUserId(User)).Single();
            TempData["EmployeeLoginId"] = usr.EmployeeId;
            if (id == null)
            {
                ViewBag.Error = "User not found";
                return View("Error");
            }
            if (!User.IsInRole("Admin"))
            {
                ViewBag.ViewToRender = "User";

                var isSupervisor = eService.isSupervisor(usr.EmployeeId);
                if (isSupervisor)
                {
                    TempData["EmployeeLoginUserType"] = isSupervisor;
                    // ViewBag.ViewToRender = "Admin";
                    var empList = eService.getAllEmployeesOfSupervisor(eService.nameOfSupervisor(usr.EmployeeId));
                    var iSIdExits = empList.Exists(x => x.Id == id);
                    if (!iSIdExits && usr.EmployeeId != id)
                    {
                        ViewBag.Error = "Permission not Granted";
                        return View("Error");
                    }
                }
                else
                {
                    if (usr.EmployeeId != id)
                    {
                        ViewBag.Error = "Permission not Granted";
                        return View("Error");
                    }
                }



            }
            else
            {
                ViewBag.ViewToRender = "Admin";
            }
            //var editViewModel = eService.getEditViewModel(int id);
            if (eService.isValidEmployee((int)id))
            {
                var edVM = eService.getEditDetailsViewModel(id);
                ViewBag.EmployeeId = edVM.EmployeeId;
                //ViewBag.DepartmentId = eService.editAllDept(edVM.DepartmentId);
                //ViewBag.WorkSchedualId = new SelectList(eService.getAllWorkSchedual(), "Id", "WorkDays", edVM.WorkSchedualId);
                //List<string> sNames = new List<string>();
                //foreach (var s in eService.getAllSupervisor())
                //{
                //    if (s.MiddleName != null)
                //    {
                //        sNames.Add(s.FirstName + " " + s.MiddleName + " " + s.LastName);
                //    }
                //    else
                //    {
                //        sNames.Add(s.FirstName + " " + s.LastName);
                //    }
                //}
                //ViewBag.Supervior = new SelectList(sNames);
                setDropDowns();
                return View(edVM);
            }
            else
            {
                ViewBag.Error = "User not found";
                return View("Error");
            }
        }

        //[Authorize(Roles = "User")]
        //public ActionResult EditEmployeeByUser(int? id)
        //{
        //    if (id == null)
        //    {
        //        ViewBag.Error = "User not found";
        //        return View("Error");
        //    }
        //    //var editViewModel = eService.getEditViewModel(int id);
        //    var usr = _context.Users.Where(u => u.Id == _userManger.GetUserId(User)).Single();
        //    if (eService.isValidEmployee((int)id) && usr.EmployeeId == id)
        //    {
        //        var edVM = eService.getEditDetailsViewModel(id);
        //        ViewBag.EmployeeId = edVM.EmployeeId;
        //        ViewBag.DepartmentId = eService.editAllDept(edVM.DepartmentId);
        //        ViewBag.WorkSchedualId = new SelectList(eService.getAllWorkSchedual(), "Id", "WorkDays", edVM.WorkSchedualId);
        //        return View(edVM);
        //    }
        //    else
        //    {
        //        ViewBag.Error = "Permission not Granted";
        //        return View("Error");
        //    }
        //}

        [Authorize(Roles = "Admin")]
        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmployee(int id, EmployeeDetailViewModel edVM)
        {
            if (id != edVM.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(employee);
                    //await _context.SaveChangesAsync();
                    //Update Logic Here
                    eService.getEditDetailsViewModel(id, edVM);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(eService.isValidEmployee(edVM.EmployeeId)))
                    {
                        ViewBag.Error = "User not found";
                        return View("Error");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("EditContract", new { id = id });
            }
            //ViewBag.DepartmentId = eService.editAllDept(edVM.DepartmentId);
            //ViewBag.WorkSchedualId = new SelectList(eService.getAllWorkSchedual(), "Id", "WorkDays", edVM.WorkSchedualId);
            setDropDowns();
            return View(edVM);
        }

        [Authorize(Roles = "Admin")]
        // GET: Employees/Edit/5
        public ActionResult EditContract(int? id)
        {
            var usr = _context.Users.Where(u => u.Id == _userManger.GetUserId(User)).Single();
            TempData["EmployeeLoginId"] = usr.EmployeeId;
            if (id == null)
            {
                return NotFound();
            }
            //var editViewModel = eService.getEditViewModel(int id);
            if (!User.IsInRole("Admin"))
            {
                ViewBag.ViewToRender = "User";

                var isSupervisor = eService.isSupervisor(usr.EmployeeId);
                if (isSupervisor)
                {
                    TempData["EmployeeLoginUserType"] = isSupervisor;
                    //ViewBag.ViewToRender = "Admin";
                    var empList = eService.getAllEmployeesOfSupervisor(eService.nameOfSupervisor(usr.EmployeeId));
                    var iSIdExits = empList.Exists(x => x.Id == id);
                    if (!iSIdExits && usr.EmployeeId != id)
                    {
                        ViewBag.Error = "Permission not Granted";
                        return View("Error");
                    }
                }
                else
                {
                    if (usr.EmployeeId != id)
                    {
                        ViewBag.Error = "Permission not Granted";
                        return View("Error");
                    }
                }



            }
            else
            {
                ViewBag.ViewToRender = "Admin";
            }
            var contract = eService.getEditContractView(id);
            if (contract != null)
            {
                ViewBag.EmployeeId = contract.EmployeeId;
                TempData["EmployeeId"] = contract.EmployeeId;
                return View(contract);
            }
            else
            {
                if (eService.isValidEmployee((int)id))
                {
                    ViewBag.EmployeeId = eService.getEmployeeById(id).Id;
                    TempData["EmployeeId"] = eService.getEmployeeById(id).Id;
                    return View();
                }
                else
                {
                    ViewBag.Error = "User not found";
                    return View("Error");
                }
            }
        }

        [Authorize(Roles = "Admin")]
        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditContract(int id, Contract contract)
        {
            if (id != contract.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(employee);
                    //await _context.SaveChangesAsync();
                    //Update Logic Here
                    eService.getEditContractView(id, contract);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(eService.isValidEmployee(contract.EmployeeId)))
                    {
                        ViewBag.Error = "User not found";
                        return View("Error");
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["EmployeeLoginId"] = contract.EmployeeId;
                return RedirectToAction("EditPersonalInfo", new { id = id });
            }
            return View(contract);
        }

        [Authorize(Roles = "Admin, User")]
        // GET: Employees/Edit/5
        public ActionResult EditPersonalInfo(int? id)
        {
            var usr = _context.Users.Where(u => u.Id == _userManger.GetUserId(User)).Single();
            TempData["EmployeeLoginId"] = usr.EmployeeId;
            if (id == null)
            {
                return NotFound();
            }
            //var editViewModel = eService.getEditViewModel(int id);
            if (!User.IsInRole("Admin"))
            {
                ViewBag.ViewToRender = "User";

                var isSupervisor = eService.isSupervisor(usr.EmployeeId);
                if (isSupervisor)
                {
                    TempData["EmployeeLoginUserType"] = isSupervisor;
                    // ViewBag.ViewToRender = "Admin";
                    var empList = eService.getAllEmployeesOfSupervisor(eService.nameOfSupervisor(usr.EmployeeId));
                    var iSIdExits = empList.Exists(x => x.Id == id);
                    if (!iSIdExits && usr.EmployeeId != id)
                    {
                        ViewBag.Error = "Permission not Granted";
                        return View("Error");
                    }
                }
                else
                {
                    if (usr.EmployeeId != id)
                    {
                        ViewBag.Error = "Permission not Granted";
                        return View("Error");
                    }
                }
            }
            else
            {
                ViewBag.ViewToRender = "Admin";
            }
            var p = eService.getEditPersonalInfoView(id);
            if (p != null)
            {
                ViewBag.EmployeeId = p.EmployeeId;
                TempData["EmployeeId"] = p.EmployeeId;
                return View(p);
            }
            else
            {
                if (eService.isValidEmployee((int)id))
                {
                    ViewBag.EmployeeId = eService.getEmployeeById(id).Id;
                    TempData["EmployeeId"] = eService.getEmployeeById(id).Id;
                    return View();
                }
                else
                {
                    ViewBag.Error = "User not found";
                    return View("Error");
                }
            }
        }

        [Authorize(Roles = "Admin, User")]
        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPersonalInfo(int id, PersonalInfo personalInfo)
        {
            if (id != personalInfo.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(employee);
                    //await _context.SaveChangesAsync();
                    //Update Logic Here
                    eService.getEditPersonalInfoView(id, personalInfo);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(eService.isValidEmployee(personalInfo.EmployeeId)))
                    {
                        ViewBag.Error = "User not found";
                        return View("Error");
                    }
                    else
                    {
                        throw;
                    }
                }
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["EmployeeLoginId"] = personalInfo.EmployeeId;
                    return RedirectToAction("EmployeeDetails", new { id = personalInfo.EmployeeId });
                }
            }
            return View(personalInfo);
        }

        [Authorize(Roles = "Admin")]
        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "User not found";
                return View("Error");
            }
            var employee = await _context.Employees.SingleOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                ViewBag.Error = "User not found";
                return View("Error");
            }
            ViewBag.DepartmentId = eService.getAllDeptObjs();
            return View(employee);
        }

        [Authorize(Roles = "Admin")]
        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            eService.removeEmployee(id);
            eService.removeJobDetail(id);
            eService.removeContract(id);
            eService.removePersonalInfo(id);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin, User")]
        // GET: Employees/Details/5
        public IActionResult EmployeeDetails(int? id)
        {
            var usr = _context.Users.Where(u => u.Id == _userManger.GetUserId(User)).Single();
            TempData["EmployeeLoginId"] = usr.EmployeeId;
            if (id == null)
            {
                ViewBag.Error = "User not found";
                return View("Error");
            }
            if (!User.IsInRole("Admin"))
            {
                ViewBag.ViewToRender = "User";

                var isSupervisor = eService.isSupervisor(usr.EmployeeId);
                if (isSupervisor)
                {
                    TempData["EmployeeLoginUserType"] = isSupervisor;
                    //ViewBag.ViewToRender = "Admin";
                    var empList = eService.getAllEmployeesOfSupervisor(eService.nameOfSupervisor(usr.EmployeeId));
                    var iSIdExits = empList.Exists(x => x.Id == id);
                    if (!iSIdExits && usr.EmployeeId != id)
                    {
                        ViewBag.Error = "Permission not Granted";
                        return View("Error");
                    }
                }
                else
                {
                    if (usr.EmployeeId != id)
                    {
                        ViewBag.Error = "Permission not Granted";
                        return View("Error");
                    }
                }



            }
            else
            {
                ViewBag.ViewToRender = "Admin";
            }

            var employee = eService.getEmployeeById((int)id);
            var jobDetail = eService.getJobDetailById((int)id);
            if (employee == null || jobDetail == null)
            {
                ViewBag.Error = "User not found";
                return View("Error");
            }
            var edVM = eService.addEmployeeAndJobDetails(employee, jobDetail);
            ViewBag.DepartmentId = eService.getAllDeptObjs();
            ViewBag.WorkSchedualId = eService.getAllWprkSchedualObjs();
            TempData["EmployeeId"] = edVM.EmployeeId;
            ViewBag.EmployeeId = edVM.EmployeeId;
            return View(edVM);
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult ContractDetails(int? id)
        {
            var usr = _context.Users.Where(u => u.Id == _userManger.GetUserId(User)).Single();
            TempData["EmployeeLoginId"] = usr.EmployeeId;
            if (id == null)
            {
                ViewBag.Error = "Contract not found for given user";
                return View("DetailsError");
            }
            if (!User.IsInRole("Admin"))
            {
                ViewBag.ViewToRender = "User";

                var isSupervisor = eService.isSupervisor(usr.EmployeeId);
                if (isSupervisor)
                {
                    TempData["EmployeeLoginUserType"] = isSupervisor;
                    //   ViewBag.ViewToRender = "Admin";
                    var empList = eService.getAllEmployeesOfSupervisor(eService.nameOfSupervisor(usr.EmployeeId));
                    var iSIdExits = empList.Exists(x => x.Id == id);
                    if (!iSIdExits && usr.EmployeeId != id)
                    {
                        ViewBag.Error = "Permission not Granted";
                        return View("Error");
                    }
                }
                else
                {
                    if (usr.EmployeeId != id)
                    {
                        ViewBag.Error = "Permission not Granted";
                        return View("Error");
                    }
                }



            }
            else
            {
                ViewBag.ViewToRender = "Admin";
            }
            var contract = eService.getContractById((int)id);
            if (contract == null)
            {
                TempData["EmployeeId"] = id;
                ViewBag.Error = "Contract not found for given user";
                return View("DetailsError");
            }
            TempData["EmployeeId"] = contract.EmployeeId;
            return View(contract);
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult PersonalInfoDetails(int? id)
        {
            var usr = _context.Users.Where(u => u.Id == _userManger.GetUserId(User)).Single();
            TempData["EmployeeLoginId"] = usr.EmployeeId;
            if (id == null)
            {
                ViewBag.Error = "Personal information not found for given user";
                return View("DetailsError");
            }
            if (!User.IsInRole("Admin"))
            {
                ViewBag.ViewToRender = "User";

                var isSupervisor = eService.isSupervisor(usr.EmployeeId);
                if (isSupervisor)
                {
                    TempData["EmployeeLoginUserType"] = isSupervisor;
                    //  ViewBag.ViewToRender = "Admin";
                    var empList = eService.getAllEmployeesOfSupervisor(eService.nameOfSupervisor(usr.EmployeeId));
                    var iSIdExits = empList.Exists(x => x.Id == id);
                    if (!iSIdExits && usr.EmployeeId != id)
                    {
                        ViewBag.Error = "Permission not Granted";
                        return View("Error");
                    }
                }
                else
                {
                    if (usr.EmployeeId != id)
                    {
                        ViewBag.Error = "Permission not Granted";
                        return View("Error");
                    }
                }



            }
            else
            {
                ViewBag.ViewToRender = "Admin";
            }
            var personalInfo = eService.getPersonalInfoById((int)id);
            if (personalInfo == null)
            {
                TempData["EmployeeId"] = id;
                ViewBag.Error = "Personal information not found for given user";
                return View("DetailsError");
            }
            TempData["EmployeeId"] = personalInfo.EmployeeId;
            return View(personalInfo);
        }

        [HttpPost]
        public async Task<IActionResult> GetUserId(string id)
        {
            var eId = await _context.Users.SingleAsync(e => e.Id == id);
            return Json(new { id = eId.EmployeeId });
        }

        [HttpPost]
        public async Task<IActionResult> FileUpload(int id)
        {
            var files = Request.Form.Files;
            var empId = TempData["EmployeeId"] ?? id;
            
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            string status;
            status = "<p> No file chosen</p>";
            bool value = false;
            string url = "";
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                        status = "<p> Successfully Uploaded</p>";
                        value = true;
                        url = "/uploads/" + file.FileName;

                        eService.getEditPersonalInfoView((int)empId, url);
                    }
                }
            }

            TempData["EmployeeId"] = empId;
            return Json(new { status = status, value = value, url = url });
        }

        [HttpPost]
        public IActionResult CSVFileUpload()
        {
            var files = Request.Form.Files;
            string status = "No file chosen";
            bool success = false;
            bool already = false;
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        var fileContent = reader.ReadToEnd();
                        var rows = fileContent.Split('\n');
                        if (!rows[0].Replace("\r", "").Replace("\n", "").Equals("FirstName,LastName,MiddleName,Email,DepartmentName,OfficeNumber,ExtensionNumber,Status,Title,Level,Supervisor,ExpectedJoinDate,WorkDays,WorkHours"))
                        {
                            return Json(new { status = "File format error, headers doesn't match", success = success });
                        }
                        for (int r = 1; r < rows.Length - 1; r++)
                        {
                            var columns = rows[r].Split(',');
                            if (eService.getEmployeeByEmail(columns[3]) == null)
                            {

                                //if (!eService.isValidEmployee(Convert.ToInt32(columns[0]))) {
                                Employee e = new Employee();
                                JobDetail j = new JobDetail();

                                // e.Id = Convert.ToInt32(columns[0]);
                                e.FirstName = columns[0];
                                e.LastName = columns[1];
                                if (columns[2] == "")
                                {
                                    e.MiddleName = null;
                                }
                                else
                                {
                                    e.MiddleName = columns[2];
                                }

                                e.Email = columns[3];
                                e.DepartmentId = eService.getDepartmentIdByName(columns[4]);
                                if (columns[5] == "")
                                {
                                    e.OfficeNumber = null;
                                }
                                else
                                {
                                    e.OfficeNumber = columns[5];
                                }
                                if (columns[6] == "")
                                {
                                    e.ExtensionNumber = null;
                                }
                                else
                                {
                                    e.ExtensionNumber = columns[6];
                                }
                                if (columns[7] == "Active")
                                {
                                    e.Status = 1;
                                }
                                else
                                {
                                    e.Status = 0;
                                }
                                _context.Employees.Add(e);
                                _context.SaveChanges();
                                j.EmployeeId = e.Id;
                                j.Title = columns[8];
                                j.Level = columns[9];
                                if (columns[10] == "")
                                {
                                    j.Supervisor = null;
                                }
                                else
                                {
                                    j.Supervisor = columns[10];
                                }
                                j.ExpectedJoinDate = Convert.ToDateTime(columns[11]);
                                j.WorkSchedualId = eService.getAllWorkSchedualID(columns[12], columns[13].Replace("\r", ""));

                                _context.JobDetails.Add(j);
                                _context.SaveChanges();
                                //  }
                                status = "Uploaded Successfully";
                                success = true;
                                if(already)
                                {
                                    status = "Uploaded Successfully, Some Users already exits";
                                }
                            }
                            else
                            {
                                already = true;
                                if (success)
                                {
                                    status = "Uploaded Successfully, Some Users already exits";
                                } else
                                {
                                    status = "All Users already exits";
                                }
                            }
                        }
                    }
                }
            }
            return Json(new { status = status, success = success });
        }


        [HttpPost]
        public IActionResult FileRemove(int id)
        {
            var empId = TempData["EmployeeId"];
            if (empId == null)
                empId = id;

            eService.removeFileUrl((int)empId);
            TempData["EmployeeId"] = empId;
            return Json(new { status = "<p> Sucessfully Removed </p>" });
        }


        //// [Route("[controller]/data.csv")]
        //public void ExportToCSV(string id)
        //{
        //    StringWriter writer = new StringWriter();
        //    writer.WriteLine("EmployeeId,FirstName,LastName,MiddleName,Email,DepartmentName,OfficeNumber,ExtensionNumber,Status,Title,Level,Supervisor,ExpectedJoinDate,WorkDays,WorkHours");
        //    Response.Clear();
        //    Response.ContentType = "text/csv";
        //    Response.Headers.Add("content-disposition", "attachment; filename=Export.csv");
        //    //var applicationDbContext = eService.getAllEmployees().ToList();
        //    string[] ids = id.Split(',');
        //    foreach (string i in ids)
        //    {
        //        var emp = eService.getEditDetailsViewModelCSV((int)Convert.ToInt32(i));
        //        writer.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}", emp.EmployeeId, emp.FirstName, emp.LastName, emp.MiddleName, emp.Email, emp.DepartmentName, emp.OfficeNumber, emp.ExtensionNumber, emp.Status, emp.Title, emp.Level, emp.Supervisor, emp.ExpectedJoinDate, emp.WorkDays, emp.WorkHours));
        //    }
        //    Response.WriteAsync(writer.ToString());
        //}

        [HttpGet]
        [Produces("text/csv")]
        public IActionResult ExportToCSV(string id)
        {
            Response.Clear();
            Response.ContentType = "application/csv";
            Response.Headers.Add("content-disposition", "attachment; filename=Employee.csv");
            return Ok(GetData(id));
        }


        private IEnumerable<EmployeeDetailViewModelCSV> GetData(string id)
        {
            List<EmployeeDetailViewModelCSV> eDMVList = new List<EmployeeDetailViewModelCSV>();

            string[] ids = id.Split(',');
            foreach (string i in ids)
            {
                EmployeeDetailViewModelCSV eMDV = eService.getEditDetailsViewModelCSV((int)Convert.ToInt32(i));
                eDMVList.Add(eMDV);
            }
            return eDMVList;
        }

        public void setDropDowns()
        {
            ViewBag.DepartmentId = eService.getAllDept();
            ViewBag.WorkSchedualId = new SelectList(eService.getAllWorkSchedual(), "Id", "WorkDays");

            List<string> sNames = new List<string>();
            foreach (var s in eService.getAllSupervisor())
            {
                if (s.MiddleName != null)
                {
                    sNames.Add(s.FirstName + " " + s.MiddleName + " " + s.LastName);
                }
                else
                {
                    sNames.Add(s.FirstName + " " + s.LastName);
                }
            }
            ViewBag.Supervior = new SelectList(sNames);
        }

        public FileContentResult ExportToExcel(string id)
        {
            //column Header name
            var columnsHeader = new List<string>{
                "S/N",
                "Id",
                "First Name",
                "Middle Name",
                "Last Name",
                "Email",
                "Department Name",
                "Status",
                "Extension Number",
                "Office Number",
                "Title",
                "Level",
                "Supervisor Name",
                "Expected Join Date",
                "Work Days",
                "Work Hours"
            };

            List<EmployeeDetailViewModelCSV> eDMVList = new List<EmployeeDetailViewModelCSV>();

            string[] ids = id.Split(',');
            foreach (string i in ids)
            {
                EmployeeDetailViewModelCSV eMDV = eService.getEditDetailsViewModelCSV((int)Convert.ToInt32(i));
                eDMVList.Add(eMDV);
            }
            var filecontent = ExportExcel(eDMVList.ToList(), columnsHeader, "Employees");
            return File(filecontent, "application/ms-excel", "Employee.xlsx"); 
        }

        public FileContentResult ExportToPDF(string id)
        {
            //column Header name
            var columnsHeader = new List<string>{
                "S/N",
                "Id",
                "First Name",
                "Middle Name",
                "Last Name",
                "Email",
                "Department Name",
                "Status",
                "Extension Number",
                "Office Number",
                "Title",
                "Level",
                "Supervisor Name",
                "Expected Join Date",
                "Work Days",
                "Work Hours"
            };

            List<EmployeeDetailViewModelCSV> eDMVList = new List<EmployeeDetailViewModelCSV>();

            string[] ids = id.Split(',');
            foreach (string i in ids)
            {
                EmployeeDetailViewModelCSV eMDV = eService.getEditDetailsViewModelCSV((int)Convert.ToInt32(i));
                eDMVList.Add(eMDV);
            }
            var filecontent = ExportPDF(eDMVList.ToList(), columnsHeader, "Employees");
            return File(filecontent, "application/pdf", "Employee.pdf");
        }


        private byte[] ExportExcel(List<EmployeeDetailViewModelCSV> dataList, List<string> columnsHeader, string heading)
        {
            byte[] result = null;

            using (ExcelPackage package = new ExcelPackage())
            {
                // add a new worksheet to the empty workbook
                var worksheet = package.Workbook.Worksheets.Add(heading);
                using (var cells = worksheet.Cells[1, 1, 1, 16])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.Aqua);
                }
                //First add the headers
                for (int i = 0; i < columnsHeader.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = columnsHeader[i];
                }

                //Add values
                var j = 2;
                var count = 1;
                foreach (var item in dataList)
                {
                    worksheet.Cells["A" + j].Value = count;
                    worksheet.Cells["B" + j].Value = item.EmployeeId;
                    worksheet.Cells["C" + j].Value = item.FirstName;
                    worksheet.Cells["D" + j].Value = item.MiddleName;
                    worksheet.Cells["E" + j].Value = item.LastName;
                    worksheet.Cells["F" + j].Value = item.Email;
                    worksheet.Cells["G" + j].Value = item.DepartmentName;
                    worksheet.Cells["H" + j].Value = item.Status;
                    worksheet.Cells["I" + j].Value = item.ExtensionNumber;
                    worksheet.Cells["J" + j].Value = item.OfficeNumber;
                    worksheet.Cells["K" + j].Value = item.Title;
                    worksheet.Cells["L" + j].Value = item.Level;
                    worksheet.Cells["M" + j].Value = item.Supervisor;
                    worksheet.Cells["N" + j].Value = item.ExpectedJoinDate.ToString();
                    worksheet.Cells["O" + j].Value = item.WorkDays;
                    worksheet.Cells["P" + j].Value = item.WorkHours;
                    j++;
                    count++;
                }
                result = package.GetAsByteArray();
            }
            return result;
        }

        private byte[] ExportPDF(List<EmployeeDetailViewModelCSV> dataList, List<string> columnsHeader, string heading)
        {

            var document = new Document();
            var outputMS = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, outputMS);
            document.Open();
            var font5 = FontFactory.GetFont(FontFactory.HELVETICA, 6);

            document.Add(new Phrase(Environment.NewLine));
            document.SetPageSize(PageSize.A4.Rotate());
            //var count = typeof(UserListVM).GetProperties().Count();
            var count = columnsHeader.Count;
            var table = new PdfPTable(count);
            float[] widths = new float[] { 2f, 4f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 8f };
            
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            var cell = new PdfPCell(new Phrase(heading));
            cell.Colspan = count;

            for (int i = 0; i < count; i++)
            {
                var headerCell = new PdfPCell(new Phrase(columnsHeader[i], font5));
                headerCell.BackgroundColor = BaseColor.Gray;
                table.AddCell(headerCell);
            }

            var sn = 1;
            foreach (var item in dataList)
            {
                table.AddCell(new Phrase(sn.ToString(), font5));
                table.AddCell(new Phrase(item.EmployeeId.ToString(), font5));
                table.AddCell(new Phrase(item.FirstName, font5));
                table.AddCell(new Phrase(item.MiddleName, font5));
                table.AddCell(new Phrase(item.LastName, font5));
                table.AddCell(new Phrase(item.Email, font5));
                table.AddCell(new Phrase(item.DepartmentName, font5));
                table.AddCell(new Phrase(item.Status, font5));
                table.AddCell(new Phrase(item.ExtensionNumber, font5));
                table.AddCell(new Phrase(item.OfficeNumber, font5));
                table.AddCell(new Phrase(item.Title, font5));
                table.AddCell(new Phrase(item.Level, font5));
                table.AddCell(new Phrase(item.Supervisor, font5));
                table.AddCell(new Phrase(item.ExpectedJoinDate.ToString(), font5));
                table.AddCell(new Phrase(item.WorkDays, font5));
                table.AddCell(new Phrase(item.WorkHours, font5));
                sn++;
            }

            document.Add(table);
            document.Close();
            var result = outputMS.ToArray();

            return result;
        }
    }


}