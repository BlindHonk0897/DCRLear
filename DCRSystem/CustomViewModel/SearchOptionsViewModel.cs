using DCRSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCRSystem.CustomViewModel
{
    public class SearchOptionsViewModel
    {
        public int Id { get; set; }
        public List<Certification> Certifications = new List<Certification>();
        public List<string> allIds = new List<string>();
        public List<string> allCells = new List<string>();
        public List<Medal> allMedals = new List<Medal>();
        public List<Filter> filters = new List<Filter>();
        public List<string> Choices = new List<string>();
        public string KeyName { get; set; }

    }
}