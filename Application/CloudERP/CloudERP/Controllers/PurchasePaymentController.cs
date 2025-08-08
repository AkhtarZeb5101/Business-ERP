using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Code;

namespace CloudERP.Controllers
{
    public class PurchasePaymentController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        SP_Purchase purchase = new SP_Purchase();
        private PurchaseEntry paymententry = new PurchaseEntry();
        // GET: PurchasePayment
        public ActionResult RemainingPaymentList()
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
            var list = purchase.ReamaningPaymentList(companyid, branchid);
            return View(list.ToList());
        }

        public ActionResult PaidHistory(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])) || string.IsNullOrEmpty(Convert.ToString(id)))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var list = purchase.PurchasePaymentHistory((int)id);
            var returndetails = db.tblSupplierReturnInvoices.Where(r => r.SupplierInvoiceID == id).ToList();
            if (returndetails != null)
            {
                if (returndetails.Count > 0)
                {
                    ViewData["ReturnPurchaseDetails"] = returndetails;
                }
            }
            double reaminingamount = 0;
            double totalinvoiceamount = db.tblSupplierInvoices.Find(id).TotalAmount;
            double totalpaidamount = db.tblSupplierPayments.Where(p => p.SupplierInvoiceID == id).Sum(p => p.PaymentAmount);

            reaminingamount = totalinvoiceamount - totalpaidamount;


            //foreach (var item in list)
            //{
            //    reaminingamount = item.RemainingBalance;
            //}
            //if (reaminingamount == 0)
            //{
            //    reaminingamount = db.tblSupplierInvoices.Find(id).TotalAmount;
            //}
            ViewBag.PreviousRemainingAmount = reaminingamount;
            ViewBag.InvoiceID = id;
            return View(list.ToList());
        }



        public ActionResult PaidAmount(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])) || string.IsNullOrEmpty(Convert.ToString(id)))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var list = purchase.PurchasePaymentHistory((int)id);
            var returndetails = db.tblSupplierReturnInvoices.Where(r => r.SupplierInvoiceID == id).ToList();
            if (returndetails != null)
            {
                if (returndetails.Count > 0)
                {
                    ViewData["ReturnPurchaseDetails"] = returndetails;
                }
            }
            double reaminingamount = 0;
            double totalpaidamount = 0;
            double totalinvoiceamount = db.tblSupplierInvoices.Find(id).TotalAmount;
            if (db.tblSupplierPayments.Where(p => p.SupplierInvoiceID == id).FirstOrDefault() != null)
            {
                totalpaidamount = db.tblSupplierPayments.Where(p => p.SupplierInvoiceID == id).Sum(p => p.PaymentAmount);
            }
            reaminingamount = totalinvoiceamount - totalpaidamount;


            //foreach (var item in list)
            //{
            //    reaminingamount = item.RemainingBalance;
            //}
            //if (reaminingamount == 0)
            //{
            //    reaminingamount = db.tblSupplierInvoices.Find(id).TotalAmount;
            //}
            ViewBag.PreviousRemainingAmount = reaminingamount;
            ViewBag.InvoiceID = id;
            return View(list.ToList());
        }

        [HttpPost]
        public ActionResult PaidAmount(int? id,float previousremainingamount, float paymentamount)
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
                    var list = purchase.PurchasePaymentHistory((int)id);
                    var returndetails = db.tblSupplierReturnInvoices.Where(r => r.SupplierInvoiceID == id).ToList();
                    if (returndetails != null)
                    {
                        if (returndetails.Count > 0)
                        {
                            ViewData["ReturnPurchaseDetails"] = returndetails;
                        }
                    }
                    double reaminingamount = 0;
                    double totalinvoiceamount = db.tblSupplierInvoices.Find(id).TotalAmount;
                    double totalpaidamount = db.tblSupplierPayments.Where(p => p.SupplierInvoiceID == id).Sum(p => p.PaymentAmount);

                    reaminingamount = totalinvoiceamount - totalpaidamount;


                    //foreach (var item in list)
                    //{
                    //    reaminingamount = item.RemainingBalance;
                    //}
                    //if (reaminingamount == 0)
                    //{
                    //    reaminingamount = db.tblSupplierInvoices.Find(id).TotalAmount;
                    //}
                    ViewBag.PreviousRemainingAmount = reaminingamount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }

                string payinvoicenno = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var supplier = db.tblSuppliers.Find(db.tblSupplierInvoices.Find(id).SupplierID);
                var purchaseinvoice = db.tblSupplierInvoices.Find(id);
                var purchasepaymentdetails = db.tblSupplierPayments.Where(p => p.SupplierInvoiceID == id);
                string message = paymententry.PurchasePayment(companyid, branchid, userid,
                    payinvoicenno, Convert.ToString(id), (float)purchaseinvoice.TotalAmount, paymentamount, Convert.ToString(supplier.SupplierID), supplier.SupplierName, (previousremainingamount - paymentamount));
                Session["Message"] = message;

                return RedirectToAction("RemainingPaymentList");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Please Try Again!";
                var list = purchase.PurchasePaymentHistory((int)id);
                var returndetails = db.tblSupplierReturnInvoices.Where(r => r.SupplierInvoiceID == id).ToList();
                if (returndetails != null)
                {
                    if (returndetails.Count > 0)
                    {
                        ViewData["ReturnPurchaseDetails"] = returndetails;
                    }
                }
                double reaminingamount = 0;
                double totalinvoiceamount = db.tblSupplierInvoices.Find(id).TotalAmount;
                double totalpaidamount = db.tblSupplierPayments.Where(p => p.SupplierInvoiceID == id).Sum(p => p.PaymentAmount);

                reaminingamount = totalinvoiceamount - totalpaidamount;


                //foreach (var item in list)
                //{
                //    reaminingamount = item.RemainingBalance;
                //}
                //if (reaminingamount == 0)
                //{
                //    reaminingamount = db.tblSupplierInvoices.Find(id).TotalAmount;
                //}
                ViewBag.PreviousRemainingAmount = reaminingamount;
                ViewBag.InvoiceID = id;
                return View(list);
            }
        }


        public ActionResult CustomPurchasesHistory(DateTime FromDate, DateTime ToDate)
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
            var list = purchase.CustomPurchaseList(companyid, branchid, FromDate,ToDate);
            return View(list.ToList());
        }

        public ActionResult SubCustomPurchasesHistory(DateTime FromDate, DateTime ToDate, int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
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
            var list = purchase.CustomPurchaseList(companyid, branchid, FromDate, ToDate);
            return View(list.ToList());
        }

        public ActionResult PurchaseItemDetail(int? id)
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
            var list = db.tblSupplierInvoiceDetails.Where(d => d.SupplierInvoiceID == id);
            return View(list.ToList());
        }


        public ActionResult PrintPurchaseInvoice(int? id)
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
            var list = db.tblSupplierInvoiceDetails.Where(d => d.SupplierInvoiceID == id);
            return View(list.ToList());
        }

    }
}