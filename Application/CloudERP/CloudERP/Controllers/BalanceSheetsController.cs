using DatabaseAccess;
using DatabaseAccess.Code;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class BalanceSheetsController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        SP_BalanceSheet bal_sheet = new SP_BalanceSheet();
        // GET: BalanceSheets
        public ActionResult BalanceSheet()
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


            var FinancialYear  = db.tblFinancialYears.Where(f => f.IsActive == true).FirstOrDefault();
            if (FinancialYear == null)
            {
                ViewBag.Message =  "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
            }

            var balancesheet = bal_sheet.GetBalanceSheet(companyid, branchid, FinancialYear.FinancialYearID, new List<int>() { 1, 2, 3, 4, 5 });
            return View(balancesheet);
        }

        [HttpPost]
        public ActionResult BalanceSheet(int? id)
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
            var balancesheet = bal_sheet.GetBalanceSheet(companyid, branchid, (int)id, new List<int>() { 1, 2, 3, 4, 5 });
            return View(balancesheet);
        }


        public ActionResult SubBalanceSheet(string bnchid)
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

            var balancesheet = bal_sheet.GetBalanceSheet(companyid, branchid, FinancialYear.FinancialYearID, new List<int>() { 1, 2, 3, 4, 5 });
            return View(balancesheet);
        }

        [HttpPost]
        public ActionResult SubBalanceSheet(int? id)
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
            var balancesheet = bal_sheet.GetBalanceSheet(companyid, branchid, (int)id, new List<int>() { 1, 2, 3, 4, 5 });
            return View(balancesheet);
        }
    }
}