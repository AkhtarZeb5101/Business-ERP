using DatabaseAccess;
using DatabaseAccess.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class IncomeStatementController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
    
        private IncomeStatement income = new IncomeStatement();
   
        // GET: IncomeStatement
        public ActionResult GetIncomeStatement()
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
            var FinancialYear = db.tblFinancialYears.Where(f => f.IsActive == true).FirstOrDefault();
            if (FinancialYear == null)
            {
                ViewBag.Message = "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
            }
            var incomestatement = income.GetIncomeStatement(companyid, branchid, FinancialYear.FinancialYearID);

            return View(incomestatement);
        }

        [HttpPost]
        public ActionResult GetIncomeStatement(int? id)
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
             
            var incomestatement = income.GetIncomeStatement(companyid, branchid, (int)id);

            return View(incomestatement);
        }

        public ActionResult GetSubIncomeStatement(string bnchid)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            if (bnchid != null)
            {
                Session["SubBranchID"] = bnchid;
            }
            branchid = Convert.ToInt32(Convert.ToString(Session["SubBranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var FinancialYear = db.tblFinancialYears.Where(f => f.IsActive == true).FirstOrDefault();
            if (FinancialYear == null)
            {
                ViewBag.Message = "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
            }
            var incomestatement = income.GetIncomeStatement(companyid, branchid, FinancialYear.FinancialYearID);

            return View(incomestatement);
        }

        [HttpPost]
        public ActionResult GetSubIncomeStatement(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["SubBranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));

            var incomestatement = income.GetIncomeStatement(companyid, branchid, (int)id);

            return View(incomestatement);
        }
    }
}