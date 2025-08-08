using CloudERP.Models;
using DatabaseAccess;
using DatabaseAccess.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SaleCartController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        private SaleEntry saleentry = new SaleEntry();
        // GET: SaleCart
        public ActionResult NewSale()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            double totalamount = 0;
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var find = db.tblSaleCartDetails.Where(i => i.BranchID == branchid && i.CompanyID == companyid && i.UserID == userid);
            foreach (var item in find)
            {
                totalamount += (item.SaleQuantity * item.SaleUnitPrice);
            }
            ViewBag.TotalAmount = totalamount;
            return View(find.ToList());
        }

        [HttpPost]
        public ActionResult AddItem(int PID, int Qty, float Price)
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
            var checkqty = db.tblStocks.Find(PID);
            if (Qty > checkqty.Quantity)
            {
                ViewBag.Message = "Sale Quantity Must be Less Then are equal to Avl Qty.";
                return RedirectToAction("NewSale");
            }


            var find = db.tblSaleCartDetails.Where(i => i.ProductID == PID && i.BranchID == branchid && i.CompanyID == companyid).FirstOrDefault();
            if (find == null)
            {
                if (PID > 0 && Qty > 0 && Price > 0)
                {
                    var newitem = new tblSaleCartDetail()
                    {
                        ProductID = PID,
                        SaleQuantity = Qty,
                        SaleUnitPrice = Price,
                        BranchID = branchid,
                        CompanyID = companyid,
                        UserID = userid,
                    };

                   

                    db.tblSaleCartDetails.Add(newitem);
                    db.SaveChanges();
                    ViewBag.Message = "Item Add Successfully.";
                }
            }
            else
            {
                ViewBag.Message = "Already Exist! Plz Check.";
            }
            return RedirectToAction("NewSale");
        }

        public ActionResult DeleteConfirm(int? id)
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
            var product = db.tblSaleCartDetails.Find(id);
            if (product != null)
            {
                db.Entry(product).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                ViewBag.Message = "Deleted Successfully.";
                return RedirectToAction("NewSale");
            }
            ViewBag.Message = "Some Unexptected issue is occure, please contact to concern person!";
            var find = db.tblSaleCartDetails.Where(i => i.BranchID == branchid && i.CompanyID == companyid && i.UserID == userid);
            return View(find.ToList());
        }

        [HttpGet]
        public ActionResult GetProduct()
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
            List<ProductMV> list = new List<ProductMV>();
            var productlist = db.tblStocks.Where(p => p.BranchID == branchid && p.CompanyID == companyid).ToList();
            foreach (var item in productlist)
            {
                list.Add(new ProductMV { Name = item.ProductName+" (Avl Qty : "+ item.Quantity+" )", ProductID = item.ProductID });
            }

            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetProductDetails(int? id)
        {
            var product = db.tblStocks.Find(id);
            return Json(new { data = product.SaleUnitPrice }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CancelSale()
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
            var list = db.tblSaleCartDetails.Where(p => p.BranchID == branchid && p.CompanyID == companyid && p.UserID == userid).ToList();
            bool cancelstatus = false;
            foreach (var item in list)
            {
                db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                int noofrecords = db.SaveChanges();
                if (cancelstatus == false)
                {
                    if (noofrecords > 0)
                    {
                        cancelstatus = true;
                    }
                }
            }
            if (cancelstatus == true)
            {
                ViewBag.Message = "Sale is Canceled.";
                return RedirectToAction("NewSale");
            }
            ViewBag.Message = "Some Unexptected issue is occure, please contact to concern person!";
            return RedirectToAction("NewSale");
        }

        public ActionResult SelectCustomer()
        {
            Session["ErrorMessageSale"] = string.Empty;
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
            var saledetails = db.tblSaleCartDetails.Where(pd => pd.CompanyID == companyid && pd.BranchID == branchid).FirstOrDefault();
            if (saledetails == null)
            {
                Session["ErrorMessageSale"] = "Sale Cart Empty!";
                return RedirectToAction("NewSale");
            }
            var customers = db.tblCustomers.Where(s => s.CompanyID == companyid && s.BranchID == branchid).ToList();
            return View(customers);
        }

        [HttpPost]
        public ActionResult SaleConfirm(FormCollection collection)
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
            int customerid = 0;
            bool IsPayment = false;
            string[] keys = collection.AllKeys;
            foreach (var name in keys)
            {
                if (name.Contains("name"))
                {
                    string idname = name;
                    string[] valueids = idname.Split(' ');
                    customerid = Convert.ToInt32(valueids[1]);
                }
            }
            string Description = string.Empty;
            string[] Descriptionlist = collection["item.Description"].Split(',');
            if (Descriptionlist != null)
            {
                if (Descriptionlist[0] != null)
                {
                    Description = Descriptionlist[0];
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
            var customer = db.tblCustomers.Find(customerid);
            var saledetails = db.tblSaleCartDetails.Where(pd => pd.BranchID == branchid && pd.CompanyID == companyid).ToList();
            double totalamount = 0;
            foreach (var item in saledetails)
            {
                totalamount = totalamount + (item.SaleQuantity * item.SaleUnitPrice);
            }

            if (totalamount == 0)
            {
                ViewBag.Message = "Sale Cart Empty!";
                return View("NewSale");
            }
            string Invoiceno = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

            var invoiceheader = new tblCustomerInvoice()
            {
                BranchID = branchid,
                Title = "Sale Invoice" + customer.Customername,
                CompanyID = companyid,
                Description = Description,
                InvoiceDate = DateTime.Now,
                InvoiceNo = Invoiceno,
                CustomerID = customerid,
                UserID = userid,
                TotalAmount = totalamount
            };

            db.tblCustomerInvoices.Add(invoiceheader);
            db.SaveChanges();

            foreach (var item in saledetails)
            {
                var saledetail = new tblCustomerInvoiceDetail()
                {
                    ProductID = item.ProductID,
                    SaleQuantity = item.SaleQuantity,
                    SaleUnitPrice = item.SaleUnitPrice,
                    CustomerInvoiceID = invoiceheader.CustomerInvoiceID
                };

                db.tblCustomerInvoiceDetails.Add(saledetail);
                db.SaveChanges();
            }
            string Message = saleentry.ConfrimSale(companyid, branchid, userid, Invoiceno, invoiceheader.CustomerInvoiceID.ToString(), (float)totalamount, customerid.ToString(), customer.Customername, IsPayment);
            if (Message.Contains("Success"))
            {

                foreach (var item in saledetails)
                {
                    var stockitem = db.tblStocks.Find(item.ProductID);
                    stockitem.Quantity = stockitem.Quantity - item.SaleQuantity;
                    db.Entry(stockitem).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }


            }
            if (Message.Contains("Success"))
            {
                return RedirectToAction("PrintSaleInvoice", "SalePayment", new { id = invoiceheader.CustomerInvoiceID });  
            }
            Session["Message"] = Message;
            return RedirectToAction("NewSale");
        }
    }
}