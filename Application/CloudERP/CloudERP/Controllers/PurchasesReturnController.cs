using DatabaseAccess;
using DatabaseAccess.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchasesReturnController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        private PurchaseEntry purchaseentry = new PurchaseEntry();
        // GET: PurchasesReturn
        public ActionResult FindPurchase()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            tblSupplierInvoice invoice;

            if (Session["InvoiceNo"] != null)
            {
                var invoiceno = Convert.ToString(Session["InvoiceNo"]);
                if (!string.IsNullOrEmpty(invoiceno))
                {
                    invoice = db.tblSupplierInvoices.Where(p => p.InvoiceNo == invoiceno.Trim()).FirstOrDefault();
                }
                else
                {
                    invoice = db.tblSupplierInvoices.Find(0);
                }
            }
            else {
                invoice = db.tblSupplierInvoices.Find(0);
            }
            return View(invoice);
        }

        [HttpPost]
        public ActionResult FindPurchase(string invoiceid)
        {
            Session["InvoiceNo"] = string.Empty;
            Session["ReturnMessage"] = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var purchaseinvoice = db.tblSupplierInvoices.Where(p=>p.InvoiceNo == invoiceid).FirstOrDefault();
            return View(purchaseinvoice);
        }

        [HttpPost]
        public ActionResult ReturnConfirm(FormCollection collection)
        {
            Session["InvoiceNo"] = string.Empty;
            Session["ReturnMessage"] = string.Empty;
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
            int supplierid = 0;
            int SupplierInvoiceID = 0;
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


            string Description = "Purchase Return";
            string[] SupplierInvoiceIDs = collection["supplierinvoiceid"].Split(',');
            if (SupplierInvoiceIDs != null)
            {
                if (SupplierInvoiceIDs[0] != null)
                {
                    SupplierInvoiceID = Convert.ToInt32(SupplierInvoiceIDs[0]);
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


            var purchasedetails = db.tblSupplierInvoiceDetails.Where(pd => pd.SupplierInvoiceID == SupplierInvoiceID).ToList();
            for (int i = 0; i < purchasedetails.Count; i++)
            {
                foreach (int proid in ProductIDs)
                {
                    if (proid == purchasedetails[i].ProductID)
                    {
                        TotalAmount = TotalAmount + (ReturnQuantity[i] * purchasedetails[i].purchaseUnitPrice);
                    }
                }
               
            }
            var supplierinvoice = db.tblSupplierInvoices.Find(SupplierInvoiceID);
            supplierid = supplierinvoice.SupplierID;
            if (TotalAmount == 0)
            {
                Session["InvoiceNo"] = supplierinvoice.InvoiceNo;
                Session["ReturnMessage"] = "Must be at least one product return qty!";
                return RedirectToAction("FindPurchase");
            }


            string Invoiceno = "RPU" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

            var returninvoiceheader = new tblSupplierReturnInvoice()
            {
                BranchID = branchid,
                CompanyID = companyid,
                Description = Description,
                InvoiceDate = DateTime.Now,
                InvoiceNo = Invoiceno,
                SupplierID = supplierid,
                UserID = userid,
                TotalAmount = TotalAmount,
                SupplierInvoiceID = SupplierInvoiceID
            };

            db.tblSupplierReturnInvoices.Add(returninvoiceheader);
            db.SaveChanges();
            var supplier = db.tblSuppliers.Find(supplierid);
            string Message = purchaseentry.ReturnPurchase(companyid, branchid, userid, Invoiceno, returninvoiceheader.SupplierInvoiceID.ToString(), returninvoiceheader.SupplierReturnInvoiceID, (float)TotalAmount, supplierid.ToString(), supplier.SupplierName, IsPayment);
            if (Message.Contains("Success"))
            {

                for (int i = 0; i < purchasedetails.Count; i++)
                {
                    foreach (int proid in ProductIDs)
                    {
                        if (proid == purchasedetails[i].ProductID)
                        {

                            if (ReturnQuantity[i] > 0)
                            {
                                var rpd = new tblSupplierReturnInvoiceDetail();
                                rpd.SupplierInvoiceID = SupplierInvoiceID;
                                rpd.PurchaseReturnQuantity = ReturnQuantity[i];
                                rpd.ProductID = proid;
                                rpd.PurchaseReturnUnitPrice = purchasedetails[i].purchaseUnitPrice;
                                rpd.SupplierReturnInvoiceID = returninvoiceheader.SupplierReturnInvoiceID;
                                rpd.SupplierInvoiceDetailID = purchasedetails[i].SupplierInvoiceDetailID;
                                db.tblSupplierReturnInvoiceDetails.Add(rpd);
                                db.SaveChanges();

                                var stock = db.tblStocks.Find(proid);
                                stock.Quantity = (stock.Quantity - ReturnQuantity[i]);
                                db.Entry(stock).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                            }
                        }
                    }

                }
                Session["InvoiceNo"] = supplierinvoice.InvoiceNo;
                Session["ReturnMessage"] = "Return Successfully";
                return RedirectToAction("FindPurchase");
            }
            Session["InvoiceNo"] = supplierinvoice.InvoiceNo;
            Session["ReturnMessage"] = "Some unexpected issue is occure plz contact to Adminstrator!";
            return RedirectToAction("FindPurchase");
        }






    }
}