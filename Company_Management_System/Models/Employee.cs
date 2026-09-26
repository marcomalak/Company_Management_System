using System;
using System.Collections.Generic;
using System.Text;

namespace Company_Management_System.Models
{
    public partial class Employee
    {
        public int EmployeeId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public int? DepartmentId { get; set; }

        public virtual Department? Department { get; set; }

        public List<Project> Projects { get; set; } = new List<Project>();
    }
}
