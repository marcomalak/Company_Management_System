using System;
using System.Collections.Generic;
using System.Text;

namespace Company_Management_System.Models
{
    public partial class Department
    {
        public int DepartmentId { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
