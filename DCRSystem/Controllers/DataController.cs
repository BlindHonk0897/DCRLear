using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using DCRSystem.Models;
using DCRSystem.DataModel;
using DCRSystem.CustomViewModel;

namespace DCRSystem.Controllers
{
    public class DataController : Controller
    {
        // GET: Data
        lear_DailiesCertificationRequirementEntities entities = new lear_DailiesCertificationRequirementEntities();
        public FileResult ExportEmployeeDetailsToExcel( String id )
        {
            var employeeDetails = entities.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id).FirstOrDefault();
            var employeeProgess = GetEmployeeDetails(id);
            if (employeeProgess.FullName != null && employeeDetails != null)
            {
                DataTable dt = new DataTable("Employee Details");

                dt.Columns.AddRange(new DataColumn[7] { new DataColumn("Employee ID"),
                                            new DataColumn("Employee Name"),
                                            new DataColumn("Position"),
                                            new DataColumn("HRC Cell"),
                                            new DataColumn("Current Cell"),
                                            new DataColumn("HRC Supervisor"),
                                            new DataColumn("Current Supervisor"),
                                           });


                //var certificates = from Certification in entities.Certifications
                //                   select Certification;

                dt.Rows.Add(employeeDetails.Employee_ID,employeeDetails.Last_Name +", "+employeeDetails.First_Name,employeeDetails.Position,employeeDetails.HRCCell
                            ,employeeDetails.CurrentCell,employeeDetails.HRCSupervisor,employeeDetails.CurrentSupervisor);

                //DataTable progress = new DataTable("Grid1");
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Certificate Code"),
                                            new DataColumn("Description"),
                                            new DataColumn("Type"),
                                            new DataColumn("Date Certified"),
                                            new DataColumn("Date Recertified"),
                                           new DataColumn("Certification Plan"),
                                            new DataColumn("Status"),
                                            new DataColumn("Identifier")});
                //dt.Rows.Add("", "", "", "", "", "", "", "","","");
                //dt.Rows.Add("", "Employee's Certificates :", "", "", "", "", "", "", "", "");

                foreach (var certificate in employeeProgess.EmployeeCertificates)
                {
                    dt.Rows.Add("", "", "", "", "", "", "", certificate.CertificateCode, certificate.Description, certificate.Type, certificate.DateCertified,certificate.DateRecertified,
                        certificate.CertificationPlan,certificate.Status,certificate.Identifier);
                }

