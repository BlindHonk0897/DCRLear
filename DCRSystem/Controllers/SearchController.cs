using DCRSystem.CustomViewModel;
using DCRSystem.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
           
            List<EmployeeDCR_Vw> employees = ldcr.EmployeeDCR_Vw.Where(emp => emp.HRCCell.Equals(Cell)).ToList();
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
            model.allCells = ldcr.EmployeeDCR_Vw.Select(emp => emp.HRCCell).Distinct().ToList();
            model.Certifications = ldcr.Certifications.OrderBy(cert => cert.Code).ToList();
           // System.Diagnostics.Debug.WriteLine(model.allCells.Count()+"MAOAOAOOAOAOAOAOA");
            return View(model);
        }

        public List<EmployeeDCR_Vw> getEmployeeByMonth(String month)
        {
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
            return model.EmployeeDCR_Vws;
        }
        public List<EmployeeDCR_Vw> getEmployeeByYear(String Year)
        {
            PartialViewModel model = new PartialViewModel();
            var certificate = ldcr.CertificationTrackers.ToList();
            foreach (var cert in certificate)
            {
                if (Convert.ToDateTime(cert.DateCertified).Year.ToString().ToUpper().Equals(Year.ToUpper()))
                {
                    var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == cert.EmpBadgeNo).FirstOrDefault();
                    if (!model.EmployeeDCR_Vws.Any(x => x.Employee_ID == employee.Employee_ID))
                    {
                        model.EmployeeDCR_Vws.Add(employee);
                    }

                }
            }
            return model.EmployeeDCR_Vws;
        }

        public List<EmployeeDCR_Vw> getEmployeeByCell(String Cell)
        {
            PartialViewModel model = new PartialViewModel();
            model.EmployeeDCR_Vws = ldcr.EmployeeDCR_Vw.Where(emp => emp.HRCCell.Equals(Cell)).ToList(); 
            return model.EmployeeDCR_Vws;
        }

        public List<EmployeeDCR_Vw> getEmployeeByRCertificates(String Certificate)
        {
            
            PartialViewModel model = new PartialViewModel();
            var Certificates = ldcr.CertificationTrackers.Where(cert => cert.CertificationCode.Contains(Certificate) && cert.DateRecertified != null).ToList();
            foreach(var certific in Certificates)
            {
                var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID.Equals(certific.EmpBadgeNo)).FirstOrDefault();
                if (employee != null)
                {
                    model.EmployeeDCR_Vws.Add(employee);
                }
            }
            return model.EmployeeDCR_Vws;
        }

        public List<EmployeeDCR_Vw> getEmployeeByCertificates(String Certificate)
        {
            
            PartialViewModel model = new PartialViewModel();
            var Certificates = ldcr.CertificationTrackers.Where(cert => cert.CertificationCode.Contains(Certificate)).ToList();
            foreach (var certific in Certificates)
            {
                var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID.Equals(certific.EmpBadgeNo)).FirstOrDefault();
                if (employee != null)
                {
                    model.EmployeeDCR_Vws.Add(employee);
                }
            }
            return model.EmployeeDCR_Vws;
        }


        [HttpGet]
        public ActionResult _DynamicTableBody(String Type ,String data)
        {
            ViewBag.KeyName = Type;
            PartialViewModel model = new PartialViewModel();
            if (Request.IsAjaxRequest())
            {
                if (Type.ToString().ToUpper().Equals("BYMONTH"))
                {
                    model.EmployeeDCR_Vws = getEmployeeByMonth(data);
                }
                return PartialView(model); // Here must be return PartialView, not View.
            }
            else
            {
                return View();
            }

        }

        [HttpGet]
        public ActionResult _DynamicFilterBody(String keyName)
        {
            ViewBag.KeyName = keyName;
           
            SearchOptionsViewModel model = new SearchOptionsViewModel();

            model.KeyName = keyName;
            //model.allIds = ldcr.EmployeeDCR_Vw.Select(emp => emp.Employee_ID).ToList();
            model.allCells = ldcr.EmployeeDCR_Vw.Select(emp => emp.HRCCell).Distinct().ToList();
            model.Certifications = ldcr.Certifications.OrderBy(cert => cert.Code).ToList();
            model.allMedals = ldcr.Medals.ToList();
           
            if (Request.IsAjaxRequest())
            {
                return PartialView(model); // Here must be return PartialView, not View.
            }
            else
            {
                return View();
            }

        }
        public ActionResult Filter(int? page , String Type="", String data = "")
        {

            SearchOptionsViewModel model = new SearchOptionsViewModel();
            model.filters = ldcr.Filters.ToList();
            ViewBag.Type = Type;
            ViewBag.Data = data;
            // Get all Employees from Database
            System.Diagnostics.Debug.WriteLine( (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(data))+ "sahdfjsfsdfhjd" );
            if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(data))
            {
                
                List<EmployeeDCR_Vw> employees = new List<EmployeeDCR_Vw>();

                if (Type.ToString().ToUpper().Equals("BYMONTH"))
                {
                    employees = getEmployeeByMonth(data);
                }

                if (Type.ToString().ToUpper().Equals("BYYEAR"))
                {
                    employees = getEmployeeByYear(data);
                }

                if (Type.ToString().ToUpper().Equals("BYCERTIFICATES"))
                {
                    employees = getEmployeeByCertificates(data);
                }

                if (Type.ToString().ToUpper().Equals("BYRCERTIFICATES"))
                {
                    employees = getEmployeeByRCertificates(data);
                }

                if (Type.ToString().ToUpper().Equals("BYCELL"))
                {
                    employees = getEmployeeByCell(data);
                }

                // wala pa neh

                //if (Type.ToString().ToUpper().Equals("BYMEDAL"))
                //{
                //    employees = getEmployeeByMedal(data);
                //}

                //if (Type.ToString().ToUpper().Equals("BYLASTNAME"))
                //{
                //    employees = getEmployeeByLastName(data);
                //}
               
                //if (Type.ToString().ToUpper().Equals("BYMONTHANDYEAR"))
                //{
                //    employees = getEmployeeByMonthAndYear(data);
                //}

                // kutob dire
                ViewBag.EmpCount = employees.Count();
                int pageSize = 10; // pagelist number of page
                int pageNumber = (page ?? 1);   
                return View(employees.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<EmployeeDCR_Vw> employees = ldcr.EmployeeDCR_Vw.OrderBy(a => a.Last_Name).ToList();
                ViewBag.EmpCount = employees.Count();
                int pageSize = 10; // pagelist number of page
                int pageNumber = (page ?? 1);
                return View(employees.ToPagedList(pageNumber, pageSize));
            }
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
                //return Json(new { status = "success", redirectUrl = url, JsonRequestBehavior.AllowGet });
                return Json(new
                {
                    msg = "Successfully Event " + st.Name
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    msg = "UnSuccessfully Event "
                });
            }
        }

        public ActionResult ModalAssignNewCell(String IdEmp, String HRCCell, int? page, String urlBack)
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
            if (EmpDcr == null)
            {
                EmployeeDCR dcr = new EmployeeDCR
                {
                    BadgeNo = EmpID,
                    HRCCell = Hrccell,
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

            return RedirectToAction(Urlback, "Search", new { page = PageNum });
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

            return RedirectToAction(Urlback, "Search", new { page = PageNum });
        }
    }
}