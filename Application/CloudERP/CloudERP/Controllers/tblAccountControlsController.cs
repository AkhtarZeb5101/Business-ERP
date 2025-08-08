using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CloudERP.Models;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class tblAccountControlsController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        private List<AccountControlMV> accountControl = new List<AccountControlMV>();
        // GET: tblAccountControls
        public ActionResult Index()
        {
            accountControl.Clear();
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
            var tblAccountControls = db.tblAccountControls.Include(t => t.tblBranch).Include(t => t.tblUser).Include(t => t.tblCompany).Where(a => a.CompanyID == companyid && a.BranchID == branchid);
            foreach (var item in tblAccountControls)
            {
                accountControl.Add(new AccountControlMV{
                    AccountControlID = item.AccountControlID,
                    AccountControlName = item.AccountControlName,
                    AccountHeadID = item.AccountHeadID,
                    AccountHeadName = db.tblAccountHeads.Find(item.AccountHeadID).AccountHeadName,
                    BranchID = item.BranchID,
                    BranchName = item.tblBranch.BranchName,
                    CompanyID = item.CompanyID,
                    Name = item.tblCompany.Name,
                    UserID = item.UserID,
                    UserName = item.tblUser.UserName

            });
            }


            return View(accountControl.ToList());
        }

 
        // GET: tblAccountControls/Create
        public ActionResult Create()
        {
            ViewBag.AccountHeadID = new SelectList(db.tblAccountHeads, "AccountHeadID", "AccountHeadName");
            return View();
        }

        // POST: tblAccountControls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblAccountControl tblAccountControl)
        {
            accountControl.Clear();
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

            tblAccountControl.BranchID = branchid;
            tblAccountControl.CompanyID = companyid;
            tblAccountControl.UserID = userid;
            if (ModelState.IsValid)
            {
                var findcountrol = db.tblAccountControls.Where(a => a.CompanyID == companyid && a.BranchID == branchid && a.AccountControlName == tblAccountControl.AccountControlName).FirstOrDefault();
                if (findcountrol == null)
                {
                    db.tblAccountControls.Add(tblAccountControl);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist!";
                }

              
            }

            ViewBag.AccountHeadID = new SelectList(db.tblAccountHeads, "AccountHeadID", "AccountHeadName", tblAccountControl.AccountHeadID);
            return View(tblAccountControl);
        }

        // GET: tblAccountControls/Edit/5
        public ActionResult Edit(int? id)
        {
 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccountControl tblAccountControl = db.tblAccountControls.Find(id);
            if (tblAccountControl == null)
            {
                return HttpNotFound();
            }

            ViewBag.AccountHeadID = new SelectList(db.tblAccountHeads, "AccountHeadID", "AccountHeadName", tblAccountControl.AccountHeadID);
            return View(tblAccountControl);
        }

        // POST: tblAccountControls/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( tblAccountControl tblAccountControl)
        {
 

            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userid = 0;
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblAccountControl.UserID = userid;
            if (ModelState.IsValid)
            {
                var findcountrol = db.tblAccountControls.Where(a => a.CompanyID == tblAccountControl.CompanyID && a.BranchID == tblAccountControl.BranchID && a.AccountControlName == tblAccountControl.AccountControlName && a.AccountControlID != tblAccountControl.AccountControlID).FirstOrDefault();
                if (findcountrol == null)
                {
                    db.Entry(tblAccountControl).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist!";
                }


            }

            ViewBag.AccountHeadID = new SelectList(db.tblAccountHeads, "AccountHeadID", "AccountHeadName", tblAccountControl.AccountHeadID);
            return View(tblAccountControl);

            
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