                for (int i = 0; i < 2; i++)
                {
                    dt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "");

                }
                dt.Rows.Add("", "Skills Certified :", employeeProgess.SkillsCertified, "", "", "", "", "");
                dt.Rows.Add("", "Percentage :", employeeProgess.SkillsCertifiedPercentage, "", "", "", "", "");
                dt.Rows.Add("", "Points :", employeeProgess.SkillsCertifiedPoints, "", "", "", "", "");
                dt.Rows.Add("", "", "", "", "", "", "", "");
                dt.Rows.Add("", "", "", "", "", "", "", "");
                dt.Rows.Add("", "Skills Re-Certified :", employeeProgess.SkillsReCertified, "", "", "", "", "");
                dt.Rows.Add("", "Percentage :", employeeProgess.SkillsReCertifiedPercentage, "", "", "", "", "");
                dt.Rows.Add("", "Points :", employeeProgess.SkillsReCertifiedPoints, "", "", "", "", "");
                dt.Rows.Add("", "", "", "", "", "", "", "");
                dt.Rows.Add("", "", "", "", "", "", "", "");
                dt.Rows.Add("Details Of :", employeeProgess.EmpBadgeNo + " - " + employeeProgess.FullName, "", "", "", "", "Date Exported :", DateTime.Now.ToString());

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    //wb.Worksheets.Add(progress);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", employeeProgess.FullName+"'s-Details.xlsx");
                    }
                }
            }
            else
            {
                return null;
            }

           
        }

        public EmployeeProgressDetails GetEmployeeDetails(String id)
        {
            EmployeeProgressDetails empP = new EmployeeProgressDetails();
            var Certificates = entities.Certifications.ToList();
            var CertificationTracker = entities.certificateTracker_Vw.ToList();
            List<CertificationTracker> MyCertificates = new List<CertificationTracker>();
            var NumberOfCertified = 0;
            var NumberOfRecertified = 0;
            int TotalCertifiedPoints = 0;
            int TotalReCertifiedPoints = 0;

            CertificationPlanGenerator generator = new CertificationPlanGenerator();

            if (id != null)
            {
                var employee = entities.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id).FirstOrDefault();

                if (employee != null)
                {
                    empP.EmpBadgeNo = employee.Employee_ID;
                    empP.FullName = employee.Last_Name + " , " + employee.First_Name;
                    empP.SkillsCertified = entities.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID).OrderBy(cr => cr.CertificationCode).ToList().Count() + " out of " + Certificates;

                    NumberOfCertified = entities.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID).OrderBy(cr => cr.CertificationCode).ToList().Count();
                    empP.SkillsCertified = NumberOfCertified + " out of " + Certificates.Count();

                    NumberOfRecertified = entities.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID && cr.DateRecertified != null).OrderBy(cr => cr.Id).ToList().Count();
                    empP.SkillsReCertified = NumberOfRecertified + " out of " + NumberOfCertified;

                    double SKP = (Convert.ToDouble(NumberOfCertified) / Convert.ToDouble(Certificates.Count())) * (100);
                    empP.SkillsCertifiedPercentage = !Double.IsNaN(SKP) ? Convert.ToInt32(SKP).ToString() + "%" : "0%";

                    double SRKP = (Convert.ToDouble(NumberOfRecertified) / Convert.ToDouble(Certificates.Count())) * (100);
                    empP.SkillsReCertifiedPercentage = !Double.IsNaN(SRKP) ? Convert.ToInt32(SRKP).ToString() + "%" : "0%";

                    MyCertificates = entities.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID).OrderBy(cr => cr.CertificationCode).ToList();

                    foreach (var certif in Certificates)
                    {
                        var existCert = MyCertificates.Where(cert => cert.CertificationCode == certif.Code).FirstOrDefault();
                        if (existCert != null)
                        {
                            TotalCertifiedPoints = TotalCertifiedPoints + Convert.ToInt32(certif.Points);
                            if (existCert.DateRecertified != null)
                            {
                                TotalReCertifiedPoints = TotalReCertifiedPoints + Convert.ToInt32(certif.Points);
                            }
                            empP.EmployeeCertificates.Add(
                                new EmployeeCertificates()
                                {
                                    CertificateCode = certif.Code,
                                    Description = certif.Description,
                                    Type = certif.Type,
                                    DateCertified = Convert.ToDateTime(existCert.DateCertified).ToShortDateString(),
                                    DateRecertified = existCert.DateRecertified == null ? "Not yet Recertified" : Convert.ToDateTime(existCert.DateRecertified).ToShortDateString(),
                                    CertificationPlan = generator.GetCertificationPlanA(certif.Code, existCert).ToShortDateString(),
                                    Status =  generator.DateNowAddMonth(-2) <= generator.GetCertificationPlanA(certif.Code, existCert) ?"Active" : "In-Active",
                                    Identifier = GenerateIdentifier(certif.Code,existCert)
                                }

                                );
                            
                        }
                    }
                    empP.SkillsCertifiedPoints = TotalCertifiedPoints.ToString();
                    empP.SkillsReCertifiedPoints = TotalReCertifiedPoints.ToString();


                }
                
            }
            return empP;

        }

        public String GenerateIdentifier(String code ,CertificationTracker tracker)
        {
            CertificationPlanGenerator generator = new CertificationPlanGenerator();
            var DiffMonth = generator.DifferenceMonth(generator.GetCertificationPlanA(code, tracker));
            if (DiffMonth > 0)
            {
                if(DiffMonth <= 31)
                {
                    return "One Month Overdue";
                }else if(DiffMonth > 31 && DiffMonth < 63)
                {
                    return "Almost Two Months Overdue";
                }
                else
                {
                    return "Two Months Overdue";
                }
            }
            else
            {
                return "In due time";
            }
               
        }

        public ActionResult ModalDetailsPrintable(String id ,String urlBack, String redirectUrl)
        {
            ViewBag.id = id;
            if (id != null) { ViewBag.id = id; }
            UrlModel mode = new UrlModel();
            if (id != null) { mode.EmpId = id; }
            mode.URLBack = urlBack;
            mode.RedirectUrl = redirectUrl;
            return View(mode);
        }

    }
}