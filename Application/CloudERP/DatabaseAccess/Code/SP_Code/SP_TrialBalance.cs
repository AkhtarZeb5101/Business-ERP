using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code.SP_Code
{
   public class SP_TrialBalance
    {
        public List<TrialBalanceModel> TrialBalance(int BranchID,int CompanyID, int FinancialYearID)
        {
            var trialBalance = new List<TrialBalanceModel>();
            SqlCommand command = new SqlCommand("GetTrialBalance", DatabaseQuery.ConnOpen());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.Parameters.AddWithValue("@CompanyID", CompanyID);
            command.Parameters.AddWithValue("@FinancialYearID", FinancialYearID);
            var dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dt);
            double totaldebit = 0;
            double totalcredit = 0;
            foreach (DataRow row in dt.Rows)
            {
                var balance = new TrialBalanceModel();
                balance.FinancialYearID = Convert.ToInt32(Convert.ToString(row[0]));
                balance.AccountSubControl = Convert.ToString(row[1]);
                balance.AccountSubControlID = Convert.ToInt32(row[2]);
                balance.Debit = Convert.ToDouble(row[3] == DBNull.Value ? 0 : row[3]);
                balance.Credit = Convert.ToDouble(row[4] == DBNull.Value ? 0 : row[4]);
                balance.BranchID = Convert.ToInt32(row[5]);
                balance.CompanyID = Convert.ToInt32(row[6]);
                totaldebit = totaldebit + Convert.ToDouble(row[3] == DBNull.Value ? 0 : row[3]);
                totalcredit = totalcredit + Convert.ToDouble(row[4] == DBNull.Value ? 0 : row[4]);
                if (balance.Debit > 0 || balance.Credit > 0)
                {
                    trialBalance.Add(balance);
                }
            }

            var totalbalance = new TrialBalanceModel();
            totalbalance.Credit = totalcredit;
            totalbalance.Debit = totaldebit;
            totalbalance.AccountSubControl = "Total";
            trialBalance.Add(totalbalance);
            return trialBalance;
        }
    }
}
