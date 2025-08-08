using DatabaseAccess;
using DatabaseAccess.Code;
using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class TrialBalanceController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        SP_TrialBalance trialBalance = new SP_TrialBalance();
        // GET: TrialBalance
        public ActionResult GetTrialBalance()
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
            var financialyearcheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
            string FinancialYearID = (financialyearcheck != null ? Convert.ToString(financialyearcheck.Rows[0][0]) : string.Empty);
            List<TrialBalanceModel> list = new List<TrialBalanceModel>();
            if (string.IsNullOrEmpty(FinancialYearID))
            {
                list = trialBalance.TrialBalance(branchid, companyid, 0);
            }
            else
            {
                list = trialBalance.TrialBalance(branchid, companyid, Convert.ToInt32(FinancialYearID));
            }
            return View(list);
        }

        [HttpPost]
        public ActionResult GetTrialBalance(int? id)
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
            var financialyearcheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
            string FinancialYearID = (financialyearcheck != null ? Convert.ToString(financialyearcheck.Rows[0][0]) : string.Empty);
            List<TrialBalanceModel> list = new List<TrialBalanceModel>();
            list = trialBalance.TrialBalance(branchid, companyid, (int)id);
            return View(list);
        }

        public ActionResult GetFinancialYear()
        {
            var getlist = db.tblFinancialYears.ToList();
            var list = new List<tblFinancialYear>();
            foreach (var item in getlist)
            {
                list.Add(new tblFinancialYear() { FinancialYearID = item.FinancialYearID, FinancialYear = item.FinancialYear });
            }
            return Json(
                new
                {
                    data = list
                }, JsonRequestBehavior.AllowGet);
        }
    }
}