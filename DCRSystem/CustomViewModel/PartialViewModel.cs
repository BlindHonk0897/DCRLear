using DCRSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCRSystem.CustomViewModel
{
    public class PartialViewModel
    {
        public int Id { get; set; }
        public List<EmployeeDCR_Vw> EmployeeDCR_Vws = new List<EmployeeDCR_Vw>();
    }
}