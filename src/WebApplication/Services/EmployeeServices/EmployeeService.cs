using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models.EmployeeModels;
using WebApplication.Models.EmployeeViewModels;

namespace WebApplication.Services.EmployeeServices
{
    public class EmployeeService
    {
        private ApplicationDbContext _context;
        public EmployeeService(ApplicationDbContext context) {
            _context = context;
        }
        public SelectList getAllDept() {
            return new SelectList(_context.Departments, "Id", "DepartmentName");
        }

        public List<Employee> getAllSupervisor()
        {
            return _context.JobDetails.Where(s => s.Title.ToLower() == "Supervisor".ToLower()).Select(s => s.Employee).ToList();
        }

        public bool isSupervisor(int employeeId)
        {
            var sup = _context.JobDetails.Where(s => s.EmployeeId == employeeId && s.Title.ToLower() == "Supervisor".ToLower()).SingleOrDefault();
            if(sup != null)
            {
                return true;
            }
            return false;
        }

        public string nameOfSupervisor(int employeeId)
        {
            var sup = _context.Employees.Where(s => s.Id == employeeId).SingleOrDefault();
            string name;
            if (sup.MiddleName != null)
            {
                name = sup.FirstName + " " + sup.MiddleName + " " + sup.LastName;
            } else
            {
                name = sup.FirstName +  " " + sup.LastName;
            }
            return name;
        }

        public List<Department> getAllDeptObjs()
        {
            return _context.Departments.ToList();
        }

        public List<WorkSchedual> getAllWprkSchedualObjs()
        {
            return _context.WorkScheduals.ToList();
        }

        public List<WorkSchedual> getAllWorkSchedual() {
            var workSechduals = _context.WorkScheduals.ToList();
            List<WorkSchedual> wList = new List<WorkSchedual>();
            foreach (var works in workSechduals) {
                WorkSchedual w = new WorkSchedual();
                w.Id = works.Id;
                w.WorkDays = works.WorkDays + " " + works.WorkHours;
                wList.Add(w);
            }
            return wList;// new SelectList(wList, "Id", "WorkDays");
        }

        public int addEmployeeDetails(EmployeeDetailViewModel employeeDetailViewModel) {
            Employee employee = new Employee();
            employee.FirstName = employeeDetailViewModel.FirstName;
            employee.MiddleName = employeeDetailViewModel.MiddleName;
            employee.LastName = employeeDetailViewModel.LastName;
            employee.Email = employeeDetailViewModel.Email;
            employee.DepartmentId = employeeDetailViewModel.DepartmentId;
            employee.Status = employeeDetailViewModel.Status;
            employee.ExtensionNumber = employeeDetailViewModel.ExtensionNumber;
            employee.OfficeNumber = employeeDetailViewModel.OfficeNumber;
            _context.Employees.Add(employee);
            _context.SaveChanges();
            int employeeId = employee.Id;
            return employeeId;
        }

        public object getAllEmployeesOfSupervisorWithImage(string empName)
        {
            var s = _context.JobDetails.Where(su => su.Supervisor == empName);
            //List<Employee> empList = new List<Employee>();
            List<ShowImageUrlViewModel> sIUVMList = new List<ShowImageUrlViewModel>();
            foreach (var sup in s)
            {
                Employee emp = new Employee();
                emp = _context.Employees.SingleOrDefault(e => e.Id == sup.EmployeeId);
                ShowImageUrlViewModel sIUM = new ShowImageUrlViewModel();
                sIUM.Employee = emp;
                var personalDetails = _context.PersonalInfo.SingleOrDefault(p => p.EmployeeId == emp.Id);
                if (personalDetails != null)
                    sIUM.ImageUrl = personalDetails.ImageUrl;
                sIUVMList.Add(sIUM);
            }
            return sIUVMList;
        }

