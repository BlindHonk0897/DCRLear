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


        public String GetCertificationPlan(String Code)
        {
            if (GetCertificateType(Code).Equals("Yearly"))
            {
                DateTime now = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));
                now = now.AddYears(1);
                return now.ToShortDateString().ToString();
            }
            else
            if (GetCertificateType(Code).Equals("Semi-Annual"))
            {
                DateTime now = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));
                now = now.AddMonths(6);
                return now.ToShortDateString().ToString();
            }
            else
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

        public DateTime GetCertificationPlanA(String Code, CertificationTracker cert)
        {
            if (GetCertificateType(Code).Equals("Yearly"))
            {
                DateTime now = Convert.ToDateTime(cert.DateCertified);
                now = now.AddYears(1);
                return now;
            }
            else
            if (GetCertificateType(Code).Equals("Semi-Annual"))
            {
                DateTime now = Convert.ToDateTime(cert.DateCertified);
                now = now.AddMonths(6);
                return now;
            }
            else
            if (GetCertificateType(Code).Equals("Quarterly"))
            {
                DateTime now = Convert.ToDateTime(cert.DateCertified);
                now = now.AddMonths(3);
                return now;
            }
            else
            {
                DateTime now = new DateTime();
                return now;
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

        public DateTime DateNowAddYear(int Year)
        {
            DateTime now = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));
            return now.AddYears(Year);
        }

        public DateTime DateNowAddMonth(int Month)
        {
            DateTime now = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));
            return now.AddMonths(Month);
        }

        public int DifferenceMonth(DateTime endDate)
        {
            DateTime startDate = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return monthsApart;
        }

        //public int GetMonthDifference(DateTime endDate)
        //{
        //    int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
        //    return Math.Abs(monthsApart);
        //}
    }
}