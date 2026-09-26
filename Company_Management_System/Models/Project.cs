using System;
using System.Collections.Generic;
using System.Text;

namespace Company_Management_System.Models
{
    public partial class Project
    {
        public int ProjectId { get; set; }

        public string Name { get; set; } = null!;

        public DateOnly StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
