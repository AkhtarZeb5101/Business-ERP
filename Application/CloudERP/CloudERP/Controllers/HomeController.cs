using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudERP.Models;

namespace CloudERP.Controllers
{
    public class HomeController : Controller
    {
        DatabaseAccess.CloudErpV1Entities db = new DatabaseAccess.CloudErpV1Entities();


        public ActionResult Index(string status)
        {
            Session["Status"] = status;
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));


            var dashboardData = DatabaseAccess.Code.SP_Code.SP_Deshboard.Get_DeshBoardHeader(branchid, companyid);
            var dmodel = new DashboardMV();
            dmodel.CurrentMonthExpenses = dashboardData.CurrentMonthExpenses;
            dmodel.NetIncome = dashboardData.NetIncome;
            dmodel.CashBankBalance = dashboardData.CashBankBalance;
            dmodel.TotalReceivable = dashboardData.TotalReceivable;
            dmodel.TotalPayable = dashboardData.TotalPayable;
            dmodel.Capital = dashboardData.Capital;
            dmodel.CurrentMonthRevenue = dashboardData.CurrentMonthRevenue;
            dmodel.CurrentMonthSale = dashboardData.CurrentMonthSale;
            dmodel.CurrentMonthSalePaymentSucceed = dashboardData.CurrentMonthSalePaymentSucceed;
            dmodel.CurrentMonthSalePaymentPending = dashboardData.CurrentMonthSalePaymentPending;
            dmodel.CurrentMonthReturnSale = dashboardData.CurrentMonthReturnSale;
            dmodel.CurrentMonthReturnSalePaymentPending = dashboardData.CurrentMonthReturnSalePaymentPending;
            dmodel.CurrentMonthReturnSalePaymentSucceed = dashboardData.CurrentMonthReturnSalePaymentSucceed;
            dmodel.CurrentMonthPurchase = dashboardData.CurrentMonthPurchase;
            dmodel.CurrentMonthPurchasePaidPayment = dashboardData.CurrentMonthPurchasePaidPayment;
            dmodel.CurrentMonthPurchaseRemainingPayment = dashboardData.CurrentMonthPurchaseRemainingPayment;
            dmodel.CurrentMonthReturnPurchase = dashboardData.CurrentMonthReturnPurchase;
            dmodel.CurrentMonthReturnPurchasePaymentPending = dashboardData.CurrentMonthReturnPurchasePaymentPending;
            dmodel.CurrentMonthReturnPurchasePaymentSucceed = dashboardData.CurrentMonthReturnPurchasePaymentSucceed;

            dmodel.DaySale = dashboardData.DaySale;
            dmodel.DaySalePaymentSucceed = dashboardData.DaySalePaymentSucceed;
            dmodel.DaySalePaymentPending = dashboardData.DaySalePaymentPending;
            dmodel.DayReturnSale = dashboardData.DayReturnSale;
            dmodel.DayReturnSalePaymentPending = dashboardData.DayReturnSalePaymentPending;
            dmodel.DayReturnSalePaymentSucceed = dashboardData.DayReturnSalePaymentSucceed;
            dmodel.DayPurchase = dashboardData.DayPurchase;
            dmodel.DayPurchasePaidPayment = dashboardData.DayPurchasePaidPayment;
            dmodel.DayPurchaseRemainingPayment = dashboardData.DayPurchaseRemainingPayment;
            dmodel.DayReturnPurchase = dashboardData.DayReturnPurchase;
            dmodel.DayReturnPurchasePaymentPending = dashboardData.DayReturnPurchasePaymentPending;
            dmodel.DayReturnPurchasePaymentSucceed = dashboardData.DayReturnPurchasePaymentSucceed;

