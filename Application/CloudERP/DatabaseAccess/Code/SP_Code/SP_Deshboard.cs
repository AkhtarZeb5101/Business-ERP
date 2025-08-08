using DatabeAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code.SP_Code
{
    public class SP_Deshboard
    {
        public static DeshboardModel Get_DeshBoardHeader(int BranchID, int CompanyID)
        {
            SqlCommand command = new SqlCommand("GETDashboardValues", DatabaseQuery.ConnOpen());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.Parameters.AddWithValue("@CompanyID", CompanyID);
            var dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dt);

            var deshboard = new DeshboardModel();
            deshboard.CurrentMonthExpenses = Convert.ToDouble(dt.Rows[0][0].ToString());
            deshboard.NetIncome = Convert.ToDouble(dt.Rows[0][1].ToString());
            deshboard.CashBankBalance = Convert.ToDouble(dt.Rows[0][3].ToString());
            deshboard.TotalReceivable = Convert.ToDouble(dt.Rows[0][4].ToString());
            deshboard.TotalPayable = Convert.ToDouble(dt.Rows[0][5].ToString());
            deshboard.Capital = Convert.ToDouble(dt.Rows[0][2].ToString());
            deshboard.CurrentMonthRevenue = Convert.ToDouble(dt.Rows[0][6].ToString());

            deshboard.CurrentMonthSale = Convert.ToDouble(dt.Rows[0][7].ToString());
            deshboard.CurrentMonthSalePaymentSucceed = Convert.ToDouble(dt.Rows[0][8].ToString());
            deshboard.CurrentMonthSalePaymentPending = Convert.ToDouble(dt.Rows[0][9].ToString());
            deshboard.CurrentMonthReturnSale = Convert.ToDouble(dt.Rows[0][10].ToString());
            deshboard.CurrentMonthReturnSalePaymentPending = Convert.ToDouble(dt.Rows[0][11].ToString());
            deshboard.CurrentMonthReturnSalePaymentSucceed = Convert.ToDouble(dt.Rows[0][12].ToString());

            deshboard.CurrentMonthPurchase = Convert.ToDouble(dt.Rows[0][13].ToString());
            deshboard.CurrentMonthPurchasePaidPayment = Convert.ToDouble(dt.Rows[0][14].ToString());
            deshboard.CurrentMonthPurchaseRemainingPayment = Convert.ToDouble(dt.Rows[0][15].ToString());
            deshboard.CurrentMonthReturnPurchase = Convert.ToDouble(dt.Rows[0][16].ToString());
            deshboard.CurrentMonthReturnPurchasePaymentPending = Convert.ToDouble(dt.Rows[0][17].ToString());
            deshboard.CurrentMonthReturnPurchasePaymentSucceed = Convert.ToDouble(dt.Rows[0][18].ToString());

            deshboard.DaySale = Convert.ToDouble(dt.Rows[0][19].ToString());
            deshboard.DaySalePaymentSucceed = Convert.ToDouble(dt.Rows[0][20].ToString());
            deshboard.DaySalePaymentPending = Convert.ToDouble(dt.Rows[0][21].ToString());
            deshboard.DayReturnSale = Convert.ToDouble(dt.Rows[0][22].ToString());
            deshboard.DayReturnSalePaymentPending = Convert.ToDouble(dt.Rows[0][23].ToString());
            deshboard.DayReturnSalePaymentSucceed = Convert.ToDouble(dt.Rows[0][24].ToString());
            deshboard.DayPurchase = Convert.ToDouble(dt.Rows[0][25].ToString());
            deshboard.DayPurchasePaidPayment = Convert.ToDouble(dt.Rows[0][26].ToString());
            deshboard.DayPurchaseRemainingPayment = Convert.ToDouble(dt.Rows[0][27].ToString());
            deshboard.DayReturnPurchase = Convert.ToDouble(dt.Rows[0][28].ToString());
            deshboard.DayReturnPurchasePaymentPending = Convert.ToDouble(dt.Rows[0][29].ToString());
            deshboard.DayReturnPurchasePaymentSucceed = Convert.ToDouble(dt.Rows[0][30].ToString());
            return deshboard;
        }
    }
}
