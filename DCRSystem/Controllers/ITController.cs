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
    [Authorize(Roles ="IT")]
    public class ITController : Controller
    {
        public commonEmployeesEntities cEE = new commonEmployeesEntities();
        public lear_DailiesCertificationRequirementEntities ldcr = new lear_DailiesCertificationRequirementEntities();
        // GET: IT
        public ActionResult Employee(int? page, String searchInput = "")
        {
            List<Employees_Details> employees = cEE.Employees_Details.OrderBy(a => a.Last_Name).ToList();

            if (!string.IsNullOrEmpty(searchInput))
            {
                employees = employees.Where(a => a.Last_Name.ToLower().Contains(searchInput.ToLower()) || a.Employee_ID.Contains(searchInput)).ToList();
            }
           
            int pageSize = 10;
            int pageNumber = (page ?? 1);


            return View(employees.ToPagedList(pageNumber, pageSize));
            //return View();
        }

        //public ActionResult PlanReCertification()
        //{
        //    ViewBag.Employees = new SelectList(cEE.Employees_Details.Select(s => new {
        //        badge_no = s.Employee_ID,
        //        Surname = s.Last_Name,
        //        Firstname = s.First_Name,
        //        FullName = s.First_Name + ", "
        //      + s.Last_Name,
        //        Position = s.Position,
        //        HiredDate = s.Hire_Date
        //    }).OrderBy(s => s.Surname), "badge_no", "FullName");

        //    ViewBag.Certifications = new SelectList(ldcr.Certifications.Select(c => new {
        //        ID = c.Id,
        //        Code = c.Code,
        //        Description = c.Description
        //    }).OrderBy(s => s.ID), "code", "code");

        //    ViewBag.Employees1 = cEE.Employees_Details.OrderBy(s => s.Last_Name).ToList();
        //    ViewBag.Certifications1 = ldcr.Certifications.OrderBy(s => s.Id).ToList();
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult AddPlanReCertification()
        //{
        //    var DateP = Request.Form["DatePlan"];
        //    var Who = Request.Form["Employee"];
        //    var Code = Request.Form["Code"];
        //    List<String> error = new List<string>();
        //    CertificationTracker certificationTracker = new CertificationTracker();
        //    if (DateP != null && Who != null && Code != null)
        //    {
        //        // Check if Employee is Exist
        //        var emp = cEE.Employees_Details.Where(u => u.Employee_ID.ToString().ToLower() == Who.ToString().ToLower()).FirstOrDefault();
        //        if (emp != null)
        //        {
        //            certificationTracker.EmpBadgeNo = emp.Employee_ID;
        //        }
        //        else
        //        {
        //            error.Add("BadgeNo not exist");
        //        }
        //        // Check if certification Code is Exist
        //        var code = ldcr.Certifications.Where(c => c.Code == Code).FirstOrDefault();
        //        if (code != null)
        //        {
        //            certificationTracker.CertificationCode = code.Code;
        //        }
        //        else
        //        {
        //            error.Add("Certification Code not exist");
        //        }

        //        //if (Validator.isValidDate(DateP))
        //        //{
        //        //    certificationTracker.DateCertified = DateTime.ParseExact(DateP, "MM/dd/yyyy", CultureInfo.InvariantCulture);
        //        //}
        //        //else
        //        //{
        //        //    error.Add("Date is prior from the date today!");
        //        //}
        //        if (error.Count > 0)
        //        {
        //            foreach (var da in error)
        //            {
        //                System.Diagnostics.Debug.WriteLine(da);
        //            }


        //            // return Content(DateP + " - " + Who + " - " + Code);
        //        }
        //        else
        //        {

        //           /// if(Validator.compareTwoDate(DateP,))
        //            var TempCertificationTracker = ldcr.CertificationTrackers.Where(ct => ct.EmpBadgeNo ==
        //            certificationTracker.EmpBadgeNo && ct.CertificationCode == certificationTracker.CertificationCode).FirstOrDefault();
        //            if (TempCertificationTracker != null)
        //            {
        //                System.Diagnostics.Debug.WriteLine(TempCertificationTracker.DateCertified.ToString());
        //                var DataFin = DateTime.ParseExact(DateP, "MM/dd/yyyy", CultureInfo.InvariantCulture);
        //                DateTime dtValue = (DateTime)TempCertificationTracker.DateCertified;
        //                System.Diagnostics.Debug.WriteLine(dtValue.ToShortDateString());
        //                //var DataFin1 = DateTime.ParseExact(dtValue.ToShortDateString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
        //                System.Diagnostics.Debug.WriteLine(DataFin.ToString());
        //                //System.Diagnostics.Debug.WriteLine(DataFin1.ToString());
        //               // if (Validator.compareTwoDate(DataFin.ToShortDateString(), dtValue.ToShortDateString()))
        //               // {
        //                    // Update that tracker
        //                    System.Diagnostics.Debug.WriteLine(certificationTracker.CertificationCode + " --- " + certificationTracker.EmpBadgeNo
        //                    + " -- " + certificationTracker.DateCertified);
        //                    TempCertificationTracker.DateRecertified = DateTime.ParseExact(DateP, "MM/dd/yyyy", CultureInfo.InvariantCulture);
        //                    ldcr.Entry(TempCertificationTracker).State = EntityState.Modified;
        //                    ldcr.SaveChanges();
        //              //  }
        //              //  else
        //               // {
        //                    //return Content("ERROR BHAI");
        //               // }

                         
        //                //ldcr.CertificationTrackers.Find(TempCertificationTracker).
        //            }
                    
        //            // return Content(error.Count.ToString());
        //        }
        //    }
        //    return RedirectToAction("PlanReCertification");
            
        //}

        //public ActionResult Certification()
        //{
        //    ViewBag.Employees = new SelectList(cEE.Employees_Details.Select(s => new {
        //        badge_no = s.Employee_ID,
        //        Surname = s.Last_Name,
        //        Firstname = s.First_Name,
        //        FullName = s.First_Name + ", "
        //      + s.Last_Name,
        //        Position = s.Position,
        //        HiredDate = s.Hire_Date
        //    }).OrderBy(s => s.Surname), "badge_no", "FullName");

        //    ViewBag.Certifications = new SelectList(ldcr.Certifications.Select(c => new {
        //        ID = c.Id,
        //        Code = c.Code,
        //        Description = c.Description
        //    }).OrderBy(s => s.ID), "code", "code");

        //    ViewBag.Employees1 = cEE.Employees_Details.OrderBy(s => s.Last_Name).ToList();
        //    ViewBag.Certifications1 = ldcr.Certifications.OrderBy(s => s.Id).ToList();
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult AddCertification()
        //{
        //    var DateP = Request.Form["DatePlan"];
        //    var Who = Request.Form["Employee"];
        //    var Code = Request.Form["Code"];
        //    List<String> error = new List<string>();
        //    CertificationTracker certificationTracker = new CertificationTracker();
        //    if (DateP != null && Who != null && Code != null)
        //    {
        //        // Check if Employee is Exist
        //        var emp = cEE.Employees_Details.Where(u => u.Employee_ID.ToString().ToLower() == Who.ToString().ToLower()).FirstOrDefault();
        //        if (emp != null)
        //        {
        //            certificationTracker.EmpBadgeNo = emp.Employee_ID;
        //        }
        //        else
        //        {
        //            error.Add("BadgeNo not exist");
        //        }
        //        // Check if certification Code is Exist
        //        var code = ldcr.Certifications.Where(c => c.Code == Code).FirstOrDefault();
        //        if (code != null)
        //        {
        //            certificationTracker.CertificationCode = code.Code;
        //        }
        //        else
        //        {
        //            error.Add("Certification Code not exist");
        //        }

        //        if (Validator.isValidDate(DateP))
        //        {
        //            certificationTracker.DateCertified = DateTime.ParseExact(DateP, "MM/dd/yyyy", CultureInfo.InvariantCulture);
        //        }
        //        else
        //        {
        //            error.Add("Date is prior from the date today!");
        //        }
        //        if (error.Count > 0)
        //        {
        //            foreach (var da in error)
        //            {
        //                System.Diagnostics.Debug.WriteLine(da);
        //            }


        //           // return Content(DateP + " - " + Who + " - " + Code);
        //        }
        //        else
        //        {
        //            var TempCertificationTracker = ldcr.CertificationTrackers.Where(ct => ct.EmpBadgeNo ==
        //            certificationTracker.EmpBadgeNo && ct.CertificationCode == certificationTracker.CertificationCode).FirstOrDefault();
        //            if (TempCertificationTracker != null)
        //            {
        //                //// Update that tracker
        //                //System.Diagnostics.Debug.WriteLine(certificationTracker.CertificationCode + " --- " + certificationTracker.EmpBadgeNo
        //                //    + " -- " + certificationTracker.DateCertified);
        //                //TempCertificationTracker.DateCertified = certificationTracker.DateCertified;
        //                //ldcr.Entry(TempCertificationTracker).State = EntityState.Modified;
        //                //ldcr.SaveChanges();
        //                //ldcr.CertificationTrackers.Find(TempCertificationTracker).
        //            }
        //            else
        //            {
        //                // Add the new Tracker
        //                ldcr.CertificationTrackers.Add(certificationTracker);
        //                ldcr.SaveChanges();
        //            }
        //           // return Content(error.Count.ToString());
        //        }
        //    }
        //    return RedirectToAction("Certification");
        //}

        [HttpGet]
        public ActionResult Certified(String id,String urlBack)
        {
            ViewBag.URLBack = urlBack;
            EmployeeModel empModel = new EmployeeModel();
            if (id != null)
            {
                var employee = cEE.Employees_Details.Where(emp => emp.Employee_ID == id).FirstOrDefault();
                if (employee != null)
                {
                    empModel.Employee = employee;
                   // ViewBag.Employee_Details = employee;
                }
            }
            empModel.TotalCertifications = ldcr.CertificationTrackers.Where(ct => ct.EmpBadgeNo == id).ToList();
            empModel.Certifications = ldcr.Certifications.OrderBy(ct => ct.Code).ToList<Certification>();
            foreach(var certTrack in empModel.Certifications)
            {
                if(empModel.TotalCertifications.FirstOrDefault(tc => tc.CertificationCode == certTrack.Code) == null)
                {
                    empModel.ImNotCertified.Add(certTrack);
                }
            }
            ViewBag.Certifications1 = ldcr.Certifications.OrderBy(s => s.Id).ToList();
            // return Content(id);
            return View(empModel);
        }

        [HttpPost]
        public ActionResult PostCertified()
        {
            var DateP = Request.Form["DateCertified"];
            var Who = Request.Form["Employee"];
            var Code = Request.Form["Code"];
            var RedirectURL = Request.Form["URLBACK"];
            var message = "";
           
            List<String> error = new List<string>();
            CertificationTracker certificationTracker = new CertificationTracker();
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
                    double SKP = (Convert.ToDouble(empModel.TotalCertifications.Count() )/ Convert.ToDouble(empModel.Certifications.Count()+2)) *(100);
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
                    double SRKP = (Convert.ToDouble(ReCertified) / Convert.ToDouble(empModel.Certifications.Count()+2)) * (100);
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

                    foreach(var certification in empModel.Certifications)
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
                        }
                    }
                }
            }
            return View(empModel);
        }

        [HttpGet]
        public ActionResult Active(int? page, String searchInput = "")
        {
            List<Employees_Details> employees = cEE.Employees_Details.Where(emp => emp.Job_Status.ToUpper() == "ACTIVE (CURRENT)").OrderBy(a => a.Last_Name).ToList();

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
            List<Employees_Details> employees = cEE.Employees_Details.Where(emp => emp.Job_Status.ToUpper() != "ACTIVE (CURRENT)").OrderBy(a => a.Last_Name).ToList();

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
