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
           // - For Bar Chart - //
            List<Chart> dataPoints = new List<Chart>();
            var Certificates = ldcr.Certifications.ToList();
            foreach(var cert in Certificates)
            {
                dataPoints.Add(new Chart(cert.Code, getNumberOfCertifiedEmployee(cert.Code)));
               
            }
            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            // -End  For Bar Chart - //

            // - For Pie Chart - //
            List<PieChart> piePoints = new List<PieChart>();

            piePoints.Add(new PieChart("Certified", GetCertifiedEmployee()));
            piePoints.Add(new PieChart("Uncertified", ldcr.EmployeeDCR_Vw.ToList().Count - GetCertifiedEmployee()));
            ViewBag.PiePoints = JsonConvert.SerializeObject(piePoints);
            ViewBag.DateToday = getDateToday();
            // -End  For Bar Chart - //

            // -Start for MonthsChart - //
            List<MonthsChart> monthChartPoints = new List<MonthsChart>();

            //monthChartPoints.Add(new MonthsChart("Initial", 7655));
            monthChartPoints.Add(new MonthsChart("Jan", getCertifiedEmployeeByMonthThisYear("JANUARY")));
            monthChartPoints.Add(new MonthsChart("Feb", getCertifiedEmployeeByMonthThisYear("FEBUARY")));
            monthChartPoints.Add(new MonthsChart("Mar", getCertifiedEmployeeByMonthThisYear("MARCH")));
            monthChartPoints.Add(new MonthsChart("Apr", getCertifiedEmployeeByMonthThisYear("APRIL")));
            monthChartPoints.Add(new MonthsChart("May", getCertifiedEmployeeByMonthThisYear("MAY")));
            monthChartPoints.Add(new MonthsChart("Jun", getCertifiedEmployeeByMonthThisYear("JUNE")));
            monthChartPoints.Add(new MonthsChart("July", getCertifiedEmployeeByMonthThisYear("JULY")));
            monthChartPoints.Add(new MonthsChart("Aug", getCertifiedEmployeeByMonthThisYear("AUGUST")));
            monthChartPoints.Add(new MonthsChart("Sep", getCertifiedEmployeeByMonthThisYear("SEPTEMBER")));
            monthChartPoints.Add(new MonthsChart("Oct", getCertifiedEmployeeByMonthThisYear("OCTOBER")));
            monthChartPoints.Add(new MonthsChart("Nov", getCertifiedEmployeeByMonthThisYear("NOVEMBER")));
            monthChartPoints.Add(new MonthsChart("Dec", getCertifiedEmployeeByMonthThisYear("DECEMBER")));
            monthChartPoints.Add(new MonthsChart("Total", true, "{y}"));

            ViewBag.MonthChartPoints = JsonConvert.SerializeObject(monthChartPoints);
            // - End of MonthsChart - //

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

        public String getDateToday()
        {
            var dateToday = DateTime.Now;
            var Month = dateToday.ToString("MMMM");
            var day = dateToday.Day;
            var year = dateToday.Year;

            return Month +" "+day+" "+year;
        }

        public int GetCertifiedEmployee()
        {
            return ldcr.certificateTracker_Vw.GroupBy(x => x.EmpBadgeNo).ToList().Count();
        }
        public int getCertifiedEmployeeByMonthThisYear(String Month)
        {
            var certificate = ldcr.certificateTracker_Vw.ToList();
            var dateNow = Convert.ToDateTime(DateTime.Now);
           
            PartialViewModel model = new PartialViewModel();
            foreach (var cert in certificate)
            {
                if (Convert.ToDateTime(cert.DateCertified).Year.ToString().ToUpper().Equals(dateNow.Year.ToString().ToUpper()) && Convert.ToDateTime(cert.DateCertified).ToString("MMMM").ToUpper().Equals(Month.ToUpper()))
                {
                    var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == cert.EmpBadgeNo).FirstOrDefault();
                    if (employee != null)
                    {
                        if (!model.EmployeeDCR_Vws.Any(x => x.Employee_ID == employee.Employee_ID))
                        {
                            model.EmployeeDCR_Vws.Add(employee);
                        }
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine(model.EmployeeDCR_Vws.Count() + "fdjglkfdglfjhlkjgfkhjlkgfjhlkgfjhgfkhlgklkglhkglhkglhjklhgkjkhjgkjlghk");
            return model.EmployeeDCR_Vws.Count();
        }
    }
}