        public List<Employee> getAllEmployeesOfSupervisor(string empName)
        {
            var s = _context.JobDetails.Where(su => su.Supervisor == empName);
            List<Employee> empList = new List<Employee>();
            foreach(var sup in s)
            {
                Employee emp = new Employee();
                emp = _context.Employees.SingleOrDefault(e => e.Id == sup.EmployeeId);
                empList.Add(emp);
            }
            return empList;
        }

        public EmployeeDetailViewModel addEmployeeAndJobDetails(Employee employee, JobDetail jobDetail) {
            EmployeeDetailViewModel edVM = new EmployeeDetailViewModel();
            edVM.EmployeeId = employee.Id;
            edVM.FirstName = employee.FirstName;
            edVM.MiddleName = employee.MiddleName;
            edVM.LastName = employee.LastName;
            edVM.Email = employee.Email;
            edVM.OfficeNumber = employee.OfficeNumber;
            edVM.ExtensionNumber = employee.ExtensionNumber;
            edVM.DepartmentId = employee.DepartmentId;
            edVM.Status = employee.Status;
            edVM.Title = jobDetail.Title;
            edVM.Level = jobDetail.Level;
            edVM.Supervisor = jobDetail.Supervisor;
            edVM.ExpectedJoinDate = jobDetail.ExpectedJoinDate;
            edVM.WorkSchedualId = jobDetail.WorkSchedualId;
            return edVM;
        }

        public void addJobDetails(EmployeeDetailViewModel employeeDetailViewModel, int employeeId) {
            JobDetail jobDetail = new JobDetail();
            jobDetail.EmployeeId = employeeId;
            jobDetail.Title = employeeDetailViewModel.Title;
            jobDetail.Level = employeeDetailViewModel.Level;
            jobDetail.Supervisor = employeeDetailViewModel.Supervisor;
            jobDetail.ExpectedJoinDate = employeeDetailViewModel.ExpectedJoinDate;
            jobDetail.WorkSchedualId = employeeDetailViewModel.WorkSchedualId;
            _context.JobDetails.Add(jobDetail);
            _context.SaveChanges();
        }

        public List<IdentityRole> getAllRoleId()
        {
            return _context.Roles.ToList();
        }

        public int addExpectedWorkSchedual(string workDays, string workHours)
        {
            if(_context.WorkScheduals.Any(t => t.WorkDays.ToLower() == workDays.ToLower() && t.WorkHours.ToLower() == workHours.ToLower()))
            {
                return 0;
            }
            WorkSchedual work = new WorkSchedual();
            work.WorkDays = workDays;
            work.WorkHours = workHours;
            _context.WorkScheduals.Add(work);
            _context.SaveChanges();
            return work.Id;
        }

        public Employee getEmployeeByEmail(string email)
        {
            var emp = _context.Employees.SingleOrDefault(e => e.Email == email);
            return emp;
        }

        public bool isValidEmployee(int employeeId)
        {
            var emp = _context.Employees.Where(e => e.Id == employeeId).FirstOrDefault();
            if (emp != null)
            {
                return true;
            }
            return false;
        }

        public void addPersonalInfo(PersonalInfo personalInfo)
        {
            PersonalInfo p = new PersonalInfo();
            p.District = personalInfo.District;
            p.EmployeeId = personalInfo.EmployeeId;
            p.Zone = personalInfo.Zone;
            p.Tole = personalInfo.Tole;
            p.IsMarried = personalInfo.IsMarried;
            p.SpouseName = personalInfo.SpouseName;
            _context.PersonalInfo.Add(p);
            _context.SaveChanges();
        }

        public void addContract(Contract contract)
        {
            Contract c = new Contract();
            c.EmployeeId = contract.EmployeeId;
            c.ContractType = contract.ContractType;
            c.ContractPeriod = contract.ContractPeriod;
            c.JoinDate = contract.JoinDate;
            c.IsProbation = contract.IsProbation;
            c.ProbationPeriod = contract.ProbationPeriod;
            _context.Contracts.Add(c);
            _context.SaveChanges();

        }

