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
    [Authorize(Roles = "IT")] // Only IT Authorized can access this Controller
    public class SearchController : Controller
    {
        public commonEmployeesEntities cEE = new commonEmployeesEntities();
        public lear_DailiesCertificationRequirementEntities ldcr = new lear_DailiesCertificationRequirementEntities();
        // GET: Search

        [HttpGet]
        public ActionResult EmployeeByMonth(int? page, String month,String searchInput ="")
        {
            ViewBag.Month = month;
            PartialViewModel model = new PartialViewModel();
            var certificate = ldcr.CertificationTrackers.ToList();
            foreach (var cert in certificate)
            {
                if (Convert.ToDateTime(cert.DateCertified).ToString("MMMM").ToUpper().Equals(month.ToUpper()))
                {
                    var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == cert.EmpBadgeNo).FirstOrDefault();
                    if (!model.EmployeeDCR_Vws.Any(x => x.Employee_ID == employee.Employee_ID))
                    {
                        model.EmployeeDCR_Vws.Add(employee);
                    }

                }
            }
            List<EmployeeDCR_Vw> employees = model.EmployeeDCR_Vws;
            ViewBag.EmpCount = employees.Count();
            if (!string.IsNullOrEmpty(searchInput))
            {
                // get employee/employees with the same lastname with the input
                employees = model.EmployeeDCR_Vws.Where(a => a.Last_Name.ToLower().Contains(searchInput.ToLower()) || a.Employee_ID.Contains(searchInput)).ToList();
            }
            int pageSize = 7; // pagelist number of page
            int pageNumber = (page ?? 1);

            return View(employees.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult EmployeeByYear(int? page, String Year, String searchInput = "")
        {
            ViewBag.Year = Year;
            PartialViewModel model = new PartialViewModel();
            var certificate = ldcr.CertificationTrackers.ToList();

            foreach (var cert in certificate)
            {
                System.Diagnostics.Debug.WriteLine(Convert.ToDateTime(cert.DateCertified).Year.ToString().ToUpper());
                if (Convert.ToDateTime(cert.DateCertified).Year.ToString().ToUpper().Equals(Year.ToUpper()))
                {
                    var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == cert.EmpBadgeNo).FirstOrDefault();
                    if (!model.EmployeeDCR_Vws.Any(x => x.Employee_ID == employee.Employee_ID))
                    {
                        model.EmployeeDCR_Vws.Add(employee);
                    }

                }
            }
            List<EmployeeDCR_Vw> employees = model.EmployeeDCR_Vws;
            ViewBag.EmpCount = employees.Count();
            if (!string.IsNullOrEmpty(searchInput))
            {
                // get employee/employees with the same lastname with the input
                employees = model.EmployeeDCR_Vws.Where(a => a.Last_Name.ToLower().Contains(searchInput.ToLower()) || a.Employee_ID.Contains(searchInput)).ToList();
            }
            int pageSize = 7; // pagelist number of page
            int pageNumber = (page ?? 1);

            return View(employees.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult EmployeeByCell(int? page, String Cell, String searchInput = "")
        {
            ViewBag.Cell = Cell;
           
            List<EmployeeDCR_Vw> employees = ldcr.EmployeeDCR_Vw.Where(emp => emp.Cost_Center_Description.Equals(Cell)).ToList();
            ViewBag.CellCount = employees.Count();
            if (!string.IsNullOrEmpty(searchInput))
            {
                // get employee/employees with the same lastname with the input
                employees = employees.Where(a => a.Last_Name.ToLower().Contains(searchInput.ToLower()) || a.Employee_ID.Contains(searchInput)).ToList();
            }
            int pageSize = 7; // pagelist number of page
            int pageNumber = (page ?? 1);

            return View(employees.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult SearchOption()
        {
            SearchOptionsViewModel model = new SearchOptionsViewModel();
            model.allIds = ldcr.EmployeeDCR_Vw.Select(emp => emp.Employee_ID).ToList();
            model.allCells = ldcr.EmployeeDCR_Vw.Select(emp => emp.Cost_Center_Description).Distinct().ToList();
            model.Certifications = ldcr.Certifications.OrderBy(cert => cert.Code).ToList();
           // System.Diagnostics.Debug.WriteLine(model.allCells.Count()+"MAOAOAOOAOAOAOAOA");
            return View(model);
        }

        [HttpGet]
        public ActionResult _DynamicTableBody(String month)
        {
            ViewBag.Number = month;

            PartialViewModel model = new PartialViewModel();
            var certificate = ldcr.CertificationTrackers.ToList();
            foreach (var cert in certificate)
            {
                if (Convert.ToDateTime(cert.DateCertified).ToString("MMMM").ToUpper().Equals(month.ToUpper()))
                {
                    var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == cert.EmpBadgeNo).FirstOrDefault();
                    if (!model.EmployeeDCR_Vws.Any(x => x.Employee_ID == employee.Employee_ID))
                    {
                        model.EmployeeDCR_Vws.Add(employee);
                    }

                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(model); // Here must be return PartialView, not View.
            }
            else
            {
                return View();
            }

        }
        public ActionResult Filter()
        {
            SearchOptionsViewModel model = new SearchOptionsViewModel();
            model.filters = ldcr.Filters.ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult JqAJAX(Month st)
        {
            var urlBuilder = new UrlHelper(Request.RequestContext);
            var url = urlBuilder.Action("Employeesbymonth", "IT");
            var bole = "FALSE";
            if (Request.IsAjaxRequest())
            {
                bole = "TRUE";
            }
            try
            {
                //return RedirectToAction("Employeesbymonth","IT", new { month = st.Name });
                //  return JavaScript("document.location.replace('" + Url.Action("Employeesbymonth","IT") + "');");
                return Json(new { status = "success", redirectUrl = url, JsonRequestBehavior.AllowGet });
                //return Json(new
                //{
                //    msg = "Successfully Event " + bole
                //});
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    msg = "UnSuccessfully Event "
                });
            }
        }
    }
}