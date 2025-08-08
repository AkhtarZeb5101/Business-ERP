using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class tblAccountSubControlsController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();

        // GET: tblAccountSubControls
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
            var tblAccountSubControls = db.tblAccountSubControls.Include(t => t.tblAccountControl).Include(t => t.tblAccountHead).
                Include(t => t.tblBranch).Include(t => t.tblUser).Where(t=>t.CompanyID == companyid && t.BranchID == branchid);
            return View(tblAccountSubControls.ToList());
        }


        // GET: tblAccountSubControls/Create
        public ActionResult Create()
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
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.AccountControlID = new SelectList(db.tblAccountControls.Where(a=>a.BranchID == branchid && a.CompanyID == companyid), "AccountControlID", "AccountControlName","0");
            return View();
        }

        // POST: tblAccountSubControls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblAccountSubControl tblAccountSubControl)
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
            tblAccountSubControl.BranchID = branchid;
            tblAccountSubControl.CompanyID = companyid;
            tblAccountSubControl.UserID = userid;
            tblAccountSubControl.AccountHeadID = db.tblAccountControls.Find(tblAccountSubControl.AccountControlID).AccountHeadID;
            if (ModelState.IsValid)
            {

                var find = db.tblAccountSubControls.Where(s => s.CompanyID == tblAccountSubControl.CompanyID && s.BranchID == tblAccountSubControl.BranchID && s.AccountSubControlName == tblAccountSubControl.AccountSubControlName).FirstOrDefault();
                if (find == null)
                {
                    db.tblAccountSubControls.Add(tblAccountSubControl);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist! Plz Check.";
                }
                
            }

            ViewBag.AccountControlID = new SelectList(db.tblAccountControls.Where(a => a.BranchID == branchid && a.CompanyID == companyid), "AccountControlID", "AccountControlName", tblAccountSubControl.AccountControlID);
            return View(tblAccountSubControl);
        }

        // GET: tblAccountSubControls/Edit/5
        public ActionResult Edit(int? id)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccountSubControl tblAccountSubControl = db.tblAccountSubControls.Find(id);
            if (tblAccountSubControl == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountControlID = new SelectList(db.tblAccountControls.Where(a => a.BranchID == branchid && a.CompanyID == companyid), "AccountControlID", "AccountControlName", tblAccountSubControl.AccountControlID);
            return View(tblAccountSubControl);
        }

        // POST: tblAccountSubControls/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblAccountSubControl tblAccountSubControl)
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
            tblAccountSubControl.UserID = userid;
            tblAccountSubControl.AccountHeadID = db.tblAccountControls.Find(tblAccountSubControl.AccountControlID).AccountHeadID;
            if (ModelState.IsValid)
            {
                var find = db.tblAccountSubControls.Where(s => s.CompanyID == tblAccountSubControl.CompanyID && s.BranchID == tblAccountSubControl.BranchID && s.AccountSubControlName == tblAccountSubControl.AccountSubControlName && s.AccountSubControlID != tblAccountSubControl.AccountSubControlID).FirstOrDefault();
                if (find == null)
                {
                    db.Entry(tblAccountSubControl).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist! Plz Check.";
                }

            }

            ViewBag.AccountControlID = new SelectList(db.tblAccountControls.Where(a => a.BranchID == branchid && a.CompanyID == companyid), "AccountControlID", "AccountControlName", tblAccountSubControl.AccountControlID);
            return View(tblAccountSubControl);
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
