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
    public class PurchasePaymentReturnController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        private PurchaseEntry purchaseentry = new PurchaseEntry();
        SP_Purchase purchase = new SP_Purchase();
        // GET: PurchasePaymentReturn
        public ActionResult ReturnPurchasePendingAmount(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var list = purchase.PurchaseReturnPaymentPending(id);

            return View(list);
        }


        public ActionResult AllPurchasesPendingPayment()
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
            var list = purchase.GetReturnPurchasesPaymentPending(companyid, branchid);

            return View(list);
        }

        public ActionResult ReturnAmount(int? id)
        {

            var list = db.tblSupplierReturnPayments.Where(r=>r.SupplierReturnInvoiceID == id);
            double reaminingamount = 0;
            foreach (var item in list)
            {
                reaminingamount = item.RemainingBalance;
                if (reaminingamount == 0)
                {
                    return RedirectToAction("AllPurchasesPendingPayment");
                }
            }
            if (reaminingamount == 0)
            {
                reaminingamount = db.tblSupplierReturnInvoices.Find(id).TotalAmount;
            }

             

            ViewBag.PreviousRemainingAmount = reaminingamount;
            ViewBag.InvoiceID = id;
            return View(list);
        }


        [HttpPost]
        public ActionResult ReturnAmount(int? id, float previousremainingamount, float paymentamount)
        {

            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"]))
                       || string.IsNullOrEmpty(Convert.ToString(id)))
                {
                    return RedirectToAction("Login", "Home");
                }


                int companyid = 0;
                int branchid = 0;
                int userid = 0;
                branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
                companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
                userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
                if (paymentamount > previousremainingamount)
                {
                    ViewBag.Message = "Payment Must be Less Then or Equal to Previous Remaining Amount!";
                    var list = db.tblSupplierReturnPayments.Where(r => r.SupplierReturnInvoiceID == id);
                    double reaminingamount = 0;
                    foreach (var item in list)
                    {
                        reaminingamount = item.RemainingBalance;
                    }
                    if (reaminingamount == 0)
                    {
                        reaminingamount = db.tblSupplierReturnInvoices.Find(id).TotalAmount;
                    }
                    ViewBag.PreviousRemainingAmount = reaminingamount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }

                string payinvoicenno = "RPP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var supplier = db.tblSuppliers.Find(db.tblSupplierReturnInvoices.Find(id).SupplierID);
                var purchaseinvoice = db.tblSupplierReturnInvoices.Find(id);
                var purchasepaymentdetails = db.tblSupplierReturnPayments.Where(p => p.SupplierReturnInvoiceID == id);
                string message = purchaseentry.ReturnPurchasePayment(companyid, branchid, userid,
                    payinvoicenno, purchaseinvoice.SupplierInvoiceID.ToString(), purchaseinvoice.SupplierReturnInvoiceID, (float)purchaseinvoice.TotalAmount, paymentamount, Convert.ToString(supplier.SupplierID), supplier.SupplierName, (previousremainingamount - paymentamount));
                Session["Message"] = message;
                return RedirectToAction("ReturnAmount", new { id= id});
            }
            catch 
            {
            var list = db.tblSupplierReturnPayments.Where(r => r.SupplierReturnInvoiceID == id);
            double reaminingamount = 0;
            foreach (var item in list)
            {
                reaminingamount = item.RemainingBalance;
            }
            if (reaminingamount == 0)
            {
                reaminingamount = db.tblSupplierReturnInvoices.Find(id).TotalAmount;
            }
            ViewBag.PreviousRemainingAmount = reaminingamount;
            ViewBag.InvoiceID = id;
            return View(list);
           
            }

        }



    }
}