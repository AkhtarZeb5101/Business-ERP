using CloudERP.Models;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudERP.HelperCls
{
    public class ComboHelper
    {
        //private CloudErpV1Entities db = new CloudErpV1Entities();
        //public List<AccountControlMV> AccountControl()
        //{
        //    var tblAccountControls = db.tblAccountControls.Include(t => t.tblBranch).Include(t => t.tblUser).Include(t => t.tblCompany).Where(a => a.CompanyID == companyid && a.BranchID == branchid);
        //    foreach (var item in tblAccountControls)
        //    {
        //        accountControl.Add(new AccountControlMV
        //        {
        //            AccountControlID = item.AccountControlID,
        //            AccountControlName = item.AccountControlName,
        //            AccountHeadID = item.AccountHeadID,
        //            AccountHeadName = db.tblAccountHeads.Find(item.AccountHeadID).AccountHeadName,
        //            BranchID = item.BranchID,
        //            BranchName = item.tblBranch.BranchName,
        //            CompanyID = item.CompanyID,
        //            Name = item.tblCompany.Name,
        //            UserID = item.UserID,
        //            UserName = item.tblUser.UserName

        //        });
        //    }

        //}
    }
}