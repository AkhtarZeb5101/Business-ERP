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
    public class SalesReturnController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        SP_Sale sale = new SP_Sale();
        private SaleEntry saleentry = new SaleEntry();
        // GET: SalesReturn
        public ActionResult FindSale()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            tblCustomerInvoice invoice;

            if (Session["SaleInvoiceNo"] != null)
            {
                var invoiceno = Convert.ToString(Session["SaleInvoiceNo"]);
                if (!string.IsNullOrEmpty(invoiceno))
                {
                    invoice = db.tblCustomerInvoices.Where(p => p.InvoiceNo == invoiceno.Trim()).FirstOrDefault();
                }
                else
                {
                    invoice = db.tblCustomerInvoices.Find(0);
                }
            }
            else
            {
                invoice = db.tblCustomerInvoices.Find(0);
            }
            Session["SaleInvoiceNo"] = string.Empty;
            return View(invoice);
        }

        [HttpPost]
        public ActionResult FindSale(string invoiceid)
        {
            Session["SaleInvoiceNo"] = string.Empty;
            Session["SaleReturnMessage"] = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var saleinvoice = db.tblCustomerInvoices.Where(p => p.InvoiceNo == invoiceid).FirstOrDefault();
            return View(saleinvoice);
        }

        [HttpPost]
        public ActionResult ReturnConfirm(FormCollection collection)
        {
            Session["SaleInvoiceNo"] = string.Empty;
            Session["SaleReturnMessage"] = string.Empty;
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
            int customerid = 0;
            int CustomerInvoiceID = 0;
            bool IsPayment = false;
            List<int> ProductIDs = new List<int>();
            List<int> ReturnQuantity = new List<int>();


            string[] keys = collection.AllKeys;
            foreach (var name in keys)
            {
                if (name.Contains("ProductID "))
                {
                    string idname = name;
                    string[] valueids = idname.Split(' ');
                    ProductIDs.Add(Convert.ToInt32(valueids[1]));
                    ReturnQuantity.Add(Convert.ToInt32(collection[idname].Split(',')[0]));
                }
            }


            string Description = "Sale Return";
            string[] CustomerInvoiceIDs = collection["customerinvoiceid"].Split(',');
            if (CustomerInvoiceIDs != null)
            {
                if (CustomerInvoiceIDs[0] != null)
                {
                    CustomerInvoiceID = Convert.ToInt32(CustomerInvoiceIDs[0]);
                }
            }

            if (collection["IsPayment"] != null)
            {
                string[] ispaymentdircet = collection["IsPayment"].Split(',');
                if (ispaymentdircet[0] == "on")
                {
                    IsPayment = true;
                }
                else
                {
                    IsPayment = false;
                }
            }
            else
            {
                IsPayment = false;
            }

            double TotalAmount = 0;


            var saledetails = db.tblCustomerInvoiceDetails.Where(pd => pd.CustomerInvoiceID == CustomerInvoiceID).ToList();
            for (int i = 0; i < saledetails.Count; i++)
            {
                foreach (int proid in ProductIDs)
                {
                    if (proid == saledetails[i].ProductID)
                    {
                        TotalAmount = TotalAmount + (ReturnQuantity[i] * saledetails[i].SaleUnitPrice);
                    }
                }

            }
            var customerinvoice = db.tblCustomerInvoices.Find(CustomerInvoiceID);
            customerid = customerinvoice.CustomerID;
            if (TotalAmount == 0)
            {
                Session["SaleInvoiceNo"] = customerinvoice.InvoiceNo;
                Session["SaleReturnMessage"] = "Must be at least one product return qty!";
                return RedirectToAction("FindSale");
            }


            string Invoiceno = "RIN" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

            var returninvoiceheader = new tblCustomerReturnInvoice()
            {
                BranchID = branchid,
                CompanyID = companyid,
                Description = Description,
                InvoiceDate = DateTime.Now,
                InvoiceNo = Invoiceno,
                CustomerID = customerid,
                UserID = userid,
                TotalAmount = TotalAmount,
                CustomerInvoiceID = CustomerInvoiceID
            };

            db.tblCustomerReturnInvoices.Add(returninvoiceheader);
            db.SaveChanges();
            var customer = db.tblCustomers.Find(customerid);
            string Message = saleentry.ReturnSale(companyid, branchid, userid, Invoiceno, returninvoiceheader.CustomerInvoiceID.ToString(), returninvoiceheader.CustomerReturnInvoiceID, (float)TotalAmount, customerid.ToString(), customer.Customername, IsPayment);
            if (Message.Contains("Success"))
            {

                for (int i = 0; i < saledetails.Count; i++)
                {
                    foreach (int proid in ProductIDs)
                    {
                        if (proid == saledetails[i].ProductID)
                        {

                            if (ReturnQuantity[i] > 0)
                            {
                                var rsd = new tblCustomerReturnInvoiceDetail();
                                rsd.CustomerInvoiceID = CustomerInvoiceID;
                                rsd.SaleReturnQuantity = ReturnQuantity[i];
                                rsd.ProductID = proid;
                                rsd.SaleReturnUnitPrice = saledetails[i].SaleUnitPrice;
                                rsd.CustomerReturnInvoiceID = returninvoiceheader.CustomerReturnInvoiceID;
                                rsd.CustomerInvoiceDetailID = saledetails[i].CustomerInvoiceDetailID;
                                db.tblCustomerReturnInvoiceDetails.Add(rsd);
                                db.SaveChanges();

                                var stock = db.tblStocks.Find(proid);
                                stock.Quantity = (stock.Quantity + ReturnQuantity[i]);
                                db.Entry(stock).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                            }
                        }
                    }

                }
                Session["SaleInvoiceNo"] = customerinvoice.InvoiceNo;
                Session["SaleReturnMessage"] = "( Sale Return Successfully )";
                return RedirectToAction("FindSale");
            }
            Session["SaleInvoiceNo"] = customerinvoice.InvoiceNo;
            Session["SaleReturnMessage"] = "Some unexpected issue is occure plz contact to Adminstrator!";
            return RedirectToAction("FindSale");
        }

    }
}