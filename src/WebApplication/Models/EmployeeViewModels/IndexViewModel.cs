using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models.EmployeeModels;

namespace WebApplication.Models.EmployeeViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Employee> Employee { get; set; }
        public List<Department> Department { get; set; }
    }
}
