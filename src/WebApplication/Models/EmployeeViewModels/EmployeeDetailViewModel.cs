﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models.EmployeeModels;

namespace WebApplication.Models.EmployeeViewModels
{
    public class EmployeeDetailViewModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Firstname is Required")]
        [Display(Name = "Firstname")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is Required")]
        [Display(Name = "Lastname")]
        public string LastName { get; set; }

        [Display(Name = "Middlename")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Department Name is Required")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Office number")]
        public string OfficeNumber { get; set; }

        [Display(Name = "Extension number")]
        public string ExtensionNumber { get; set; }

        [Required(ErrorMessage = "Status is Required")]
        public int Status { get; set; }

        [Required(ErrorMessage = "Job Title is Required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Job Level is Required")]
        public string Level { get; set; }

        public string Supervisor { get; set; }

        [Required(ErrorMessage = "Expected Joining Date is Required")]
        [DataType(DataType.Date)]
        [Display(Name = "Expected Joining Date")]
        public DateTime ExpectedJoinDate { get; set; }

        [Display(Name = "Expected Work Schedual")]
        [Required(ErrorMessage = "Expected Work Schedual is Required")]
        public int WorkSchedualId { get; set; }

        public string StatusString { get; set; }
        public string DepartmentName { get; set; }

    }
}
