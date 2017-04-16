using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models.EmployeeModels
{
    public class PersonalInfo
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [ScaffoldColumn(false)]
        public int EmployeeId { get; set; }

        //[Required(ErrorMessage = "Zone is Required")]
        public string Zone { get; set; }

        //[Required(ErrorMessage = "District is Required")]
        public string District { get; set; }

        //[Required(ErrorMessage = "Tole is Required")]
        public string Tole { get; set; }

        public string ImageUrl { get; set; }

        public bool IsMarried { get; set; }

        public string SpouseName { get; set; }
    }
}
