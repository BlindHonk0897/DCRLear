using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCRSystem.Models
{
   public static class DateConfig
    {
        public static lear_DailiesCertificationRequirementEntities ldcr = new lear_DailiesCertificationRequirementEntities();

        public static DateTime AddOneYear(DateTime dateTime)
        {
            return dateTime.AddYears(1);
        }

        public static CertificationTracker getLastCertified(String id)
        {
            return ldcr.CertificationTrackers.Where(ctr => ctr.EmpBadgeNo == id).ToList().FirstOrDefault();
        }
        public static CertificationTracker getLastReCertified(String id)
        {
            return ldcr.CertificationTrackers.Where(ctr => ctr.EmpBadgeNo == id && ctr.DateRecertified!= null).ToList().FirstOrDefault();
        }
    }
}