using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DCRSystem.Models;

namespace DCRSystem.CustomViewModel
{
    public class CertificateViewModel
    {
        public int Id { get; set; }
        public List<Certification> Certifications { get; set; }
    }
}