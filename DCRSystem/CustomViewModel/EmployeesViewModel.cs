using DCRSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCRSystem.CustomViewModel
{
    public class EmployeesViewModel
    {
        public int Id { get; set; }
        public IEnumerable<Employees_Details> Employees { get; set; }

    }
}