        public SelectList editAllDept(int departmentId)
        {
            return new SelectList(_context.Departments, "Id", "DepartmentName", departmentId);
        }

        public int addDepartment(string deptName) {
            if (_context.Departments.Any(n => n.DepartmentName.ToLower() == deptName.ToLower())) {
                return 0;
            }
            Department dept = new Department();
            dept.DepartmentName = deptName;
            _context.Departments.Add(dept);
            _context.SaveChanges();
            return dept.Id;
        }

        public IEnumerable<Employee> getAllEmployees() {
            var emps = _context.Employees.ToList();
            //List<EmployeeDetailViewModel> empVMs = new List<EmployeeDetailViewModel>();
            //foreach (var e in emps) {
            //    EmployeeDetailViewModel edVM = new EmployeeDetailViewModel();
            //    edVM.EmployeeId = e.Id;
            //    edVM.FirstName = e.FirstName;
            //    edVM.LastName = e.LastName;
            //    edVM.MiddleName = e.MiddleName;
            //    edVM.OfficeNumber = e.OfficeNumber;
            //    edVM.ExtensionNumber = e.ExtensionNumber;
            //    if (e.Status == 0)
            //    {
            //        edVM.StatusString = "Inactive";
            //    }
            //    else {
            //        edVM.StatusString = "Active";
            //    }
            //}
            return emps;
        }

        public List<ShowImageUrlViewModel> getAllEmployeesWithImage()
        {
            var emps = getAllEmployees();
           List<ShowImageUrlViewModel> sIUVMList = new List<ShowImageUrlViewModel>();
           foreach (var e in emps)
            {
                ShowImageUrlViewModel sIUM = new ShowImageUrlViewModel();
                sIUM.Employee = e;
                var personalDetails = _context.PersonalInfo.SingleOrDefault(p => p.EmployeeId == e.Id);
                if(personalDetails != null)
                    sIUM.ImageUrl = personalDetails.ImageUrl;
                sIUVMList.Add(sIUM);
                var reg = _context.Users.SingleOrDefault(em => em.Email == e.Email);
                if(reg == null)
                {
                    sIUM.IsRegistered = false;
                } else
                {
                    sIUM.IsRegistered = true;
                }
            }
            return sIUVMList;
        }

        

        public EmployeeDetailViewModel getEditDetailsViewModel(int? id) {
            var employee =  _context.Employees.SingleOrDefault(m => m.Id == id);
            var jobDetail =  _context.JobDetails.SingleOrDefault(m => m.EmployeeId == id);
            if (employee == null || jobDetail == null)
            {
                return null;
            }
            EmployeeDetailViewModel edVM = new EmployeeDetailViewModel();
            edVM.FirstName = employee.FirstName;
            edVM.MiddleName = employee.MiddleName;
            edVM.LastName = employee.LastName;
            edVM.Email = employee.Email;
            edVM.DepartmentId = employee.DepartmentId;
            edVM.Status = employee.Status;
            edVM.ExtensionNumber = employee.ExtensionNumber;
            edVM.OfficeNumber = employee.OfficeNumber;

            edVM.EmployeeId = jobDetail.EmployeeId;
            edVM.Title = jobDetail.Title;
            edVM.Level = jobDetail.Level;
            edVM.Supervisor = jobDetail.Supervisor;
            edVM.ExpectedJoinDate = jobDetail.ExpectedJoinDate;
            edVM.WorkSchedualId = jobDetail.WorkSchedualId;

            return edVM;
        }

