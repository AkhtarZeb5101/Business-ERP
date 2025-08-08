using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace DatabaseAccess.Code.SP_Code
{
    public class SP_BalanceSheet
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();

        public BalanceSheetModel GetBalanceSheet(int CompanyID, int BranchID, int FinancialYearID, List<int> HeadIDs)
        {
            var BalanceSheet = new BalanceSheetModel();
            double TotalAssets = 0;
            double TotalLiabilities = 0;
            double TotalOwnerEquity = 0;
            double TotalReturnEarning = 0;
            // Return Earning Fields
            double TotalExpenses = 0;
            double TotalRevenue = 0;
            var AllHead = new List<AccountHeadTotal>();
            foreach (var HeadID in HeadIDs)
            {
                var AccountHead = new AccountHeadTotal();
                if (HeadID == 1 || HeadID == 2 || HeadID == 4)  // Total Assets
                {
                    AccountHead = GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, HeadID);
                    if (HeadID == 1)
                    {
                        TotalAssets = GetHeadAccountTotalAmount(CompanyID, BranchID, FinancialYearID, HeadID);
                    }
                    else if (HeadID == 2)
                    {
                        TotalLiabilities = GetHeadAccountTotalAmount(CompanyID, BranchID, FinancialYearID, HeadID);
                    }
                    else if (HeadID == 4)
                    {
                        TotalOwnerEquity = GetHeadAccountTotalAmount(CompanyID, BranchID, FinancialYearID, HeadID);
                    }
                    AllHead.Add(AccountHead);
                }
                else if (HeadID == 3) // Total Expenses
                {
                    AccountHead = GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, HeadID);
                    TotalExpenses = AccountHead.TotalAmount;

                }
                else if (HeadID == 5) // Total Revenue
                {
                    AccountHead = GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, HeadID);
                    TotalRevenue = AccountHead.TotalAmount;

                }
            }
            TotalReturnEarning = TotalRevenue - TotalExpenses;
            BalanceSheet.Title = "Total Balance";
            BalanceSheet.ReturnEarning = TotalReturnEarning;
            BalanceSheet.Total_Liabilites_OwnerEquity_ReturnEarning = TotalLiabilities + TotalOwnerEquity + TotalReturnEarning;
            BalanceSheet.TotalAssets = TotalAssets;
            BalanceSheet.AccountHeadTotals = AllHead;
            return BalanceSheet;
        }

        public double GetHeadAccountTotalAmount(int CompanyID, int BranchID, int FinancialYearID, int HeadID)
        {

            SqlCommand command = new SqlCommand("GETTotalBYHeadAccount", DatabaseQuery.ConnOpen());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.Parameters.AddWithValue("@CompanyID", CompanyID);
            command.Parameters.AddWithValue("@HeadID", HeadID);
            command.Parameters.AddWithValue("@FinancialYearID", FinancialYearID);
            var dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dt);

            double TotalAmount = 0;
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    TotalAmount = Convert.ToDouble(dt.Rows[0][0]);
                }
            }
            return TotalAmount;
        }

        public AccountHeadTotal GetHeadAccountsWithTotal(int CompanyID, int BranchID, int FinancialYearID, int HeadID)
        {
            var AccountsHeadWithDetails = new AccountHeadTotal();
            SqlCommand command = new SqlCommand("GetAccountHeadDetials", DatabaseQuery.ConnOpen());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.Parameters.AddWithValue("@CompanyID", CompanyID);
            command.Parameters.AddWithValue("@HeadID", HeadID);
            command.Parameters.AddWithValue("@FinancialYearID", FinancialYearID);
            var dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dt);
            var AccountsList = new List<AccountHeadDetail>();
            double TotalAmount = 0;
            foreach (DataRow row in dt.Rows)
            {
                var account = new AccountHeadDetail();
                account.AccountSubTitle = Convert.ToString(row[0].ToString());
                account.TotalAmount = Convert.ToDouble(row[1]);
                account.Status = Convert.ToString(row[2]);
                TotalAmount = TotalAmount + account.TotalAmount;
                if (account.TotalAmount > 0)
                {
                    AccountsList.Add(account);
                }
            }

            var accountHead = db.tblAccountHeads.Find(HeadID);
            AccountsHeadWithDetails.TotalAmount = TotalAmount;
            AccountsHeadWithDetails.AccountHeadTitle = accountHead.AccountHeadName;
            AccountsHeadWithDetails.AccountHeadDetails = AccountsList;

            return AccountsHeadWithDetails;
        }


    }
}
