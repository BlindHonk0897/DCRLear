using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCRSystem.DataModel
{
    public class EmployeeCertificates
    {
        public int Id { get; set; }
        public String CertificateCode { get; set; }
        public String Description { get; set; }
        public String Type { get; set; }
        public String DateCertified { get; set; }
        public String DateRecertified { get; set; }
        public String CertificationPlan { get; set; }
        public String Status { get; set; }
        public String Identifier { get; set; }
    }
}