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
using System.Web.Script.Serialization;
using System.Web.Services;

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
            List<EmployeeDCR_Vw> employees = ldcr.EmployeeDCR_Vw.OrderBy(a => a.Last_Name).ToList();

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
                var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id).FirstOrDefault();

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
            // [ GET Data from submitted form --
            var DateP = Request.Form["DateCertified"];
            var Who = Request.Form["Employee"];
            var Code = Request.Form["Code"];
            var RedirectURL = Request.Form["URLBACK"];
            var TrainingDate = Request.Form["TrainingDate"];
            // ----------------------
            var message = "";// initialize variable for message
            List<String> error = new List<string>();// initialize variable for error

            CertificationTracker certificationTracker = new CertificationTracker();

            // Validate Data submitted
            if (DateP != null && Who != null && Code != null && Code!="X" && !String.IsNullOrEmpty(DateP) && TrainingDate !=null && !String.IsNullOrEmpty(TrainingDate))
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
                    error.Add("Date Certified is not Valid!");
                }
                if (Validator.isValidDate(TrainingDate))
                {
                    certificationTracker.TrainingDate = DateTime.ParseExact(TrainingDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    error.Add("TrainingDate is not Valid!");
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
                if (TrainingDate == null || String.IsNullOrEmpty(TrainingDate))
                {
                    ModelState.AddModelError("", "Please provide Training Date.");
                    message += "Please provide Training Date.";
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
        public ActionResult ModalMessage(String message , String urlBack)
        {
            ViewBag.message = message;
            UrlModel mode = new UrlModel();
            mode.URLBack = urlBack;
            return View(mode);
        }


        [HttpGet]
        public ActionResult ReCertified(String id, String urlBack)
        {
            ViewBag.URLBack = urlBack;
            EmployeeModel empModel = new EmployeeModel();
            if (id != null)
            {
                var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id).FirstOrDefault();
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

                        // DELETE RECERTIFACTION PLan Of the Employee //
                        ldcr.deleteLastPlan(TempCertificationTracker.EmpBadgeNo);
                        Session["NumberOfRecertificationPlans"] = Convert.ToInt32(Session["NumberOfRecertificationPlans"]) - 1;
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
                var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id).FirstOrDefault();
                if (employee != null)
                {
                    empModel.Employee = new EmployeeDCR_Vw
                    {
                        Employee_ID = employee.Employee_ID,
                        First_Name = employee.First_Name,
                        Last_Name = employee.Last_Name,
                        HRCCell = employee.HRCCell,
                        Job_Status = employee.Job_Status,
                        HRCSupervisor = employee.HRCSupervisor,
                        PlanRecertificationDate = employee.PlanRecertificationDate,
                        Position = employee.Position
                    };
                    empModel.Certifications = ldcr.Certifications.OrderBy(l => l.Code).ToList();
                    empModel.TotalCertifications = ldcr.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID).OrderBy(cr => cr.CertificationCode).ToList();
                    empModel.CurrentCertification = ldcr.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID).OrderByDescending(cr => cr.DateCertified).FirstOrDefault();
                    double SKP = (Convert.ToDouble(empModel.TotalCertifications.Count() )/ Convert.ToDouble(empModel.Certifications.Count())) *(100);
                    if (!Double.IsNaN(SKP))
                    {
                       
                        empModel.PercentAgeCertified = Convert.ToInt32(SKP);
                    }
                    else
                    {                     
                       empModel.PercentAgeCertified = 0;
                    }
                    var ReCertified = ldcr.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID && cr.DateRecertified != null).OrderBy(cr => cr.Id).ToList().Count();
                    
                    empModel.NumberReCertified = ReCertified;
                    double SRKP = (Convert.ToDouble(ReCertified) / Convert.ToDouble(empModel.Certifications.Count())) * (100);
                    if (!Double.IsNaN(SRKP))
                    {                    
                        empModel.PercentAgeReCertified = Convert.ToInt32(SRKP);
                    }
                    else
                    {
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
                            empModel.ImNotCertified.Add(certification);
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
            List<EmployeeDCR_Vw> employees = ldcr.EmployeeDCR_Vw.Where(emp => emp.Job_Status.ToUpper().Contains("CURRENT")).OrderBy(a => a.Last_Name).ToList();

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
            List<EmployeeDCR_Vw> employees = ldcr.EmployeeDCR_Vw.Where(emp => emp.Job_Status.ToUpper().Contains("INACTIVE")).OrderBy(a => a.Last_Name).ToList();

            if (!string.IsNullOrEmpty(searchInput))
            {
                employees = employees.Where(a => a.Last_Name.ToLower().Contains(searchInput.ToLower()) || a.Employee_ID.Contains(searchInput)).ToList();
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

          System.Diagnostics.Debug.WriteLine(ldcr.GET_LastReCertificationPlanned("022675").FirstOrDefault().ToString());

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
                var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id).FirstOrDefault();
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


        [HttpGet]
        public ActionResult ReCertificationPlan(int? page, String searchInput = "")
        {

            // Get All Certification which is not yet Recertified
           var NotYetRecertified =  new SelectList(ldcr.certificateTracker_Vw.Where(s => s.DateRecertified == null).OrderBy(s => s.EmpBadgeNo).Select(s => new { EmpBadgeNo = s.EmpBadgeNo, Name = s.EmpBadgeNo +" - " +s.Last_Name +", "+ s.First_Name }).ToList().Distinct(), "EmpBadgeNo", "Name");
            // Pass to ViewBag
            ViewBag.NotYetRecertified = NotYetRecertified;

            // Get all Recertification Plan/Plans from Database
            List<ReCertificationPlan> reCertificationPlans = ldcr.ReCertificationPlans.OrderByDescending(a => a.Id).ToList();

            if (!string.IsNullOrEmpty(searchInput))
            {
                // get Recertification Plan/Plans with the same lastname with the input
                reCertificationPlans = reCertificationPlans.Where(a => a.Lastname.ToLower().Contains(searchInput.ToLower()) || a.Badge_No.ToString().Contains(searchInput)).ToList();
            }

            int pageSize = 10; // pagelist number of page
            int pageNumber = (page ?? 1);


            return View(reCertificationPlans.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult AddRecertification()
        {
            System.Diagnostics.Debug.WriteLine(Request.Form["EmployeeID"]);
            System.Diagnostics.Debug.WriteLine(Request.Form["Code"]);
            System.Diagnostics.Debug.WriteLine(Request.Form["DatePlanned"]);
            if (Request.Form["EmployeeID"] != null && Request.Form["Code"] != "X" && !String.IsNullOrEmpty(Request.Form["DatePlanned"]))
            {
                var EmpId = Request.Form["EmployeeID"].ToString();
                var Code = Request.Form["Code"].ToString();

                if ((ldcr.ReCertificationPlans.Where(rcp => rcp.Badge_No == EmpId).ToList().FirstOrDefault())== null)
                {
                    Employees_Details employee = cEE.Employees_Details.Where(emp => emp.Employee_ID == EmpId).FirstOrDefault();
                    ReCertificationPlan reCertificationPlan = new ReCertificationPlan();
                    reCertificationPlan.Badge_No = Request.Form["EmployeeID"];
                    reCertificationPlan.CertificationCode = Request.Form["Code"];
                    reCertificationPlan.PlanDate = Convert.ToDateTime(Request.Form["DatePlanned"]);
                    reCertificationPlan.Lastname = employee.Last_Name;
                    reCertificationPlan.Firstname = employee.First_Name;
                    ldcr.ReCertificationPlans.Add(reCertificationPlan);
                    ldcr.SaveChanges();
                    Session["NumberOfRecertificationPlans"] = Convert.ToInt32(Session["NumberOfRecertificationPlans"]) + 1;
                }
                else
                {
                    ReCertificationPlan reCertificationPlan = ldcr.ReCertificationPlans.Where(rcp => rcp.Badge_No == EmpId).ToList().FirstOrDefault();
                    reCertificationPlan.CertificationCode = Code;
                    reCertificationPlan.PlanDate = Convert.ToDateTime(Request.Form["DatePlanned"]);
                    ldcr. Entry(reCertificationPlan).State = EntityState.Modified;
                    ldcr.SaveChanges();
                }
            }
            return RedirectToAction("ReCertificationPlan");
        }

        [HttpPost]
        public ActionResult AddModalRecertification()
        {
            var EmpId = Request.Form["EmployeeID"].ToString();
            Employees_Details employee = cEE.Employees_Details.Where(emp => emp.Employee_ID == EmpId).FirstOrDefault();
            ViewBag.Name = employee.Last_Name + ", " + employee.First_Name;
            ModalAddRecertificactionModel modal = new ModalAddRecertificactionModel();
            modal.BadgeNo = EmpId;
            modal.CertificationTrackers = ldcr.CertificationTrackers.Where(ctr => ctr.EmpBadgeNo == EmpId && ctr.DateRecertified == null).ToList();
            return View(modal);
        }

        public ActionResult Testing2()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ModalCertifiedCertificates(String id ,String urlBack)
        {
            EmployeeModel model = new EmployeeModel();
            model.MyCertifications  = ldcr.CertificationTrackers.Where(ct => ct.EmpBadgeNo == id).ToList();
            var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id).FirstOrDefault();

            if (employee != null) // if exist
            {
                model.Employee = employee;
            }
            ViewBag.urlBack = urlBack;
            return View(model);
        }

        [HttpGet]
        public ActionResult ModalReCertifiedCertificates(String id, String urlBack)
        {
            EmployeeModel model = new EmployeeModel();
            model.MyCertifications = ldcr.CertificationTrackers.Where(ct => ct.EmpBadgeNo == id).ToList();
            var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id).FirstOrDefault();

            if (employee != null) // if exist
            {
                model.Employee = employee;
            }
            ViewBag.urlBack = urlBack;
            return View(model);
        }

        [HttpGet]
        public ActionResult ModalCurrentCertification(String id,String urlBack)
        {
            EmployeeModel model = new EmployeeModel();
            var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id).FirstOrDefault();
          
            if (employee != null) // if exist
            {
                model.Employee = employee;
                model.CurrentCertification = ldcr.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID ).OrderByDescending(cr => cr.DateCertified).FirstOrDefault();
            }
            ViewBag.urlBack = urlBack;
            return View(model);
        }
        [HttpGet]
        public ActionResult ModalNotCertified(String id, String urlBack)
        {
            EmployeeModel empModel = new EmployeeModel();
            if (id != null)
            {
                // Get employee using the id
                var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id).FirstOrDefault();

                if (employee != null) // if exist
                {
                    empModel.Employee = employee;
                }
            }

            // Get the Total Certifications
            empModel.TotalCertifications = ldcr.CertificationTrackers.Where(ct => ct.EmpBadgeNo == id).ToList();

            // Get All Certifications
            empModel.Certifications = ldcr.Certifications.OrderBy(ct => ct.Code).ToList<Certification>();

            foreach (var certTrack in empModel.Certifications)
            {
                if (empModel.TotalCertifications.FirstOrDefault(tc => tc.CertificationCode == certTrack.Code) == null)
                {
                    // Add Certifications which employee is not Certified
                    empModel.ImNotCertified.Add(certTrack);
                }
            }
            ViewBag.urlBack = urlBack;
            return View(empModel);
        }

        public ActionResult ModalCeritificatesWithPoints(String id, String urlBack)
        {
            EmployeeModel empModel = new EmployeeModel();
            if (id != null)
            {
                // Get employee using the id
                var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id).FirstOrDefault();

                if (employee != null) // if exist
                {
                    empModel.Employee = employee;
                }
            }
            empModel.Certifications = ldcr.Certifications.ToList();
            ViewBag.urlBack = urlBack;
            return View(empModel);
        }

        public ActionResult ModalAssignNewCell(String IdEmp ,String HRCCell ,int? page,String urlBack)
        {
            EmployeeModel empModel = new EmployeeModel();
            empModel.Employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == IdEmp).FirstOrDefault();
            ViewBag.Page = page;
            ViewBag.UrlBack = urlBack;
            return View(empModel);
        }

        public ActionResult ModalAssignNewSupervisor(String IdEmp, String HRCSupervisor, int? page, String urlBack)
        {
            EmployeeModel empModel = new EmployeeModel();
            empModel.Employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == IdEmp).FirstOrDefault();
            ViewBag.Page = page;
            ViewBag.UrlBack = urlBack;
            return View(empModel);
        }

        [HttpPost]
        public ActionResult AssignNewCell()
        {
            var EmpID = Request.Form["EmployeeID"];
            var Hrccell = Request.Form["HrcCell"];
            var Cell = Request.Form["Cell"];
            var PageNum = Request.Form["PageNum"];
            var Urlback = Request.Form["UrlBack"];
            if (Cell == "X") { Cell = ""; }
            var EmpDcr = ldcr.EmployeeDCRs.Where(emp => emp.BadgeNo == EmpID).FirstOrDefault();
            if(EmpDcr == null)
            {
                EmployeeDCR dcr = new EmployeeDCR
                {
                    BadgeNo = EmpID,
                    HRCCell =Hrccell,
                    CurrentCell = Cell
                };
                ldcr.EmployeeDCRs.Add(dcr);
                ldcr.SaveChanges();
            }
            else
            {
                EmpDcr.CurrentCell = Cell;               
                ldcr.Entry(EmpDcr).State = EntityState.Modified;
                ldcr.SaveChanges();
            }

            return RedirectToAction(Urlback,"IT" , new { page = PageNum});
        }

        [HttpPost]
        public ActionResult AssignNewSupervisor()
        {
            var EmpID = Request.Form["EmployeeID"];
            var Hrcsupervisor = Request.Form["HrcSupervisor"];
            var Supervisor = Request.Form["Supervisor"];
            var PageNum = Request.Form["PageNum"];
            var Urlback = Request.Form["UrlBack"];
            if (Supervisor == "X") { Supervisor = ""; }
            var EmpDcr = ldcr.EmployeeDCRs.Where(emp => emp.BadgeNo == EmpID).FirstOrDefault();
            if (EmpDcr == null)
            {
                EmployeeDCR dcr = new EmployeeDCR
                {
                    BadgeNo = EmpID,
                    HRCSupervisor = Hrcsupervisor,
                    CurrentSupervisor = Supervisor
                };
                ldcr.EmployeeDCRs.Add(dcr);
                ldcr.SaveChanges();
            }
            else
            {
                EmpDcr.CurrentSupervisor = Supervisor;
                ldcr.Entry(EmpDcr).State = EntityState.Modified;
                ldcr.SaveChanges();
            }

            return RedirectToAction(Urlback, "IT", new { page = PageNum });
        }
    }
}
