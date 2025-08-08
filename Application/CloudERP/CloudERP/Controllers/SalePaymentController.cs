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
    public class SalePaymentController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        SP_Sale sale = new SP_Sale();
        private SaleEntry saleentry = new SaleEntry();
        // GET: salePayment
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
            var list = sale.ReamaningPaymentList(companyid, branchid);
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
            var list = sale.SalePaymentHistory((int)id);
            var returndetails = db.tblCustomerReturnInvoices.Where(r => r.CustomerInvoiceID == id).ToList();
            if (returndetails != null)
            {
                if (returndetails.Count > 0)
                {
                    ViewData["ReturnSaleDetails"] = returndetails;
                }
            }
            double reaminingamount = 0;
            double totalinvoiceamount = db.tblCustomerInvoices.Find(id).TotalAmount;
            double totalpaidamount = db.tblCustomerPayments.Where(p => p.CustomerInvoiceID == id).Sum(p => p.PaidAmount);

            reaminingamount = totalinvoiceamount - totalpaidamount;

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
            var list = sale.SalePaymentHistory((int)id);
            double reaminingamount = 0;
            foreach (var item in list)
            {
                reaminingamount = item.RemainingBalance;
            }
            if (reaminingamount == 0)
            {
                reaminingamount = db.tblCustomerInvoices.Find(id).TotalAmount;
            }
            ViewBag.PreviousRemainingAmount = reaminingamount;
            ViewBag.InvoiceID = id;
            return View(list.ToList());
        }

        [HttpPost]
        public ActionResult PaidAmount(int? id, float previousremainingamount, float paidamount)
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
                if (paidamount > previousremainingamount)
                {
                    ViewBag.Message = "Payment Must be Less Then or Equal to Previous Remaining Amount!";
                    var list = sale.SalePaymentHistory((int)id);
                    double reaminingamount = 0;
                    foreach (var item in list)
                    {
                        reaminingamount = item.RemainingBalance;
                    }
                    if (reaminingamount == 0)
                    {
                        reaminingamount = db.tblCustomerInvoices.Find(id).TotalAmount;
                    }
                    ViewBag.PreviousRemainingAmount = reaminingamount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }

                string payinvoicenno = "INP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var customer = db.tblCustomers.Find(db.tblCustomerInvoices.Find(id).CustomerID);
                var saleinvoice = db.tblCustomerInvoices.Find(id);
                var salepaymentdetails = db.tblCustomerPayments.Where(p => p.CustomerInvoiceID == id);
                string message = saleentry.SalePayment(companyid, branchid, userid,
                    payinvoicenno, Convert.ToString(id), (float)saleinvoice.TotalAmount, paidamount, Convert.ToString(customer.CustomerID), customer.Customername, (previousremainingamount - paidamount));
                Session["Message"] = message;

                return RedirectToAction("RemainingPaymentList");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Please Try Again!";
                var list = sale.SalePaymentHistory((int)id);
                double reaminingamount = 0;
                foreach (var item in list)
                {
                    reaminingamount = item.RemainingBalance;
                }
                if (reaminingamount == 0)
                {
                    reaminingamount = db.tblCustomerInvoices.Find(id).TotalAmount;
                }
                ViewBag.PreviousRemainingAmount = reaminingamount;
                ViewBag.InvoiceID = id;
                return View(list);
            }
        }

        public ActionResult CustomSalesHistory(DateTime FromDate, DateTime ToDate)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            int.TryParse(Convert.ToString(Session["BranchID"]), out branchid);
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var list = sale.CustomSalesList(companyid, branchid, FromDate, ToDate);
            return View(list.ToList());
        }

        public ActionResult SubCustomSalesHistory(DateTime FromDate, DateTime ToDate, int? id)
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
            int.TryParse(Convert.ToString(Session["SubBranchID"]), out branchid);
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var list = sale.CustomSalesList(companyid, branchid, FromDate, ToDate);
            return View(list.ToList());
        }



        public ActionResult SaleItemDetail(int? id)
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
            var list = db.tblCustomerInvoiceDetails.Where(d => d.CustomerInvoiceID == id);
            return View(list.ToList());
        }

        public ActionResult PrintSaleInvoice(int? id)
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
            var list = db.tblCustomerInvoiceDetails.Where(d => d.CustomerInvoiceID == id);
            return View(list.ToList());
        }

    }
}