            return View(dmodel);
        }

        public ActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult LoginUser(string email, string password)
        {
            Session["Status"] = "nodashboard";
            var user = db.tblUsers.Where(u => u.Email == email && u.Password == password && u.IsActive == true).FirstOrDefault();
            if (user != null)
            {
                Session["UserID"] = user.UserID;
                Session["UserTypeID"] = user.UserTypeID;
                Session["FullName"] = user.FullName;
                Session["Email"] = user.Email;
                Session["ContactNo"] = user.ContactNo;
                Session["UserName"] = user.UserName;
                Session["Password"] = user.Password;
                Session["IsActive"] = user.IsActive;
                var EmployeeDetails = db.tblEmployees.Where(e => e.UserID == user.UserID).FirstOrDefault();
                if (EmployeeDetails == null)
                {
                    ViewBag.Message = "Please Contact to Adminstrator!";
                    Session["UserTypeID"] = string.Empty;
                    Session["FullName"] = string.Empty;
                    Session["Email"] = string.Empty;
                    Session["ContactNo"] = string.Empty;
                    Session["UserName"] = string.Empty;
                    Session["Password"] = string.Empty;
                    Session["IsActive"] = string.Empty;
                    Session["EmployeeID"] = string.Empty;
                    Session["EName"] = string.Empty;
                    Session["EPhoto"] = string.Empty;
                    Session["Designation"] = string.Empty;
                    Session["BranchID"] = string.Empty;
                    Session["CompanyID"] = string.Empty;
                    return View("Login");
                }

                Session["EmployeeID"] = EmployeeDetails.EmployeeID;
                Session["EName"] = EmployeeDetails.Name;
                Session["EPhoto"] = EmployeeDetails.Photo;
                Session["Designation"] = EmployeeDetails.Designation;
                Session["BranchID"] = EmployeeDetails.BranchID;
                Session["CompanyID"] = EmployeeDetails.CompanyID;

                var company = db.tblCompanies.Where(c => c.CompanyID == EmployeeDetails.CompanyID).FirstOrDefault();
                if (company == null)
                {
                    ViewBag.Message = "Please Contact to Adminstrator!";
                    Session["UserTypeID"] = string.Empty;
                    Session["FullName"] = string.Empty;
                    Session["Email"] = string.Empty;
                    Session["ContactNo"] = string.Empty;
                    Session["UserName"] = string.Empty;
                    Session["Password"] = string.Empty;
                    Session["IsActive"] = string.Empty;
                    Session["EmployeeID"] = string.Empty;
                    Session["EName"] = string.Empty;
                    Session["EPhoto"] = string.Empty;
                    Session["Designation"] = string.Empty;
                    Session["BranchID"] = string.Empty;
                    Session["CompanyID"] = string.Empty;
                    return View("Login");
                }

                Session["CName"] = company.Name;
                Session["Logo"] = company.Logo;
                var BranchType = db.tblBranches.Where(b => b.BranchID == EmployeeDetails.BranchID).FirstOrDefault();
                if (BranchType == null)
                {
                    ViewBag.Message = "Please Contact Adminstartor!";
                    return View("Login");
                }

                Session["BranchTypeID"] = BranchType.BranchTypeID;
                Session["BrchID"] = BranchType.BrchID == null ? 0 : BranchType.BrchID;
                if (user.UserTypeID == 1)
                {
                    return RedirectToAction("Index", "tblCompanies");
                }
                return RedirectToAction("Index");

            }
            else
            {

                ViewBag.Message = "incorrect creditionals";

                Session["UserTypeID"] = string.Empty;
                Session["FullName"] = string.Empty;
                Session["Email"] = string.Empty;
                Session["ContactNo"] = string.Empty;
                Session["UserName"] = string.Empty;
                Session["Password"] = string.Empty;
                Session["IsActive"] = string.Empty;
                Session["EmployeeID"] = string.Empty;
                Session["EName"] = string.Empty;
                Session["EPhoto"] = string.Empty;
                Session["Designation"] = string.Empty;
                Session["BranchID"] = string.Empty;
                Session["CompanyID"] = string.Empty;
                Session["BrchID"] = string.Empty;
            }

            return View("Login");
        }

        public ActionResult Logout()
        {
            Session["UserTypeID"] = string.Empty;
            Session["FullName"] = string.Empty;
            Session["Email"] = string.Empty;
            Session["ContactNo"] = string.Empty;
            Session["UserName"] = string.Empty;
            Session["Password"] = string.Empty;
            Session["IsActive"] = string.Empty;
            Session["EmployeeID"] = string.Empty;
            Session["EName"] = string.Empty;
            Session["EPhoto"] = string.Empty;
            Session["Designation"] = string.Empty;
            Session["BranchID"] = string.Empty;
            Session["CompanyID"] = string.Empty;
            Session["BrchID"] = string.Empty;
            return View("Login");
        }



        public ActionResult ForgetPassword()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
 

    }
}