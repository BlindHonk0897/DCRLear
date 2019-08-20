using DCRSystem.CustomViewModel;
using DCRSystem.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCRSystem.Controllers
{
    [Authorize(Roles ="IT")] // Only IT Authorized can access this Controller
    public class ITController : Controller
    {
        public commonEmployeesEntities cEE = new commonEmployeesEntities();
        public lear_DailiesCertificationRequirementEntities ldcr = new lear_DailiesCertificationRequirementEntities();
       
        // GET: IT
        public ActionResult Employee(int? page, String searchInput = "")
        {
            // Get all Employees from Database
            List<Employees_Details> employees = cEE.Employees_Details.OrderBy(a => a.Last_Name).ToList();

            if (!string.IsNullOrEmpty(searchInput))
            {
                // get employee/employees with the same lastname with the input
                employees = employees.Where(a => a.Last_Name.ToLower().Contains(searchInput.ToLower()) || a.Employee_ID.Contains(searchInput)).ToList();
            }
           
            int pageSize = 10; // pagelist number of page
            int pageNumber = (page ?? 1);


            return View(employees.ToPagedList(pageNumber, pageSize));
            //return View();
        }

        [HttpGet]
        public ActionResult Certified(String id,String urlBack)
        {
            ViewBag.URLBack = urlBack;// Backing Purposes

            EmployeeModel empModel = new EmployeeModel();
            if (id != null)
            {
                // Get employee using the id
                var employee = cEE.Employees_Details.Where(emp => emp.Employee_ID == id).FirstOrDefault();

                if (employee != null) // if exist
                {
                    empModel.Employee = employee;
                }
            }

            // Get the Total Certifications
            empModel.TotalCertifications = ldcr.CertificationTrackers.Where(ct => ct.EmpBadgeNo == id).ToList();

            // Get All Certifications
            empModel.Certifications = ldcr.Certifications.OrderBy(ct => ct.Code).ToList<Certification>();

            foreach(var certTrack in empModel.Certifications)
            {
                if(empModel.TotalCertifications.FirstOrDefault(tc => tc.CertificationCode == certTrack.Code) == null)
                {
                    // Add Certifications which employee is not Certified
                    empModel.ImNotCertified.Add(certTrack);
                }
            }
            return View(empModel);
        }

        [HttpPost]
        public ActionResult PostCertified()
        {
            // [ GET DATAS from submitted form --
            var DateP = Request.Form["DateCertified"];
            var Who = Request.Form["Employee"];
            var Code = Request.Form["Code"];
            var RedirectURL = Request.Form["URLBACK"];
            // ----------------------
            var message = "";// initialize variable for message
            List<String> error = new List<string>();// initialize variable for error

            CertificationTracker certificationTracker = new CertificationTracker();

            // Validate DATA submitted
            if (DateP != null && Who != null && Code != null && Code!="X" && !String.IsNullOrEmpty(DateP))
            {
                // Check if Employee is Exist
                var emp = cEE.Employees_Details.Where(u => u.Employee_ID.ToString().ToLower() == Who.ToString().ToLower()).FirstOrDefault();
                if (emp != null)
                {
                    certificationTracker.EmpBadgeNo = emp.Employee_ID;
                }
                else
                {
                    error.Add("BadgeNo not exist");
                }
                // Check if certification Code is Exist
                var code = ldcr.Certifications.Where(c => c.Code == Code).FirstOrDefault();
                if (code != null)
                {
                    certificationTracker.CertificationCode = code.Code;
                }
                else
                {
                    error.Add("Certification Code not exist");
                }

                if (Validator.isValidDate(DateP))
                {
                    certificationTracker.DateCertified = DateTime.ParseExact(DateP, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    error.Add("Date is not Valid!");
                }
                if (error.Count > 0)
                {
                    foreach (var da in error)
                    {
                        System.Diagnostics.Debug.WriteLine(da);
                    }
                }
                else
                {
                    var TempCertificationTracker = ldcr.CertificationTrackers.Where(ct => ct.EmpBadgeNo ==
                    certificationTracker.EmpBadgeNo && ct.CertificationCode == certificationTracker.CertificationCode).FirstOrDefault();
                    
                    if(TempCertificationTracker == null)
                    {
                        // Add the new Tracker
                        ldcr.CertificationTrackers.Add(certificationTracker);
                        ldcr.SaveChanges();
                        message = emp.Last_Name + " , " + emp.First_Name + " is successfully certified in " + certificationTracker.CertificationCode;
                        return RedirectToAction("ModalSuccess", "IT", new { message = message, id = Who, urlBack = "Details",redirectUrl = RedirectURL });
                    }
                }
                return RedirectToAction("ModalFailed", "IT", new { message = message, id = Who, urlBack = "Certified", redirectUrl = RedirectURL });
            }
            else
            {
               
                if (DateP == null || String.IsNullOrEmpty(DateP))
                {
                    ModelState.AddModelError("", "Please provide date certified.");
                    message += "Please provide date certified.";
                }
                if (Code == "X")
                {
                    ModelState.AddModelError("", "Please choose certification code");
                    message = "Please choose certification code";
                }

                return RedirectToAction("ModalFailed", "IT", new { message = message, id = Who, urlBack = "Certified", redirectUrl = RedirectURL });
            }
        }

        [HttpGet]
        public ActionResult ModalMessage(String message , int? id)
        {
            ViewBag.message = message;
            
            return View();
        }


        [HttpGet]
        public ActionResult ReCertified(String id, String urlBack)
        {
            ViewBag.URLBack = urlBack;
            EmployeeModel empModel = new EmployeeModel();
            if (id != null)
            {
                var employee = cEE.Employees_Details.Where(emp => emp.Employee_ID == id).FirstOrDefault();
                if (employee != null)
                {
                    empModel.Employee = employee;
                     empModel.AvailableReCertifications = ldcr.CertificationTrackers.Where(l => l.EmpBadgeNo == employee.Employee_ID && l.DateRecertified==null);
                    //ViewBag.ReCertifications = ldcr.CertificationTrackers.Where(l => l.EmpBadgeNo == employee.Employee_ID);
                    return View(empModel);
                }
            }
            return Content("Not Found Employee");
        }

        [HttpPost]
        public ActionResult PostReCertified()
        {
            var DateP = Request.Form["DateReCertified"];
            var Who = Request.Form["Employee"];
            var Code = Request.Form["Code"];
            var RedirectURL = Request.Form["URLBACK"];
            var message = "";
           
            List<String> error = new List<string>();
            CertificationTracker certificationTracker = new CertificationTracker();
            if (DateP != null && Who != null && Code != null && Code != "X" && !String.IsNullOrEmpty(DateP))
            {
                // Check if Employee is Exist
                var emp = cEE.Employees_Details.Where(u => u.Employee_ID.ToString().ToLower() == Who.ToString().ToLower()).FirstOrDefault();
                if (emp != null)
                {
                    certificationTracker.EmpBadgeNo = emp.Employee_ID;
                }
                else
                {
                    error.Add("BadgeNo not exist");
                }
                // Check if certification Code is Exist
                var code = ldcr.Certifications.Where(c => c.Code == Code).FirstOrDefault();
                if (code != null)
                {
                    certificationTracker.CertificationCode = code.Code;
                }
                else
                {
                    error.Add("Certification Code not exist");
                }

                if (Validator.isValidDate(DateP))
                {
                   // CertificationTracker ctr = new CertificationTracker();
                  var  ctr = ldcr.CertificationTrackers.Where(cTr => cTr.EmpBadgeNo == Who && cTr.CertificationCode == Code).FirstOrDefault();
                    if (ctr != null)
                    {
                        DateTime dtValue = (DateTime)ctr.DateCertified;
                        String Date = "";
                        if ( dtValue.Month.ToString().Length > 1){ Date += dtValue.Month.ToString()+"/";}
                        else{ Date +="0"+ dtValue.Month.ToString() + "/"; }
                        if(dtValue.Day.ToString().Length > 1) { Date += dtValue.Day.ToString() + "/"; }
                        else { Date += "0" + dtValue.Day.ToString() + "/"; }
                        Date += dtValue.Year.ToString();
                        
                        if (Validator.compareTwoDate(DateP, Date))
                        {
                            certificationTracker.DateCertified = DateTime.ParseExact(DateP, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            error.Add("Date is prior to Last Certified Date!");
                            message += "Date is prior to Last Certified Date!";
                        }
                            
                    }
                    
                }
                else
                {
                    error.Add("Date is not Valid!");
                }
                if (error.Count > 0)
                {
                    foreach (var da in error)
                    {
                        System.Diagnostics.Debug.WriteLine(da);
                    }
                    return RedirectToAction("ModalFailed", "IT", new { message = message, id = Who, urlBack = "ReCertified", redirectUrl = RedirectURL });
                }
                else
                {
                    var TempCertificationTracker = ldcr.CertificationTrackers.Where(ct => ct.EmpBadgeNo ==
                    certificationTracker.EmpBadgeNo && ct.CertificationCode == certificationTracker.CertificationCode).FirstOrDefault();
                    if (TempCertificationTracker != null)
                    {
                        // Update that tracker
                        System.Diagnostics.Debug.WriteLine(certificationTracker.CertificationCode + " --- " + certificationTracker.EmpBadgeNo
                            + " -- " + certificationTracker.DateCertified);
                        TempCertificationTracker.DateRecertified = certificationTracker.DateCertified;
                        ldcr.Entry(TempCertificationTracker).State = EntityState.Modified;
                        ldcr.SaveChanges();
                        //ldcr.CertificationTrackers.Find(TempCertificationTracker);
                        message = emp.Last_Name + " , " + emp.First_Name + " is succesfully Re-Certified in " + TempCertificationTracker.CertificationCode;

                    }
                    return RedirectToAction("ModalSuccess", "IT", new { message = message ,id = Who , urlBack = "Details", redirectUrl = RedirectURL });
                }
               
            }
            else
            {

                if (DateP == null || String.IsNullOrEmpty(DateP)) {
                    ModelState.AddModelError("", "Please provide date certified.");
                    message += "Please provide date Re-certified.";
                }
                if (Code == "X") {
                    ModelState.AddModelError("", "Please choose certification code");
                    message = "Please choose certification code";
                }

                return RedirectToAction("ModalFailed", "IT", new { message = message, id = Who,urlBack = "ReCertified", redirectUrl = RedirectURL });
            }
          
        }

        [HttpGet]
        public ActionResult Details(String id, String urlBack)
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
                    double SKP = (Convert.ToDouble(empModel.TotalCertifications.Count() )/ Convert.ToDouble(empModel.Certifications.Count())) *(100);
                    if (!Double.IsNaN(SKP))
                    {
                        //ViewBag.SkillsCertifiedPercentage = Convert.ToInt32(SKP);
                        empModel.PercentAgeCertified = Convert.ToInt32(SKP);
                    }
                    else
                    {
                        //ViewBag.SkillsCertifiedPercentage = 0;
                        empModel.PercentAgeCertified = 0;
                    }
                    var ReCertified = ldcr.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID && cr.DateRecertified != null).OrderBy(cr => cr.Id).ToList().Count();
                    //ViewBag.TotalReCertified = ReCertified;
                    empModel.NumberReCertified = ReCertified;
                    double SRKP = (Convert.ToDouble(ReCertified) / Convert.ToDouble(empModel.Certifications.Count())) * (100);
                    if (!Double.IsNaN(SRKP))
                    {
                       // ViewBag.SkillsReCertifiedPercentage = Convert.ToInt32(SRKP);
                        empModel.PercentAgeReCertified = Convert.ToInt32(SRKP);
                    }
                    else
                    {
                       // ViewBag.SkillsReCertifiedPercentage = 0;
                        empModel.PercentAgeReCertified = 0;
                    }
                    
                    System.Diagnostics.Debug.WriteLine(Convert.ToInt32(SKP));
                    System.Diagnostics.Debug.WriteLine(Convert.ToInt32(SRKP));
                    var totalPointsCertiFied = 0;
                    var TotalPointsReCertified = 0;
                    foreach (var certification in empModel.Certifications)
                    {
                        if(empModel.TotalCertifications.FirstOrDefault(el => el.CertificationCode == certification.Code)==null)
                        {
                            CertificationTracker CT = new CertificationTracker();
                            CT.Id = 0;
                            CT.EmpBadgeNo = id;
                            CT.CertificationCode = certification.Code;
                            CT.DateCertified = null;
                            CT.DateRecertified = null;
                            empModel.MyCertifications.Add(CT);
                        }
                        else
                        {
                            empModel.MyCertifications.Add(empModel.TotalCertifications.FirstOrDefault(el => el.CertificationCode == certification.Code));
                            totalPointsCertiFied += Convert.ToInt32(certification.Points);
                        }
                        if(empModel.TotalCertifications.FirstOrDefault(el => el.CertificationCode == certification.Code && el.DateRecertified != null) != null)
                        {
                            TotalPointsReCertified +=Convert.ToInt32(certification.Points);
                        }
                    }
                    empModel.TotalPointsCertified = totalPointsCertiFied;
                    empModel.TotalPointsReCertified = TotalPointsReCertified;


                }
            }
            return View(empModel);
        }

        [HttpGet]
        public ActionResult Active(int? page, String searchInput = "")
        {
            List<Employees_Details> employees = cEE.Employees_Details.Where(emp => emp.Job_Status.ToUpper().Contains("CURRENT")).OrderBy(a => a.Last_Name).ToList();

            if (!string.IsNullOrEmpty(searchInput))
            {
                employees = employees.Where(a => a.Last_Name.ToLower().Contains(searchInput.ToLower()) || a.Employee_ID.Contains(searchInput)).ToList();
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);


            return View(employees.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult Inactive(int? page, String searchInput = "")
        {
            List<Employees_Details> employees = cEE.Employees_Details.Where(emp => emp.Job_Status.ToUpper().Contains("INACTIVE")).OrderBy(a => a.Last_Name).ToList();

            if (!string.IsNullOrEmpty(searchInput))
            {
                employees = employees.Where(a => a.Last_Name.ToLower().Contains(searchInput.ToLower()) || a.Employee_ID.Contains(searchInput)).ToList();
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);


            return View(employees.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult NewlyEmployees(int? page, String searchInput = "")
        {
            List<newlyEmployee> employees = cEE.newlyEmployees.OrderBy(nwEmp => nwEmp.Last_Name).ToList();

            if (!string.IsNullOrEmpty(searchInput))
            {
                employees = employees.Where(a => a.Last_Name.ToLower().Contains(searchInput.ToLower()) || a.Employee_ID.Contains(searchInput)).ToList();
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);


            return View(employees.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult ModalSuccess(String message, String id , String urlBack,String redirectUrl)
        {
            ViewBag.message = message;
            ViewBag.id = id;
            if (id != null) { ViewBag.id = id; }
            UrlModel mode = new UrlModel();
            if (id != null) { mode.EmpId = id; }
            mode.URLBack = urlBack;
            mode.RedirectUrl = redirectUrl;
            return View(mode);
        }

        [HttpGet]
        public ActionResult ModalFailed(String message, String id, String urlBack, String redirectUrl)
        {
            ViewBag.message = message;
            ViewBag.UrlBack = urlBack;
            if (id != null) { ViewBag.id = id; }
            UrlModel mode = new UrlModel();
            if (id != null) { mode.EmpId = id; }
            mode.URLBack = urlBack;
            mode.RedirectUrl = redirectUrl;
            return View(mode);
        }

        
        [HttpGet]
        public ActionResult Testing(String id, String urlBack)
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
                    double SKP = (Convert.ToDouble(empModel.TotalCertifications.Count()) / Convert.ToDouble(empModel.Certifications.Count())) * (100);
                    if (!Double.IsNaN(SKP))
                    {
                        //ViewBag.SkillsCertifiedPercentage = Convert.ToInt32(SKP);
                        empModel.PercentAgeCertified = Convert.ToInt32(SKP);
                    }
                    else
                    {
                        //ViewBag.SkillsCertifiedPercentage = 0;
                        empModel.PercentAgeCertified = 0;
                    }
                    var ReCertified = ldcr.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID && cr.DateRecertified != null).OrderBy(cr => cr.Id).ToList().Count();
                    //ViewBag.TotalReCertified = ReCertified;
                    empModel.NumberReCertified = ReCertified;
                    double SRKP = (Convert.ToDouble(ReCertified) / Convert.ToDouble(empModel.Certifications.Count())) * (100);
                    if (!Double.IsNaN(SRKP))
                    {
                        // ViewBag.SkillsReCertifiedPercentage = Convert.ToInt32(SRKP);
                        empModel.PercentAgeReCertified = Convert.ToInt32(SRKP);
                    }
                    else
                    {
                        // ViewBag.SkillsReCertifiedPercentage = 0;
                        empModel.PercentAgeReCertified = 0;
                    }

                    System.Diagnostics.Debug.WriteLine(Convert.ToInt32(SKP));
                    System.Diagnostics.Debug.WriteLine(Convert.ToInt32(SRKP));
                    var totalPointsCertiFied = 0;
                    var TotalPointsReCertified = 0;
                    foreach (var certification in empModel.Certifications)
                    {
                        if (empModel.TotalCertifications.FirstOrDefault(el => el.CertificationCode == certification.Code) == null)
                        {
                            CertificationTracker CT = new CertificationTracker();
                            CT.Id = 0;
                            CT.EmpBadgeNo = id;
                            CT.CertificationCode = certification.Code;
                            CT.DateCertified = null;
                            CT.DateRecertified = null;
                            empModel.MyCertifications.Add(CT);
                        }
                        else
                        {
                            empModel.MyCertifications.Add(empModel.TotalCertifications.FirstOrDefault(el => el.CertificationCode == certification.Code));
                            totalPointsCertiFied += Convert.ToInt32(certification.Points);
                        }
                        if (empModel.TotalCertifications.FirstOrDefault(el => el.CertificationCode == certification.Code && el.DateRecertified != null) != null)
                        {
                            TotalPointsReCertified += Convert.ToInt32(certification.Points);
                        }
                    }
                    empModel.TotalPointsCertified = totalPointsCertiFied;
                    empModel.TotalPointsReCertified = TotalPointsReCertified;


                }
            }
            return View(empModel);
        }
    }
}
