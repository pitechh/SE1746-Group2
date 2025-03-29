using Business.Data;
using Business.Extensions.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Resources.Person
{
    public class NewPersonResourse
    {
        [Required]
        [MaxLength(500)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(500)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [EmailAddress]
        [MaxLength(500)]
        public string Email { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Phone]
        [MaxLength(25)]
        public string Phone { get; set; }

        [DoB]
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Gender]
        public eGender Gender { get; set; }

        [Required]
        [Display(Name = "Position Id")]
        public int PositionId { get; set; }

        [Required]
        [Display(Name = "Department Id")]
        public int DepartmentId { get; set; }

        [Display(Name = "Group Id")]
        public int? GroupId { get; set; }

        [MaxLength(50)]
        [ComponentOrder]
        [Display(Name = "Order Index")]
        public List<int> OrderIndex { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
    }
}
