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
    public class tblUsersController : Controller
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();

        // GET: tblUsers

        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var tblUsers = db.tblUsers.Include(t => t.tblUserType);
            return View(tblUsers.ToList());
        }


        public ActionResult SubBranchUser()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyId = 0;
            int.TryParse(Convert.ToString(Session["CompanyID"]), out companyId);

            int branchtypeid = 0;
            int.TryParse(Convert.ToString(Session["BranchTypeID"]), out branchtypeid);

            int brchid = 0;
            brchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));

          

            if (branchtypeid == 1)
            {
                var tblUsers = from s in db.tblUsers
                               join sa in db.tblEmployees on s.UserID equals sa.UserID
                               where sa.CompanyID == companyId
                               select s;
                foreach (var item in tblUsers)
                {
                    item.FullName = item.FullName + "(" + db.tblEmployees.Where(e => e.UserID == item.UserID).FirstOrDefault().tblBranch.BranchName + ")";
                }
                //// var tblUsers = db.tblUsers.Include(t => t.tblUserType).Where(u=>u.);
                return View(tblUsers.ToList());


            }
            else
            {
                var tblUsers = from s in db.tblUsers
                               join sa in db.tblEmployees on s.UserID equals sa.UserID
                               where sa.tblBranch.BrchID == brchid
                               select s;

                foreach (var item in tblUsers)
                {
                    item.FullName = item.FullName + "(" + db.tblEmployees.Where(e => e.UserID == item.UserID).FirstOrDefault().tblBranch.BranchName + ")";
                }
                //// var tblUsers = db.tblUsers.Include(t => t.tblUserType).Where(u=>u.);
                return View(tblUsers.ToList());
            }

           

            
        }

        // GET: tblUsers/Details/5
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
            tblUser tblUser = db.tblUsers.Find(id);
            if (tblUser == null)
            {
                return HttpNotFound();
            }
            return View(tblUser);
        }

        // GET: tblUsers/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.UserTypeID = new SelectList(db.tblUserTypes, "UserTypeID", "UserType");
            return View();
        }

        // POST: tblUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblUser tblUser)
        {
            int companyId = 0;
            int.TryParse(Convert.ToString(Session["CompanyID"]), out companyId);
            if (companyId == 0)
            {
                tblUser.UserTypeID = 1;
            }
            else
            {
                tblUser.UserTypeID = 2;
            }
          
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (ModelState.IsValid)
            {
                db.tblUsers.Add(tblUser);
                db.SaveChanges();
                if (companyId == 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("SubBranchUser");
                }
            }
            ViewBag.UserTypeID = new SelectList(db.tblUserTypes, "UserTypeID", "UserType", tblUser.UserTypeID);
            return View(tblUser);
        }

        // GET: tblUsers/Edit/5
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
            tblUser tblUser = db.tblUsers.Find(id);
            if (tblUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserTypeID = new SelectList(db.tblUserTypes, "UserTypeID", "UserType", tblUser.UserTypeID);
            return View(tblUser);
        }

        // POST: tblUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblUser tblUser)
        {
            int companyId = 0;
            int.TryParse(Convert.ToString(Session["CompanyID"]), out companyId);
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (ModelState.IsValid)
            {
                db.Entry(tblUser).State = EntityState.Modified;
                db.SaveChanges();
                if (companyId == 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("SubBranchUser");
                }
               
            }
            ViewBag.UserTypeID = new SelectList(db.tblUserTypes, "UserTypeID", "UserType", tblUser.UserTypeID);
            return View(tblUser);
        }

        // GET: tblUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblUser tblUser = db.tblUsers.Find(id);
            if (tblUser == null)
            {
                return HttpNotFound();
            }
            return View(tblUser);
        }

        // POST: tblUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            tblUser tblUser = db.tblUsers.Find(id);
            db.tblUsers.Remove(tblUser);
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
