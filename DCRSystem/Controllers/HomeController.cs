using DCRSystem.CustomViewModel;
using DCRSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace DCRSystem.Controllers
{
    public class HomeController : Controller
    {
        public commonEmployeesEntities cEE = new commonEmployeesEntities();
        public lear_DailiesCertificationRequirementEntities ldcr = new lear_DailiesCertificationRequirementEntities();

        [Authorize(Roles = "Default,Approver,IT")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Default,Approver,IT")]
        public ActionResult Employee()
        {
            ViewBag.Message = "Your application description page.";
            System.Diagnostics.Debug.WriteLine(Session["UserId"]);
            var checkerId = Session["UserId"].ToString();
            //EmpRouteViewModel viewModel = new EmpRouteViewModel();
            //viewModel.emp_Routes = cEE.Emp_Route.Where(u => u.Checker_1_ID == checkerId).ToList<Emp_Route>();
          
           ViewBag.Employees = new SelectList(cEE.Employees_Details.Select(s => new { badge_no = s.Employee_ID,
               Surname = s.Last_Name,Firstname = s.First_Name,FullName = s.First_Name +", "
               +s.Last_Name, Position = s.Position , HiredDate = s.Hire_Date }).OrderBy(s => s.Surname), "badge_no", "FullName");

            ViewBag.Employees1 = cEE.Employees_Details.OrderBy(s => s.Last_Name).ToList();
           // EmployeesViewModel viewModel = new EmployeesViewModel();
           // viewModel.Employees = cEE.Employees_Details.ToList<Employees_Details>();
            return View();
        }

        [Authorize(Roles = "Default,Approver,IT")]
        public ActionResult Home()
        {
            ViewBag.Message = "Your contact page.";
            List<Chart> dataPoints = new List<Chart>();
            var Certificates = ldcr.Certifications.ToList();

            foreach(var cert in Certificates)
            {
                dataPoints.Add(new Chart(cert.Code, getNumberOfCertifiedEmployee(cert.Code)));
            }

            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            return View();
        }

        [Authorize(Roles = "Default,Approver,IT")]
        public ActionResult MyCertificate()
        {
            ViewBag.Message = "Your MyCertificate page.";

            return View();
        }

        [Authorize(Roles = "Default,Approver,IT")]
        public ActionResult PlanReCertication()
        {
          
            return View();
        }

        [Authorize(Roles = "Default,Approver,IT")]
        [HttpPost]
        public ActionResult AddPlanReCertification()
        {
            var DateP = Request.Form["DatePlan"];
            var Badge = Request.Form["Employee"];

           // return Content(DateP +" --- "+ Badge);
          return RedirectToAction("Employee");
        }

        [Authorize(Roles = "Default,Approver,IT")]
        [HttpGet]
        public ActionResult About()
        {
            return View();
        }

        public int getNumberOfCertifiedEmployee(String CertificateCode)
        {
            return ldcr.certificateTracker_Vw.Where(cert => cert.CertificationCode == CertificateCode).ToList().Count();
        }
    }
}