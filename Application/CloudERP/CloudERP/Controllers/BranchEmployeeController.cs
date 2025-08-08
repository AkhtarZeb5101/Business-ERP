using CloudERP.HelperCls;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class BranchEmployeeController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        // GET: BranchEmployee
        public ActionResult Employee()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            var tblEmployee = db.tblEmployees.Where(c => c.CompanyID == companyid && c.BranchID == branchid);

            return View(tblEmployee);
        }


        public ActionResult EmployeeRegistration()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeRegistration(tblEmployee employee)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            employee.BranchID = branchid;
            employee.CompanyID = companyid;
            employee.UserID = null;
            if (ModelState.IsValid)
            {
                db.tblEmployees.Add(employee);
                db.SaveChanges();

                if (employee.LogoFile != null)
                {
                    var folder = "~/Content/EmployeePhoto";
                    var file = string.Format("{0}.jpg", employee.EmployeeID);
                    var response = FileHelpers.UploadPhoto(employee.LogoFile, folder, file);
                    if (response)
                    {
                        var pic = string.Format("{0}/{1}", folder, file);
                        employee.Photo = pic;
                        db.Entry(employee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Employee");
            }

            return View(employee);
        }


        public ActionResult EmployeeUpdation(int? id)
        {

            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = db.tblEmployees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeUpdation(tblEmployee employee)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            employee.BranchID = branchid;
            employee.CompanyID = companyid;
            employee.UserID = null;
            if (ModelState.IsValid)
            {
                if (employee.LogoFile != null)
                {
                    var folder = "~/Content/EmployeePhoto";
                    var file = string.Format("{0}.jpg", employee.EmployeeID);
                    var response = FileHelpers.UploadPhoto(employee.LogoFile, folder, file);
                    if (response)
                    {
                        var pic = string.Format("{0}/{1}", folder, file);
                        employee.Photo = pic;
                        db.Entry(employee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Employee");
            }

            return View(employee);
        }


        public ActionResult ViewProfile(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }
                if (id == null)
                {
                    return RedirectToAction("EP500", "EP");
                }
                int companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
                var employee = db.tblEmployees.Where(e => e.CompanyID == companyid && e.EmployeeID == id).FirstOrDefault();
                if (employee == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(employee);
            }
            catch  
            {
                return RedirectToAction("EP500", "EP");
            }
        }

    }
}