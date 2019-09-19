using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DCRSystem.DataModel;

namespace DCRSystem.DataModel
{
    public class EmployeeProgressDetails
    {
        public int id { get; set; }
        public String EmpBadgeNo { get; set; }
        public String FullName { get; set; }
        public String Position { get; set; }
        public String HrcCell { get; set; }
        public String CurrentCell { get; set; }
        public String HrcSupervisor { get; set; }
        public String CurrentSupervisor { get; set; }
        public String SkillsCertified { get; set; }
        public String SkillsCertifiedPercentage { get; set; }
        public String SkillsCertifiedPoints { get; set; }
        public String SkillsReCertified { get; set; }
        public String SkillsReCertifiedPercentage { get; set; }
        public String SkillsReCertifiedPoints { get; set; }
        public List<EmployeeCertificates> EmployeeCertificates = new List<EmployeeCertificates>();
        // --for print purposes only --//
        public String LogoImagePath { get; set; }
        public String Medal { get; set; }

    }
}