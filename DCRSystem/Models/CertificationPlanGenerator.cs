using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DCRSystem.Models;

namespace DCRSystem.Models
{
    public class CertificationPlanGenerator
    {
        lear_DailiesCertificationRequirementEntities ldcr = new lear_DailiesCertificationRequirementEntities();

       
        public String GetCertificationPlan(String Code )
        {
            if (GetCertificateType(Code).Equals("Yearly"))
            {
                DateTime now =Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));
                now = now.AddYears(1);
                return now.ToShortDateString().ToString();
            }else
            if (GetCertificateType(Code).Equals("Semi-Annual"))
            {
                 DateTime now = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));
               now = now.AddMonths(6);
                return now.ToShortDateString().ToString();
            }else
            if (GetCertificateType(Code).Equals("Quarterly"))
            {
                DateTime now = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));
               now = now.AddMonths(3);
                return now.ToShortDateString().ToString();
            }
            else
            {
                return "";
            }
           
        }

        public String GetCertificationPlanA(String Code ,CertificationTracker cert)
        {
            if (GetCertificateType(Code).Equals("Yearly"))
            {
                DateTime now = Convert.ToDateTime(cert.DateCertified);
                now =  now.AddYears(1);
                return now.ToShortDateString().ToString();
            }
            else
            if (GetCertificateType(Code).Equals("Semi-Annual"))
            {
                DateTime now = Convert.ToDateTime(cert.DateCertified);
                now.AddMonths(6);
                return now.ToShortDateString().ToString();
            }
            else
            if (GetCertificateType(Code).Equals("Quarterly"))
            {
                DateTime now = Convert.ToDateTime(cert.DateCertified);
                now.AddMonths(3);
                return now.ToShortDateString().ToString();
            }
            else
            {
                return "";
            }


        }

        public Certification GetCertification(String Code)
        {
            return ldcr.Certifications.Where(cert => cert.Code == Code).FirstOrDefault();
        }

        public String GetCertificateType(String Code)
        {
            return ldcr.Certifications.Where(cert => cert.Code == Code).FirstOrDefault().Type;
        }
    }
}