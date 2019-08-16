using DCRSystem.CustomViewModel;
using DCRSystem.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCRSystem.Controllers
{
    [Authorize(Roles = "Approver")]
    public class ApproverController : Controller
    {
        public commonEmployeesEntities cEE = new commonEmployeesEntities();
        public lear_DailiesCertificationRequirementEntities ldcr = new lear_DailiesCertificationRequirementEntities();

        [HttpGet]
        public ActionResult Employees(String id,int? page, String searchInput = "")
        {
            List<Emp_Route> underEmployees = cEE.Emp_Route.Where(emp => emp.Checker_1_ID == id).ToList<Emp_Route>();

            if (!string.IsNullOrEmpty(searchInput))
            {
                underEmployees = underEmployees.Where(a => a.Employee_Name.ToLower().Contains(searchInput.ToLower()) || a.Employee_ID.Contains(searchInput)).ToList();
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);


            return View(underEmployees.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult MyCertificate(String id)
        {

            EmployeeModel empModel = new EmployeeModel();
            if (id != null)
            {
                var employee = cEE.Employees_Details.Where(emp => emp.Employee_ID == id).FirstOrDefault();
                if (employee != null)
                {
                    empModel.Employee = employee;
                    empModel.Certifications = ldcr.Certifications.OrderBy(l => l.Code).ToList();
                    empModel.TotalCertifications = ldcr.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID).OrderBy(cr => cr.CertificationCode).ToList();
                    empModel.CurrentCertification = ldcr.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID).OrderByDescending(cr => cr.DateCertified).FirstOrDefault();
                    double SKP = (Convert.ToDouble(empModel.TotalCertifications.Count()) / Convert.ToDouble(empModel.Certifications.Count() + 2)) * (100);
                    if (!Double.IsNaN(SKP))
                    {
                        ViewBag.SkillsCertifiedPercentage = Convert.ToInt32(SKP);
                    }
                    else
                    {
                        ViewBag.SkillsCertifiedPercentage = 0;
                    }
                    var ReCertified = ldcr.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID && cr.DateRecertified != null).OrderBy(cr => cr.Id).ToList().Count();
                    ViewBag.TotalReCertified = ReCertified;
                    double SRKP = (Convert.ToDouble(ReCertified) / Convert.ToDouble(empModel.Certifications.Count() + 2)) * (100);
                    if (!Double.IsNaN(SRKP))
                    {
                        ViewBag.SkillsReCertifiedPercentage = Convert.ToInt32(SRKP);
                    }
                    else
                    {
                        ViewBag.SkillsReCertifiedPercentage = 0;
                    }

                    System.Diagnostics.Debug.WriteLine(Convert.ToInt32(SKP));
                    System.Diagnostics.Debug.WriteLine(Convert.ToInt32(SRKP));

                    foreach (var certification in empModel.Certifications)
                    {
                        if (empModel.TotalCertifications.FirstOrDefault(el => el.CertificationCode == certification.Code) == null)
                        {
                            CertificationTracker CT = new CertificationTracker();
                            CT.Id = 0;
                            CT.EmpBadgeNo = id;
                            CT.CertificationCode = certification.Code;
                            CT.DateCertified = null;
                            CT.DateCertified = null;
                            empModel.MyCertifications.Add(CT);
                        }
                        else
                        {
                            empModel.MyCertifications.Add(empModel.TotalCertifications.FirstOrDefault(el => el.CertificationCode == certification.Code));
                        }
                    }
                    System.Diagnostics.Debug.WriteLine(empModel.MyCertifications.Count());
                }
            }
            return View(empModel);

        }
        public ActionResult Details (String id ,String urlBack)
        {
            ViewBag.URLBack = urlBack;
            EmployeeModel empModel = new EmployeeModel();
            if (id != null)
            {
                var employee = cEE.Employees_Details.Where(emp => emp.Employee_ID == id).FirstOrDefault();
                if (employee != null)
                {
                    empModel.Employee = employee;
                    empModel.Certifications = ldcr.Certifications.OrderBy(l => l.Code).ToList();
                    empModel.TotalCertifications = ldcr.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID).OrderBy(cr => cr.CertificationCode).ToList();
                    empModel.CurrentCertification = ldcr.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID).OrderByDescending(cr => cr.DateCertified).FirstOrDefault();
                    double SKP = (Convert.ToDouble(empModel.TotalCertifications.Count()) / Convert.ToDouble(empModel.Certifications.Count() + 2)) * (100);
                    if (!Double.IsNaN(SKP))
                    {
                        ViewBag.SkillsCertifiedPercentage = Convert.ToInt32(SKP);
                    }
                    else
                    {
                        ViewBag.SkillsCertifiedPercentage = 0;
                    }
                    var ReCertified = ldcr.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID && cr.DateRecertified != null).OrderBy(cr => cr.Id).ToList().Count();
                    ViewBag.TotalReCertified = ReCertified;
                    double SRKP = (Convert.ToDouble(ReCertified) / Convert.ToDouble(empModel.Certifications.Count() + 2)) * (100);
                    if (!Double.IsNaN(SRKP))
                    {
                        ViewBag.SkillsReCertifiedPercentage = Convert.ToInt32(SRKP);
                    }
                    else
                    {
                        ViewBag.SkillsReCertifiedPercentage = 0;
                    }

                    System.Diagnostics.Debug.WriteLine(Convert.ToInt32(SKP));
                    System.Diagnostics.Debug.WriteLine(Convert.ToInt32(SRKP));

                    foreach (var certification in empModel.Certifications)
                    {
                        if (empModel.TotalCertifications.FirstOrDefault(el => el.CertificationCode == certification.Code) == null)
                        {
                            CertificationTracker CT = new CertificationTracker();
                            CT.Id = 0;
                            CT.EmpBadgeNo = id;
                            CT.CertificationCode = certification.Code;
                            CT.DateCertified = null;
                            CT.DateCertified = null;
                            empModel.MyCertifications.Add(CT);
                        }
                        else
                        {
                            empModel.MyCertifications.Add(empModel.TotalCertifications.FirstOrDefault(el => el.CertificationCode == certification.Code));
                        }
                    }
                }
            }
            return View(empModel);
        }
    }
}
