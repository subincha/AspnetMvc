using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models.EmployeeModels
{
    public class Employee
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "FirstName is Require")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is Required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Department Name is Required")]
        [Display(Name = "Department Name")]
        public int DepartmentId { get; set; }

        [Display(Name = "Office Number")]
        public string OfficeNumber { get; set; }

        [Display(Name = "Extension Number")]
        public string ExtensionNumber { get; set; }

        [Required(ErrorMessage = "Status is Required")]
        public int Status { get; set; }

        public virtual Department Department { get; set; }
    }
}