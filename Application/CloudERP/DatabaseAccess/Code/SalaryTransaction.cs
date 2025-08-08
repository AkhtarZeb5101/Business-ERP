using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code
{
   public class SalaryTransaction
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        DataTable dtEntries = null;

        public string Confirm(
            int EmployeeID,
            double TransferAmount,// Parameters
            int UserID,// Parameters
            int BranchID,// Parameters
            int CompanyID,// Parameters
            string InvoiceNo,// Parameters
            string SalaryMonth,
            string SalaryYear
            )
        {

            try
            {
                dtEntries = null;
                string transectiontitle = "Salary is Pending";
                var employee = db.tblEmployees.Find(EmployeeID);
                string employeename = string.Empty;
                if (employee != null)
                {
                    employeename = ", To " + employee.Name;
                }
                transectiontitle = transectiontitle + employeename;

                var financialyearcheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = (financialyearcheck != null ? Convert.ToString(financialyearcheck.Rows[0][0]) : string.Empty);
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
                }


                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;
                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)



                var account = db.tblAccountSettings.Where(a => a.AccountActivityID == 5 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(account.AccountHeadID);
                AccountControlID = Convert.ToString(account.AccountControlID);
                AccountSubControlID = Convert.ToString(account.AccountSubControlID);

                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(TransferAmount), DateTime.Now, transectiontitle);


                account = db.tblAccountSettings.Where(a => a.AccountActivityID == 16 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(account.AccountHeadID);
                AccountControlID = Convert.ToString(account.AccountControlID);
                AccountSubControlID = Convert.ToString(account.AccountSubControlID);
                transectiontitle = "Salary Succeed" + employeename;
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(TransferAmount), "0", DateTime.Now, transectiontitle);



                string paymentquery = string.Format("insert into tblPayroll(EmployeeID,BranchID,CompanyID,TransferAmount,PayrollInvoiceNo,PaymentDate,SalaryMonth,SalaryYear,UserID) " +
                  "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                  EmployeeID, BranchID, CompanyID, TransferAmount, InvoiceNo, DateTime.Now.ToString("yyyy/MM/dd"), SalaryMonth, SalaryYear, UserID);
                DatabaseQuery.Insert(paymentquery);
              



                foreach (DataRow entryrow in dtEntries.Rows)
                {
                    string entryquery = string.Format("insert into tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID,InvoiceNo, UserID,Credit,Debit,TransectionDate,TransectionTitle,CompanyID,BranchID) values " +
                 " ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')",
                 Convert.ToString(entryrow[0]), Convert.ToString(entryrow[1]), Convert.ToString(entryrow[2]), Convert.ToString(entryrow[3]), Convert.ToString(entryrow[4]), Convert.ToString(entryrow[5]), Convert.ToString(entryrow[6]), Convert.ToString(entryrow[7]), Convert.ToDateTime(Convert.ToString(entryrow[8])).ToString("yyyy/MM/dd HH:mm"), Convert.ToString(entryrow[9]), CompanyID, BranchID);
                    DatabaseQuery.Insert(entryquery);
                }

                return "Salary Succeed.";
            }
            catch (Exception ex)
            {

                return "Unexpected Error is Occur Plz Try Again!";
            }

        }

        private void SetEntries(
      string FinancialYearID,
      string AccountHeadID,
      string AccountControlID,
      string AccountSubControlID,
      string InvoiceNo,
      string UserID,
      string Credit,
      string Debit,
      DateTime TransactionDate,
      string TransectionTitle)
        {
            if (dtEntries == null)
            {
                dtEntries = new DataTable();
                dtEntries.Columns.Add("FinancialYearID");
                dtEntries.Columns.Add("AccountHeadID");
                dtEntries.Columns.Add("AccountControlID");
                dtEntries.Columns.Add("AccountSubControlID");
                dtEntries.Columns.Add("InvoiceNo");
                dtEntries.Columns.Add("UserID");
                dtEntries.Columns.Add("Credit");
                dtEntries.Columns.Add("Debit");
                dtEntries.Columns.Add("TransactionDate");
                dtEntries.Columns.Add("TransectionTitle");
            }
            if (dtEntries != null)
            {
                dtEntries.Rows.Add(
                FinancialYearID,
                AccountHeadID,
                AccountControlID,
                AccountSubControlID,
                InvoiceNo,
                UserID,
                Credit,
                Debit,
                TransactionDate,
                TransectionTitle);
            }
        }
    }
}