        public EmployeeDetailViewModelCSV getEditDetailsViewModelCSV(int? id)
        {
            var employee = _context.Employees.SingleOrDefault(m => m.Id == id);
            var jobDetail = _context.JobDetails.SingleOrDefault(m => m.EmployeeId == id);
            if (employee == null || jobDetail == null)
            {
                return null;
            }
            EmployeeDetailViewModelCSV edVM = new EmployeeDetailViewModelCSV();
            edVM.FirstName = employee.FirstName;
            edVM.MiddleName = employee.MiddleName;
            edVM.LastName = employee.LastName;
            edVM.Email = employee.Email;
            var dName = (_context.Departments.SingleOrDefault(i => i.Id == employee.DepartmentId));
            if(dName != null)
            {
                edVM.DepartmentName = dName.DepartmentName;
            }
            if(employee.Status == 1)
            {
                edVM.Status = "Active";
            } else
            {
                edVM.Status = "Inactive";
            }
            
            edVM.ExtensionNumber = employee.ExtensionNumber;
            edVM.OfficeNumber = employee.OfficeNumber;

            edVM.EmployeeId = jobDetail.EmployeeId;
            edVM.Title = jobDetail.Title;
            edVM.Level = jobDetail.Level;
            edVM.Supervisor = jobDetail.Supervisor;
            edVM.ExpectedJoinDate = jobDetail.ExpectedJoinDate;
            var work = _context.WorkScheduals.SingleOrDefault(w => w.Id == jobDetail.WorkSchedualId);
            edVM.WorkDays = work.WorkDays;
            edVM.WorkHours = work.WorkHours;

            return edVM;
        }

        public Employee getEmployeeById(int? id)
        {
            return _context.Employees.SingleOrDefault(m => m.Id == id);
        }

        public EmployeeDetailViewModel getEditDetailsViewModel(int? id, EmployeeDetailViewModel edVM)
        {
            var employee = _context.Employees.SingleOrDefault(m => m.Id == id);
            var jobDetail = _context.JobDetails.SingleOrDefault(m => m.EmployeeId == id);
            if (employee == null || jobDetail == null)
            {
                return null;
            }

            employee.FirstName = edVM.FirstName;
            employee.MiddleName = edVM.MiddleName;
            employee.LastName = edVM.LastName;
            employee.Email = edVM.Email;
            employee.DepartmentId = edVM.DepartmentId;
            employee.Status = edVM.Status;
            employee.ExtensionNumber = edVM.ExtensionNumber;
            employee.OfficeNumber = edVM.OfficeNumber;

            jobDetail.EmployeeId = edVM.EmployeeId;
            jobDetail.Title = edVM.Title;
            jobDetail.Level = edVM.Level;
            jobDetail.Supervisor = edVM.Supervisor;
            jobDetail.ExpectedJoinDate = edVM.ExpectedJoinDate;
            jobDetail.WorkSchedualId = edVM.WorkSchedualId;
            _context.Update(employee);
            _context.Update(jobDetail);
            _context.SaveChanges();
            return edVM;
        }

        public Contract getEditContractView(int? id)
        {
           var contract = _context.Contracts.SingleOrDefault(m => m.EmployeeId == id);
           if (contract == null)
            {            
                    return null;
            }
            return contract;
        }

        public void getEditContractView(int? id, Contract contract)
        {
            var c = _context.Contracts.SingleOrDefault(m => m.EmployeeId == id);
            if (c == null)
            {
                if (id != null && isValidEmployee((int)id))
                {
                    addContract(contract);
                }
            } else
            {
                c.EmployeeId = contract.EmployeeId;
                c.ContractType = contract.ContractType;
                c.ContractPeriod = contract.ContractPeriod;
                c.JoinDate = contract.JoinDate;
                c.IsProbation = contract.IsProbation;
                c.ProbationPeriod = contract.ProbationPeriod;
                _context.Update(c);
                _context.SaveChanges();
            }
           
        }

        public PersonalInfo getEditPersonalInfoView(int? id)
        {
            var p = _context.PersonalInfo.SingleOrDefault(m => m.EmployeeId == id);

            if (p == null)
            {
                return null;
            }
            return p;
        }

