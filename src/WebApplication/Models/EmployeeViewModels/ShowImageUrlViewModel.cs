using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models.EmployeeModels;

namespace WebApplication.Models.EmployeeViewModels
{
    public class ShowImageUrlViewModel
    {
        public Employee Employee { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        public bool IsRegistered { get; set; }
    }
}
