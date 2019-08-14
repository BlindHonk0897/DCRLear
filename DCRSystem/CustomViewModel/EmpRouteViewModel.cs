using DCRSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCRSystem.CustomViewModel
{
    public class EmpRouteViewModel
    {
        public int Id { get; set; }
        public IEnumerable<Emp_Route> emp_Routes { get; set; }

    }
}