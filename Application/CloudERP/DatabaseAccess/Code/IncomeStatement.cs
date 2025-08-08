using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code
{
    public class IncomeStatement
    {
        SP_BalanceSheet income = new SP_BalanceSheet();
        public IncomeStatementModel GetIncomeStatement(int CompanyID, int BranchID, int FinancialYearID)
        {

            var incomestatement = new IncomeStatementModel();
            incomestatement.incomeStatementHeads = new List<IncomeStatementHead>();
            incomestatement.Title = "Net Income";
            var revenue = income.GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, 5);
            var revenuedetials = new IncomeStatementHead();
            revenuedetials.Title = "Total Revenue";
            revenuedetials.TotalAmount = Math.Abs(revenue.TotalAmount);
            revenuedetials.AccountHead = revenue;
            incomestatement.incomeStatementHeads.Add(revenuedetials);

            var expenses = income.GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, 3);
            var expensesdetials = new IncomeStatementHead();
            expensesdetials.Title = "Total Expenses";
            expensesdetials.TotalAmount = Math.Abs(expenses.TotalAmount);
            expensesdetials.AccountHead = expenses;
            incomestatement.incomeStatementHeads.Add(expensesdetials);
            incomestatement.NetIncome = Math.Abs(revenuedetials.TotalAmount) - Math.Abs(expensesdetials.TotalAmount);

            return incomestatement;

        }
    }


   
}
