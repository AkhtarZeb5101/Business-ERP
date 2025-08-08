using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code
{
   public class SaleEntry
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        public string selectcustomerid = string.Empty;
        DataTable dtEntries = null;
        public string ConfrimSale(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, float Amount, string CustomerID, string Customername, bool ispayment)
        {
            try
            {
                dtEntries = null;
                string saletitle = "Sale to " + Customername.Trim();
                var financialyearcheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = (financialyearcheck != null ? Convert.ToString(financialyearcheck.Rows[0][0]) : string.Empty);
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
                }
                string successmessage = "Sale Success";

                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;
                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)
                var SaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 1 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                // Credit Entry Sale
                AccountHeadID = Convert.ToString(SaleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(SaleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(SaleAccount.AccountSubControlID);
                string transectiontitle = string.Empty;
                transectiontitle = "Sale to " + Customername.Trim();
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0",  DateTime.Now, transectiontitle);


                // Debit Entry Sale
                SaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 10 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(SaleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(SaleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(SaleAccount.AccountSubControlID);
                transectiontitle = Customername + " , Sale Payment is Pending!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(),  "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);


                if (ispayment == true)
                {
                    string payinvoicenno = "INP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                    SaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 10 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    AccountHeadID = Convert.ToString(SaleAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(SaleAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(SaleAccount.AccountSubControlID);
                    transectiontitle = "Sale Payment Paid By " + Customername;
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, payinvoicenno, UserID.ToString(), Convert.ToString(Amount),"0", DateTime.Now, transectiontitle);


                    SaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 11 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    AccountHeadID = Convert.ToString(SaleAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(SaleAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(SaleAccount.AccountSubControlID);
                    transectiontitle = Customername + " , Sale Payment is Succeed!";
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, payinvoicenno, UserID.ToString(), "0",Convert.ToString(Amount), DateTime.Now, transectiontitle);


                    string paymentquery = string.Format("insert into tblCustomerPayment(CustomerID,CustomerInvoiceID,UserID,invoiceNo,TotalAmount,PaidAmount,RemainingBalance,CompanyID,BranchID,InvoiceDate) " +
                    " values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                    CustomerID, CustomerInvoiceID, UserID, payinvoicenno, Amount, Amount, "0", CompanyID, BranchID, DateTime.Now.ToString("yyyy/MM/dd"));
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


        public string SalePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, float TotalAmount, float Amount, string CustomerID, string Customername, float RemainingBalance)
        {
            try
            {
                dtEntries = null;
                string saletitle = "Sale to " + Customername.Trim();
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

                var SaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 10 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(SaleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(SaleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(SaleAccount.AccountSubControlID);
                transectiontitle = "Sale Payment Paid By " + Customername;
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);


                SaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 11 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(SaleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(SaleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(SaleAccount.AccountSubControlID);
                transectiontitle = Customername + " , Sale Payment is Succeed!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);


                string paymentquery = string.Format("insert into tblCustomerPayment(CustomerID,CustomerInvoiceID,UserID,invoiceNo,TotalAmount,PaidAmount,RemainingBalance,CompanyID,BranchID,InvoiceDate) " +
                " values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                CustomerID, CustomerInvoiceID, UserID, InvoiceNo, TotalAmount, Amount, Convert.ToString(RemainingBalance), CompanyID, BranchID,DateTime.Now.ToString("yyyy/MM/dd"));
                DatabaseQuery.Insert(paymentquery);

                foreach (DataRow entryrow in dtEntries.Rows)
                {
                    string entryquery = string.Format("insert into tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID,InvoiceNo, UserID,Credit,Debit,TransectionDate,TransectionTitle,CompanyID,BranchID) values " +
                 " ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')",
                 Convert.ToString(entryrow[0]), Convert.ToString(entryrow[1]), Convert.ToString(entryrow[2]), Convert.ToString(entryrow[3]), Convert.ToString(entryrow[4]), Convert.ToString(entryrow[5]), Convert.ToString(entryrow[6]), Convert.ToString(entryrow[7]), Convert.ToDateTime(Convert.ToString(entryrow[8])).ToString("yyyy/MM/dd HH:mm"), Convert.ToString(entryrow[9]), CompanyID, BranchID);
                    DatabaseQuery.Insert(entryquery);
                }

                return "Paid Successfully.";
            }
            catch (Exception ex)
            {

                return "Unexpected Error is Occur Plz Try Again!";
            }
        }


        // Sale Return Code
        public string ReturnSale(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, int CustomerReturnInvoiceID, float Amount, string CustomerID, string Customername, bool ispayment)
        {
            try
            {
                dtEntries = null;
                string returnsaletitle = "Return Sale from " + Customername.Trim();
                var financialyearcheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = (financialyearcheck != null ? Convert.ToString(financialyearcheck.Rows[0][0]) : string.Empty);
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
                }
                string successmessage = "Return Sale Success";

                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;
                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)
                var ReturnSaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 2 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                // Debit Entry Return Sale
                AccountHeadID = Convert.ToString(ReturnSaleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(ReturnSaleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(ReturnSaleAccount.AccountSubControlID);
                string transectiontitle = string.Empty;
                transectiontitle = "Return Sale From " + Customername.Trim();
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(),  "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);


                // Credit Entry Return Sale
                ReturnSaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 14 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(ReturnSaleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(ReturnSaleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(ReturnSaleAccount.AccountSubControlID);
                transectiontitle = Customername + " , Return Sale Payment is Pending!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);


                if (ispayment == true)
                {
                    string payinvoicenno = "RIP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                    ReturnSaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 14 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    AccountHeadID = Convert.ToString(ReturnSaleAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(ReturnSaleAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(ReturnSaleAccount.AccountSubControlID);
                    transectiontitle = "Return Sale Payment Paid to " + Customername;
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, payinvoicenno, UserID.ToString(),  "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);


                    ReturnSaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 15 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    AccountHeadID = Convert.ToString(ReturnSaleAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(ReturnSaleAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(ReturnSaleAccount.AccountSubControlID);
                    transectiontitle = Customername + " , Return Sale Payment is Succeed!";
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, payinvoicenno, UserID.ToString(), Convert.ToString(Amount), "0",  DateTime.Now, transectiontitle);


                    string paymentquery = string.Format("insert into tblCustomerReturnPayment(CustomerID,CustomerInvoiceID,UserID,InvoiceNo,TotalAmount,PaidAmount,RemainingBalance,CompanyID,BranchID,CustomerReturnInvoiceID,InvoiceDate) " +
                    " values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                    CustomerID, CustomerInvoiceID, UserID, payinvoicenno, Amount, Amount, "0", CompanyID, BranchID, CustomerReturnInvoiceID,DateTime.Now.ToString("yyyy/MM/dd"));
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


        public string ReturnSalePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, int CustomerReturnInvoiceID, float TotalAmount, float Amount, string CustomerID, string Customername, float RemainingBalance)
        {
            try
            {
                dtEntries = null;
                string saletitle = "Return Sale From " + Customername.Trim();
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

                var ReturnSaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 14 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(ReturnSaleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(ReturnSaleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(ReturnSaleAccount.AccountSubControlID);
                transectiontitle = "Return Sale Payment Paid to " + Customername;
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);


                ReturnSaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 15 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(ReturnSaleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(ReturnSaleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(ReturnSaleAccount.AccountSubControlID);
                transectiontitle = Customername + " , Return Sale Payment is Succeed!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);


                string paymentquery = string.Format("insert into tblCustomerReturnPayment(CustomerID,CustomerInvoiceID,UserID,InvoiceNo,TotalAmount,PaidAmount,RemainingBalance,CompanyID,BranchID,CustomerReturnInvoiceID,InvoiceDate) " +
                " values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                CustomerID, CustomerInvoiceID, UserID, InvoiceNo, TotalAmount, Amount, Convert.ToString(RemainingBalance), CompanyID, BranchID, CustomerReturnInvoiceID,DateTime.Now.ToString("yyyy/MM/dd"));
                DatabaseQuery.Insert(paymentquery);

                foreach (DataRow entryrow in dtEntries.Rows)
                {
                    string entryquery = string.Format("insert into tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID,InvoiceNo, UserID,Credit,Debit,TransectionDate,TransectionTitle,CompanyID,BranchID) values " +
                 " ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')",
                 Convert.ToString(entryrow[0]), Convert.ToString(entryrow[1]), Convert.ToString(entryrow[2]), Convert.ToString(entryrow[3]), Convert.ToString(entryrow[4]), Convert.ToString(entryrow[5]), Convert.ToString(entryrow[6]), Convert.ToString(entryrow[7]), Convert.ToDateTime(Convert.ToString(entryrow[8])).ToString("yyyy/MM/dd HH:mm"), Convert.ToString(entryrow[9]), CompanyID, BranchID);
                    DatabaseQuery.Insert(entryquery);
                }

                return "Paid Successfully.";
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
