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
        public List<Filter> Filters = new List<Filter>();
        public Filter Filter { get; set; }
        public List<string> Choices = new List<string>();
        public string KeyName { get; set; }
    }
}