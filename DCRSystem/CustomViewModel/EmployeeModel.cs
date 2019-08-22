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
        public EmployeeDCR_Vw Employee { get; set; }
        public IEnumerable<CertificationTracker> AvailableReCertifications { get; set; }
        public IEnumerable<Certification> Certifications { get; set; }
        public IEnumerable<CertificationTracker> TotalCertifications { get; set; }
        public List<CertificationTracker> MyCertifications = new List<CertificationTracker>();
        public List<Certification> ImNotCertified = new List<Certification>();
        public CertificationTracker CurrentCertification { get; set; }
        public int PercentAgeCertified { get; set; }
        public int PercentAgeReCertified { get; set; }
        public int TotalPointsReCertified { get; set; }
        public int TotalPointsCertified { get; set; }
        public int NumberReCertified { get; set; }
    }
}