        public void getEditPersonalInfoView(int? id, PersonalInfo personalInfo)
        {
            var p = _context.PersonalInfo.SingleOrDefault(m => m.EmployeeId == id);
            if(p == null)
            {
                if (id != null && isValidEmployee((int)id))
                {
                    addPersonalInfo(personalInfo);
                }          
            }
            else
            {
                p.District = personalInfo.District;
                p.EmployeeId = personalInfo.EmployeeId;
                p.Zone = personalInfo.Zone;
                p.Tole = personalInfo.Tole;
                p.IsMarried = personalInfo.IsMarried;
                p.SpouseName = personalInfo.SpouseName;
                _context.Update(p);
                _context.SaveChanges();
            }
        }

        public void getEditPersonalInfoView(int empId, string url)
        {
            var p = _context.PersonalInfo.SingleOrDefault(m => m.EmployeeId == empId);
            if(p != null)
            {
                p.ImageUrl = url;
                _context.Update(p);
                _context.SaveChanges();
            } else
            {
                PersonalInfo pInfo = new PersonalInfo();
                pInfo.EmployeeId = empId;
                pInfo.ImageUrl = url;
                _context.PersonalInfo.Add(pInfo);
                _context.SaveChanges();
            }
        }

        public void removeFileUrl(int empId)
        {
            var p = _context.PersonalInfo.SingleOrDefault(m => m.EmployeeId == empId);
            if (p != null)
            {
                p.ImageUrl = null;
                _context.Update(p);
                _context.SaveChanges();
            }
        }

        public void removeEmployee(int id)
        {
            var employee =  _context.Employees.SingleOrDefault(m => m.Id == id);
            if(employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
        }

        public void removeJobDetail(int id)
        {
            var job = _context.JobDetails.SingleOrDefault(j => j.EmployeeId == id);
            if (job != null)
            {
                _context.JobDetails.Remove(job);
                _context.SaveChanges();
            }
        }

        public void removeContract(int id)
        {
            var contract = _context.Contracts.SingleOrDefault(c => c.EmployeeId == id);
            if(contract != null)
            {
                _context.Contracts.Remove(contract);
                _context.SaveChanges();
            }
        }

        public void removePersonalInfo(int id)
        {
            var personalInfo = _context.PersonalInfo.SingleOrDefault(c => c.EmployeeId == id);
            if(personalInfo != null)
            {
                _context.PersonalInfo.Remove(personalInfo);
                _context.SaveChanges();
            }
        }

        public Employee getEmployeeDetailById(int id)
        {
            var employee = _context.Employees.SingleOrDefault(m => m.Id == id);
            if(employee != null)
            {
                return employee;
            }
            return null;
        }

        public JobDetail getJobDetailById(int id)
        {
            var job = _context.JobDetails.SingleOrDefault(j => j.EmployeeId == id);
            if (job != null)
            {
                return job;
            }
            return null;
        }

        public Contract getContractById(int id)
        {
            var contract = _context.Contracts.SingleOrDefault(c => c.EmployeeId == id);
            if (contract != null) {
                return contract;
            }
            return null;
        }

        public PersonalInfo getPersonalInfoById(int id)
        {
            var personalInfo = _context.PersonalInfo.SingleOrDefault(c => c.EmployeeId == id);
            if (personalInfo != null)
            {
                return personalInfo;
            }
            return null;
        }

        public int getDepartmentIdByName(string dName)
        {
            var d = _context.Departments.SingleOrDefault(dp => dp.DepartmentName.ToLower() == dName.ToLower());
            if(d != null)
            {
                return d.Id;
            }
            return 0;
        }

        public int getAllWorkSchedualID(string v1, string v2)
        {
            var ws = _context.WorkScheduals.SingleOrDefault(w => w.WorkDays.ToLower().Trim() == v1.ToLower().Trim() && w.WorkHours.ToLower().Trim() == v2.ToLower().Trim());
            if(ws != null)
            {
                return ws.Id;
            } else
            {
                WorkSchedual w = new WorkSchedual();
                w.WorkDays = v1;
                w.WorkHours = v2;
                _context.WorkScheduals.Add(w);
                _context.SaveChanges();
                return w.Id;
            }
        }
    }
}
