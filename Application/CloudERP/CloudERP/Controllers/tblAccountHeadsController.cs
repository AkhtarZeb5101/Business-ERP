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
    public class tblAccountHeadsController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();

        // GET: tblAccountHeads
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

            var tblAccountHeads = db.tblAccountHeads.Include(t => t.tblUser).ToList();
            return View(tblAccountHeads.ToList());
        }

        // GET: tblAccountHeads/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccountHead tblAccountHead = db.tblAccountHeads.Find(id);
            if (tblAccountHead == null)
            {
                return HttpNotFound();
            }
            return View(tblAccountHead);
        }

        // GET: tblAccountHeads/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tblAccountHeads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( tblAccountHead tblAccountHead)
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
            tblAccountHead.UserID = userid;

            if (ModelState.IsValid)
            {

                var findhead = db.tblAccountHeads.Where(a => a.AccountHeadName == tblAccountHead.AccountHeadName).FirstOrDefault();
                if (findhead == null)
                {

                    db.tblAccountHeads.Add(tblAccountHead);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist!";
                }
            }
            return View(tblAccountHead);
        }

        // GET: tblAccountHeads/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccountHead tblAccountHead = db.tblAccountHeads.Find(id);
            if (tblAccountHead == null)
            {
                return HttpNotFound();
            }

            return View(tblAccountHead);
        }

        // POST: tblAccountHeads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( tblAccountHead tblAccountHead)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
       
            int userid = 0;
    
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));

            tblAccountHead.UserID = userid;

            if (ModelState.IsValid)
            {

                var findhead = db.tblAccountHeads.Where(a => a.AccountHeadName == tblAccountHead.AccountHeadName && a.AccountHeadID != tblAccountHead.AccountHeadID).FirstOrDefault();
                if (findhead == null)
                {

                    db.Entry(tblAccountHead).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist!";
                }
            }
            return View(tblAccountHead);
        }

        // GET: tblAccountHeads/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccountHead tblAccountHead = db.tblAccountHeads.Find(id);
            if (tblAccountHead == null)
            {
                return HttpNotFound();
            }
            return View(tblAccountHead);
        }

        // POST: tblAccountHeads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblAccountHead tblAccountHead = db.tblAccountHeads.Find(id);
            db.tblAccountHeads.Remove(tblAccountHead);
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
