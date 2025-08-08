using CloudERP.HelperCls;
using CloudERP.Models;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatabaseAccess.Code;
namespace CloudERP.Controllers
{
    public class CompanyEmployeeController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();

        SalaryTransaction salarytransaction = new SalaryTransaction();
        // GET: CompanyEmployee
        public ActionResult Employees()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            var tblEmployee = db.tblEmployees.Where(c => c.CompanyID == companyid);
            return View(tblEmployee);
        }

        public ActionResult EmployeeRegistration()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            ViewBag.BranchID = new SelectList(db.tblBranches.Where(b => b.CompanyID == companyid), "BranchID", "BranchName", 0);
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
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
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

                return RedirectToAction("Employees");
            }

            return View(employee);
        }






        public ActionResult EmployeeSalary()
        {
            if (Session["SalaryMessage"] == null)
            {
                Session["SalaryMessage"] = string.Empty;
            }
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            var salary = new SalaryMV();
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            salary.SalaryMonth = DateTime.Now.AddMonths(-1).ToString("MMMM");
            salary.SalaryYear = DateTime.Now.AddMonths(-1).ToString("yyyy");
            return View(salary);

        }

        [HttpPost]
        public ActionResult EmployeeSalary(SalaryMV salary) // CNIC
        {
            Session["SalaryMessage"] = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var employee = db.tblEmployees.Where(p => p.CNIC == salary.CNIC).FirstOrDefault();
            salary.SalaryMonth = DateTime.Now.AddMonths(-1).ToString("MMMM");
            salary.SalaryYear = DateTime.Now.AddMonths(-1).ToString("yyyy");
            if (employee != null)
            {
                salary.EmployeeID = employee.EmployeeID;
                salary.EmployeeName = employee.Name;
                salary.Designation = employee.Designation;
                salary.CNIC = employee.CNIC;
                salary.TransferAmount = employee.MonthlySalary;
                Session["SalaryMessage"] = "";
            }
            else
            {
                Session["SalaryMessage"] = "Record Not Found!";
            }
            return View(salary);
        }




        [HttpPost]
        public ActionResult EmployeeSalaryConfirm(SalaryMV salary)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }
                int companyid = 0;
                int branchid = 0;
                int userid = 0;
                branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
                companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
                userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));

                salary.SalaryMonth = salary.SalaryMonth.ToLower();
                var emp = db.tblPayrolls.Where(p => p.EmployeeID == salary.EmployeeID && p.BranchID == branchid && p.CompanyID == companyid && p.SalaryMonth == salary.SalaryMonth && p.SalaryYear == salary.SalaryYear).FirstOrDefault();
                if (emp == null)
                {

                    string Invoiceno = "ESA" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                    string message = string.Empty;
                    if (ModelState.IsValid)
                    {
                        message = salarytransaction.Confirm(salary.EmployeeID, salary.TransferAmount, userid, branchid, companyid, Invoiceno, salary.SalaryMonth, salary.SalaryYear);

                    }
                    if (message.Contains("Succeed"))
                    {
                        Session["SalaryMessage"] = message;
                        int payrollno = db.tblPayrolls.Max(p => p.PayrollID);
                        return RedirectToAction("PrintSalaryInvoice", new { id = payrollno });
                    }
                    else
                    {
                        Session["SalaryMessage"] = "Plz re-login and try Again!";
                    }
                }
                else
                {
                    Session["SalaryMessage"] = "Salary is Already Paid!";
                }
                return RedirectToAction("EmployeeSalary");
            }
            catch
            {
                Session["SalaryMessage"] = "some unexpected issue is occure plz try again!";
                return RedirectToAction("EmployeeSalary");
            }

        }




        public ActionResult SalaryHistory()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));

            var salarylist = db.tblPayrolls.Where(p => p.BranchID == branchid && p.CompanyID == companyid).OrderByDescending(p => p.PayrollID).ToList();
            return View(salarylist);

        }

        public ActionResult PrintSalaryInvoice(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }


            var salary = db.tblPayrolls.Where(p => p.PayrollID == id).FirstOrDefault();
            Session["SalaryMessage"] = "";
            return View(salary);

        }





    }
}