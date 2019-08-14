using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DCRSystem.Models;

namespace DCRSystem.CustomViewModel
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public Employees_Details Employee { get; set; }
        public IEnumerable<CertificationTracker> AvailableReCertifications { get; set; }
        public IEnumerable<Certification> Certifications { get; set; }
        public IEnumerable<CertificationTracker> TotalCertifications { get; set; }
        public List<CertificationTracker> MyCertifications = new List<CertificationTracker>();
        public List<Certification> ImNotCertified = new List<Certification>();
    }
}