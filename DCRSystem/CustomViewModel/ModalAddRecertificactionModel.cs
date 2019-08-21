using DCRSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCRSystem.CustomViewModel
{
    public class ModalAddRecertificactionModel
    {
        public int Id { get; set; }
        public String BadgeNo { get; set; }
        public IEnumerable<CertificationTracker> CertificationTrackers { get; set; }
    }
}