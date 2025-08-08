using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code
{
    public class PurchaseEntry
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        public string selectsupplierid = string.Empty;
        DataTable dtEntries = null;
        public string ConfrimPurchase(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, float Amount, string SupplierID, string SupplierName, bool ispayment)
        {
            try
            {
                dtEntries = null;
                string pruchasetitle = "Purchase From " + SupplierName.Trim();
                var financialyearcheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = (financialyearcheck != null ? Convert.ToString(financialyearcheck.Rows[0][0]) : string.Empty);
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
                }
                string successmessage = "Purchase Success";

                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;
                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)
                var purchaseaccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 3 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                // Debit Entry Purchase
                AccountHeadID = Convert.ToString(purchaseaccount.AccountHeadID);
                AccountControlID = Convert.ToString(purchaseaccount.AccountControlID);
                AccountSubControlID = Convert.ToString(purchaseaccount.AccountSubControlID);
                string transectiontitle = string.Empty;
                transectiontitle = "Purchase From " + SupplierName.Trim();
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);


                // Credit Entry Purchase
                purchaseaccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 8 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(purchaseaccount.AccountHeadID);
                AccountControlID = Convert.ToString(purchaseaccount.AccountControlID);
                AccountSubControlID = Convert.ToString(purchaseaccount.AccountSubControlID);
                transectiontitle = SupplierName + " , Purchase Payment is Pending!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);


                if (ispayment == true)
                {
                    string payinvoicenno = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                    purchaseaccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 8 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    AccountHeadID = Convert.ToString(purchaseaccount.AccountHeadID);
                    AccountControlID = Convert.ToString(purchaseaccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(purchaseaccount.AccountSubControlID);
                    transectiontitle = "Payment Paid to " + SupplierName;
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, payinvoicenno, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);


                    purchaseaccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 9 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    AccountHeadID = Convert.ToString(purchaseaccount.AccountHeadID);
                    AccountControlID = Convert.ToString(purchaseaccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(purchaseaccount.AccountSubControlID);
                    transectiontitle = SupplierName + " , Purchase Payment is Succeed!";
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, payinvoicenno, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);


                    string paymentquery = string.Format("insert into tblSupplierPayment(SupplierID,SupplierInvoiceID,UserID,InvoiceNo,TotalAmount,PaymentAmount,RemainingBalance,CompanyID,BranchID,InvoiceDate) " +
                    "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                    SupplierID, SupplierInvoiceID, UserID, payinvoicenno, Amount, Amount, "0", CompanyID, BranchID,DateTime.Now.ToString("yyyy/MM/dd"));
                    DatabaseQuery.Insert(paymentquery);
                    successmessage = successmessage + " with Payment.";

                }

                foreach (DataRow entryrow in dtEntries.Rows)
                {
                    string entryquery = string.Format("insert into tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID,InvoiceNo, UserID,Credit,Debit,TransectionDate,TransectionTitle,CompanyID,BranchID) values " +
                 " ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')",
                 Convert.ToString(entryrow[0]), Convert.ToString(entryrow[1]), Convert.ToString(entryrow[2]), Convert.ToString(entryrow[3]), Convert.ToString(entryrow[4]), Convert.ToString(entryrow[5]), Convert.ToString(entryrow[6]), Convert.ToString(entryrow[7]), Convert.ToDateTime(Convert.ToString(entryrow[8])).ToString("yyyy/MM/dd HH:mm"), Convert.ToString(entryrow[9]), CompanyID, BranchID);
                    DatabaseQuery.Insert(entryquery);
                }

                return successmessage;
            }
            catch (Exception ex)
            {

                return "Unexpected Error is Occur Plz Try Again!";
            }
        }


        public string PurchasePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, float TotalAmount, float Amount, string SupplierID, string SupplierName, float RemainingBalance)
        {
            try
            {
                dtEntries = null;
                string pruchasetitle = "Purchase From " + SupplierName.Trim();
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

                string transectiontitle = string.Empty;
                

                var purchaseaccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 8 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(purchaseaccount.AccountHeadID);
                AccountControlID = Convert.ToString(purchaseaccount.AccountControlID);
                AccountSubControlID = Convert.ToString(purchaseaccount.AccountSubControlID);
                transectiontitle = "Payment Paid to " + SupplierName;
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);


                purchaseaccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 9 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(purchaseaccount.AccountHeadID);
                AccountControlID = Convert.ToString(purchaseaccount.AccountControlID);
                AccountSubControlID = Convert.ToString(purchaseaccount.AccountSubControlID);
                transectiontitle = SupplierName + " , Purchase Payment is Succeed!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);


                string paymentquery = string.Format("insert into tblSupplierPayment(SupplierID,SupplierInvoiceID,UserID,InvoiceNo,TotalAmount,PaymentAmount,RemainingBalance,CompanyID,BranchID,InvoiceDate) " +
                "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                SupplierID, SupplierInvoiceID, UserID, InvoiceNo, TotalAmount, Amount, Convert.ToString(RemainingBalance), CompanyID, BranchID,DateTime.Now.ToString("yyyy/MM/dd"));
                DatabaseQuery.Insert(paymentquery);
                foreach (DataRow entryrow in dtEntries.Rows)
                {
                    string entryquery = string.Format("insert into tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID,InvoiceNo, UserID,Credit,Debit,TransectionDate,TransectionTitle,CompanyID,BranchID) values " +
                 " ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')",
                 Convert.ToString(entryrow[0]), Convert.ToString(entryrow[1]), Convert.ToString(entryrow[2]), Convert.ToString(entryrow[3]), Convert.ToString(entryrow[4]), Convert.ToString(entryrow[5]), Convert.ToString(entryrow[6]), Convert.ToString(entryrow[7]), Convert.ToDateTime(Convert.ToString(entryrow[8])).ToString("yyyy/MM/dd HH:mm"), Convert.ToString(entryrow[9]), CompanyID, BranchID);
                    DatabaseQuery.Insert(entryquery);
                }

                return "Payment is Paid.";
            }
            catch (Exception ex)
            {

                return "Unexpected Error is Occur Plz Try Again!";
            }
        }

        // Purchase Return Function

        public string ReturnPurchase(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, int SupplierReturnInvoiceID, float Amount, string SupplierID, string SupplierName, bool ispayment)
        {
            try
            {
                dtEntries = null;
                string returnpruchasetitle = "Return Purchase to " + SupplierName.Trim();
                var financialyearcheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = (financialyearcheck != null ? Convert.ToString(financialyearcheck.Rows[0][0]) : string.Empty);
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
                }
                string successmessage = "Return Purchase Success";

                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;
                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)
                var returnpurchaseaccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 4 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                // Credit Entry  Return Purchase
                AccountHeadID = Convert.ToString(returnpurchaseaccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnpurchaseaccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnpurchaseaccount.AccountSubControlID);
                string transectiontitle = string.Empty;
                transectiontitle = " Return Purchase to " + SupplierName.Trim();
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0",  DateTime.Now, transectiontitle);


                // Debit Entry Return Purchase
                returnpurchaseaccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 12 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(returnpurchaseaccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnpurchaseaccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnpurchaseaccount.AccountSubControlID);
                transectiontitle = SupplierName + " ,Return Purchase Payment is Pending!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(),  "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);


                if (ispayment == true)
                {
                    string payinvoicenno = "RPP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                    returnpurchaseaccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 12 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    AccountHeadID = Convert.ToString(returnpurchaseaccount.AccountHeadID);
                    AccountControlID = Convert.ToString(returnpurchaseaccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(returnpurchaseaccount.AccountSubControlID);
                    transectiontitle = "Return Payment From " + SupplierName;
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, payinvoicenno, UserID.ToString(), Convert.ToString(Amount), "0",  DateTime.Now, transectiontitle);


                    returnpurchaseaccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 13 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    AccountHeadID = Convert.ToString(returnpurchaseaccount.AccountHeadID);
                    AccountControlID = Convert.ToString(returnpurchaseaccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(returnpurchaseaccount.AccountSubControlID);
                    transectiontitle = SupplierName + " , Return Purchase Payment is Succeed!";
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, payinvoicenno, UserID.ToString(),  "0", Convert.ToString(Amount),  DateTime.Now, transectiontitle);


                    string paymentquery = string.Format("insert into tblSupplierReturnPayment(SupplierID,SupplierInvoiceID,UserID,InvoiceNo,TotalAmount,PaymentAmount,RemainingBalance,CompanyID,BranchID,SupplierReturnInvoiceID,InvoiceDate) " +
                    "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                    SupplierID, SupplierInvoiceID, UserID, payinvoicenno, Amount, Amount, "0", CompanyID, BranchID, SupplierReturnInvoiceID,DateTime.Now.ToString("yyyy/MM/dd"));
                    DatabaseQuery.Insert(paymentquery);
                    successmessage = successmessage + " with Payment.";

                }

                foreach (DataRow entryrow in dtEntries.Rows)
                {
                    string entryquery = string.Format("insert into tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID,InvoiceNo, UserID,Credit,Debit,TransectionDate,TransectionTitle,CompanyID,BranchID) values " +
                 " ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')",
                 Convert.ToString(entryrow[0]), Convert.ToString(entryrow[1]), Convert.ToString(entryrow[2]), Convert.ToString(entryrow[3]), Convert.ToString(entryrow[4]), Convert.ToString(entryrow[5]), Convert.ToString(entryrow[6]), Convert.ToString(entryrow[7]), Convert.ToDateTime(Convert.ToString(entryrow[8])).ToString("yyyy/MM/dd HH:mm"), Convert.ToString(entryrow[9]), CompanyID, BranchID);
                    DatabaseQuery.Insert(entryquery);
                }

                return successmessage;
            }
            catch (Exception ex)
            {

                return "Unexpected Error is Occur Plz Try Again!";
            }
        }


        public string ReturnPurchasePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, int SupplierReturnInvoiceID, float TotalAmount, float Amount, string SupplierID, string SupplierName, float RemainingBalance)
        {
            try
            {
                dtEntries = null;
                string returnpruchasetitle = "Return Purchase to " + SupplierName.Trim();
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

                string transectiontitle = string.Empty;


                var returnpurchaseaccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 12 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(returnpurchaseaccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnpurchaseaccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnpurchaseaccount.AccountSubControlID);
                transectiontitle = "Return Payment From " + SupplierName;
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);


                returnpurchaseaccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 13 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(returnpurchaseaccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnpurchaseaccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnpurchaseaccount.AccountSubControlID);
                transectiontitle = SupplierName + " , Return Purchase Payment is Succeed!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);


                string paymentquery = string.Format("insert into tblSupplierReturnPayment(SupplierID,SupplierInvoiceID,UserID,InvoiceNo,TotalAmount,PaymentAmount,RemainingBalance,CompanyID,BranchID,SupplierReturnInvoiceID,InvoiceDate) " +
                "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                SupplierID, SupplierInvoiceID, UserID, InvoiceNo, TotalAmount, Amount, Convert.ToString(RemainingBalance), CompanyID, BranchID, SupplierReturnInvoiceID,DateTime.Now.ToString("yyyy/MM/dd"));
                DatabaseQuery.Insert(paymentquery);
            
                foreach (DataRow entryrow in dtEntries.Rows)
                {
                    string entryquery = string.Format("insert into tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID,InvoiceNo, UserID,Credit,Debit,TransectionDate,TransectionTitle,CompanyID,BranchID) values " +
                 " ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')",
                 Convert.ToString(entryrow[0]), Convert.ToString(entryrow[1]), Convert.ToString(entryrow[2]), Convert.ToString(entryrow[3]), Convert.ToString(entryrow[4]), Convert.ToString(entryrow[5]), Convert.ToString(entryrow[6]), Convert.ToString(entryrow[7]), Convert.ToDateTime(Convert.ToString(entryrow[8])).ToString("yyyy/MM/dd HH:mm"), Convert.ToString(entryrow[9]), CompanyID, BranchID);
                    DatabaseQuery.Insert(entryquery);
                }

                return "Return Purchase Payment is Paid.";
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
