using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DCRSystem.Models;
using DCRSystem.CustomViewModel;
using System.Data.Entity;

namespace DCRSystem.Controllers
{
    [Authorize(Roles = "IT")] // Only IT Authorized can access this Controller
    public class CertificateController : Controller
    {
        // GET: Certificate
        public ActionResult Certificates()
        {
            CertificateViewModel model = new CertificateViewModel();
            using (lear_DailiesCertificationRequirementEntities db = new lear_DailiesCertificationRequirementEntities())
            {
                model.Certifications = db.Certifications.ToList<Certification>();
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(String id)
        {
            using (lear_DailiesCertificationRequirementEntities db = new lear_DailiesCertificationRequirementEntities())
            {
                int ID = Convert.ToInt32(id);
                var certification = db.Certifications.Where(crt => crt.Id == ID).FirstOrDefault();
                ViewBag.OldCode = certification.Code;
                return View(certification);
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostEdit([Bind(Include = "Id,Code,Description,Points")] Certification certification)
        {
            var oldCode = Request.Form["OldCode"].ToString();
            var Type = Request.Form["Type"];
            using (lear_DailiesCertificationRequirementEntities db = new lear_DailiesCertificationRequirementEntities())
            {
                if (ModelState.IsValid)
                {
                    if (oldCode.Equals(certification.Code.ToUpper()))
                    {
                        certification.Code = certification.Code.ToUpper();
                        certification.Description = certification.Description.ToUpper();
                        certification.Type = Type;
                        db.Entry(certification).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Certificates");
                    }
                    else
                    {
                        if (db.Certifications.Where(crt => crt.Code == certification.Code.ToUpper()).FirstOrDefault() == null)
                        {
                            certification.Code = certification.Code.ToUpper();
                            certification.Description = certification.Description.ToUpper();
                            certification.Type = Type;
                            db.Entry(certification).State = EntityState.Modified;
                            db.SaveChanges();
                            db.UpdateLDCRTablesWhenUpdateCertification(oldCode, certification.Code);
                            return RedirectToAction("Certificates");
                        }
                        else
                        {
                            return RedirectToAction("ModalFailed", new { id = certification.Id, errorMessage = "Code is already used! It must be unique. " });
                        }
                    }
                    
                   
                }
            }
            return RedirectToAction("Edit",new {id = certification.Id.ToString() });
        }

        public ActionResult ModalFailed(int id ,String errormessage)
        {
            ViewBag.ErrorMessage = errormessage;
            ViewBag.ID = id.ToString();
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Save()
        {
            var Code = Request.Form["Code"].ToUpper();
            var Description = Request.Form["Description"].ToUpper();
            var Point = Convert.ToInt32(Request.Form["Point"]);
            var Type = Request.Form["Type"];

            using (lear_DailiesCertificationRequirementEntities ldcr =new lear_DailiesCertificationRequirementEntities())
            {
                var exist = ldcr.Certifications.Where(cert => cert.Code == Code).FirstOrDefault();
                if (exist == null)
                {
                    Certification certification = new Certification
                    {
                        Code = Code,
                        Description = Description,
                        Points = Point,
                        Type = Type
                    };
                    ldcr.Certifications.Add(certification);
                    ldcr.SaveChanges();
                }
            }
            return RedirectToAction("Certificates", "Certificate");
        }
    }
}