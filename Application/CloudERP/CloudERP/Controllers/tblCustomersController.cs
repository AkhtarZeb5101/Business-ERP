using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CloudERP.HelperCls;
using CloudERP.Models;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class tblCustomersController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();

        public ActionResult AllCustomer()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var tblCustomers = db.tblCustomers.Include(t => t.tblBranch).Include(t => t.tblUser).Include(t => t.tblCompany);
            return View(tblCustomers.ToList());
        }


        // GET: tblCustomers
        public ActionResult Index()
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

            var tblCustomers = db.tblCustomers.Include(t => t.tblBranch).
                Include(t => t.tblUser).Include(t => t.tblCompany).Where(c => c.CompanyID == companyid && c.BranchID == branchid);
            return View(tblCustomers.ToList());
        }

        // GET: tblCustomers
        public ActionResult SubBranchCustomer()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            List<BranchsCustomersMV> list = new List<BranchsCustomersMV>();
            int branchid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            List<int> branchids = Branch.GetBranchids(branchid, db);

            foreach (var item in branchids)
            {
                foreach (var customer in db.tblCustomers.Where(c => c.BranchID == item))
                {
                    var cus = new BranchsCustomersMV();
                    cus.BranchName = customer.tblBranch.BranchName;
                    cus.CompanyName = customer.tblCompany.Name;
                    cus.CustomerAddress = customer.CustomerAddress;
                    cus.CustomerArea = customer.CustomerArea;
                    cus.CustomerContact = customer.CustomerContact;
                    cus.Customername = customer.Customername;
                    cus.Description = customer.Description;
                    cus.User = customer.tblUser.UserName;
                    list.Add(cus);
                }
            }
            return View(list);
        }

        // GET: tblCustomers/Details/5
        public ActionResult Details(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomer tblCustomer = db.tblCustomers.Find(id);
            if (tblCustomer == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomer);
        }

        public ActionResult CustomerDetails(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomer tblCustomer = db.tblCustomers.Find(id);
            if (tblCustomer == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomer);
        }


        // GET: tblCustomers/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        // POST: tblCustomers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblCustomer tblCustomer)
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
            tblCustomer.BranchID = branchid;
            tblCustomer.CompanyID = companyid;
            tblCustomer.UserID = userid;

            if (ModelState.IsValid)
            {
                var find = db.tblCustomers.Where(c => c.Customername == tblCustomer.Customername && c.BranchID == branchid && c.CustomerContact == tblCustomer.CustomerContact).FirstOrDefault();
                if (find == null)
                {
                    db.tblCustomers.Add(tblCustomer);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist! Plz Check.";
                }
            }
            return View(tblCustomer);
        }

        // GET: tblCustomers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomer tblCustomer = db.tblCustomers.Find(id);
            if (tblCustomer == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomer);
        }

        // POST: tblCustomers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblCustomer tblCustomer)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userid = 0;
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblCustomer.UserID = userid;
            if (ModelState.IsValid)
            {
                var find = db.tblCustomers.Where(c => c.Customername == tblCustomer.Customername && c.CustomerContact == tblCustomer.CustomerContact && c.CustomerID != tblCustomer.CustomerID).FirstOrDefault();
                if (find == null)
                {
                    db.Entry(tblCustomer).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist! Plz Check.";
                }
            }
            return View(tblCustomer);
        }

        // GET: tblCustomers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomer tblCustomer = db.tblCustomers.Find(id);
            if (tblCustomer == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomer);
        }

        // POST: tblCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblCustomer tblCustomer = db.tblCustomers.Find(id);
            db.tblCustomers.Remove(tblCustomer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
