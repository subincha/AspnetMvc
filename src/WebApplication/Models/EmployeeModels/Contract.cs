using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models.EmployeeModels
{ 
    public class Contract
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [ScaffoldColumn(false)]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Contract type is required")]
        [Display(Name = "Contract Type")]
        public string ContractType { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Join date is required")]
        [Display(Name = "Join date")]
        public DateTime JoinDate { get; set; }

        [Display(Name = "Contract Period")]
        public int ContractPeriod { get; set; }

        [Required(ErrorMessage = "IsProbation is required")]
        public bool IsProbation { get; set; }

        [Display(Name = "Probation Period")]
        public int ProbationPeriod { get; set; }

        public virtual Employee Employee { get; set; }
    }
}