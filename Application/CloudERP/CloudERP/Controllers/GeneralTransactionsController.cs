using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatabaseAccess.Models;
using CloudERP.Models;
using DatabaseAccess;
using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Code;

namespace CloudERP.Controllers
{
    public class GeneralTransactionsController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        SP_GeneralTransaction accounts = new SP_GeneralTransaction();
        private GeneralTransactionEntry generalentry = new GeneralTransactionEntry();
        // GET: GeneralTransactions
        public ActionResult GeneralTransaction(GeneralTransactionMV transaction)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["GNMesage"] != null)
            {
                Session["GNMesage"] = string.Empty;
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));

            ViewBag.CreditAccountControlID = new SelectList(accounts.GetAllAccounts(companyid,branchid), "AccountSubControlID", "AccountSubControl","0");
            ViewBag.DebitAccountControlID = new SelectList(accounts.GetAllAccounts(companyid, branchid), "AccountSubControlID", "AccountSubControl","0");
            return View(transaction);
        }

        
        public ActionResult SaveGeneralTransaction(GeneralTransactionMV transaction)
        {
            Session["GNMesage"] = string.Empty;
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

            if (ModelState.IsValid)
            {
                string payinvoicenno = "GEN" + DateTime.Now.ToString("yyyyMMddHHmmssmm");
                var message = generalentry.ConfirmGeneralTransaction(transaction.TransferAmount, userid,
                    branchid, companyid, payinvoicenno, transaction.DebitAccountControlID, transaction.CreditAccountControlID, transaction.Reason);
                if (message.Contains("Succeed"))
                {
                    Session["GNMesage"] = message;
                    //return RedirectToAction("Journal");
                }
                else {
                    Session["GNMesage"] = "Some issue is occure, re-login and try again!";
                     
                }
            }

            ViewBag.CreditAccountControlID = new SelectList(accounts.GetAllAccounts(companyid, branchid), "AccountSubControlID", "AccountSubControl", "0");
            ViewBag.DebitAccountControlID = new SelectList(accounts.GetAllAccounts(companyid, branchid), "AccountSubControlID", "AccountSubControl", "0");
            return RedirectToAction("GeneralTransaction", new { transaction = transaction });
        }


        public ActionResult Journal()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["GNMesage"] != null)
            {
                Session["GNMesage"] = string.Empty;
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));

            var list = accounts.GetJournal(companyid, branchid, DateTime.Now, DateTime.Now);
            return View(list);
        }



        [HttpPost]
        public ActionResult Journal(DateTime FromDate, DateTime ToDate)
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
            var list = accounts.GetJournal(companyid, branchid, FromDate, ToDate);
            return View(list);
             
        }

        public ActionResult SubJournal(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["GNMesage"] != null)
            {
                Session["GNMesage"] = string.Empty;
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            if (id != null)
            {
                Session["SubBranchID"] = id;
            }
            branchid = Convert.ToInt32(Convert.ToString(Session["SubBranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));

            var list = accounts.GetJournal(companyid, branchid, DateTime.Now, DateTime.Now);
            return View(list);
        }


        [HttpPost]
        public ActionResult SubJournal(DateTime FromDate, DateTime ToDate)
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
            var list = accounts.GetJournal(companyid, branchid, FromDate, ToDate);
            return View(list);

        }
    }
}