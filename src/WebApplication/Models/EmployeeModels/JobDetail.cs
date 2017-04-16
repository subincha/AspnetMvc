using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models.EmployeeModels
{
    public class JobDetail
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [ScaffoldColumn(false)]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Job Title is Required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Job Level is Required")]
        public string Level { get; set; }

        public string Supervisor { get; set; }

        [Required(ErrorMessage = "Expected Joining Date is Required")]
        [DataType(DataType.Date)]
        public DateTime ExpectedJoinDate { get; set; }

        [Required(ErrorMessage = "Expected Work Schedual is Required")]
        public int WorkSchedualId { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual WorkSchedual WorkSchedual { get; set; }
    }
}