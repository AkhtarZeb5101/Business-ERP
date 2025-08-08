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
    public class SP_Ledger
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        public List<AccountLedgerModel> GetLedger(int CompanyID, int BranchID, int FinancialYearID)
        {
            int sno = 0;
            var ledger = new List<AccountLedgerModel>();
            SqlCommand command = new SqlCommand("GetLedger", DatabaseQuery.ConnOpen());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.Parameters.AddWithValue("@CompanyID", CompanyID);
            command.Parameters.AddWithValue("@FinancialYearID", FinancialYearID);
            var journaldb = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(journaldb);
            if (journaldb == null)
            {
                ledger.Clear();
                return ledger;
            }
            if (journaldb.Rows.Count == 0)
            {
                ledger.Clear();
                return ledger;
            }

            decimal debit = 0;
            decimal credit = 0;
            int totalrecods = 0;
            string accountname = string.Empty;
            foreach (DataRow row in journaldb.Rows)
            {
                decimal ldebit = 0;
                decimal lcredit = 0;
                if (accountname == Convert.ToString(row[7]).Trim())  // Check Account Title
                {

                    var createrow = new AccountLedgerModel();
                    createrow.SNo = sno;
                    sno = sno + 1;
                    createrow.Date = Convert.ToString(row[9]); // TransetionDate
                    createrow.Description = Convert.ToString(row[10]); // TransectioTitle
                    decimal.TryParse(Convert.ToString(row[11]), out ldebit);
                    debit = debit + ldebit;
                    createrow.Debit = Convert.ToString(row[11]); // Debit
                    decimal.TryParse(Convert.ToString(row[12]), out lcredit);
                    credit = credit + lcredit;
                    createrow.Credit = Convert.ToString(row[12]); // Credit
                    ledger.Add(createrow);


                }
                else
                {

                    if (!string.IsNullOrEmpty(accountname))
                    {
                        var totalrow = new AccountLedgerModel();

                        totalrow.SNo = sno;
                        sno = sno + 1;
                        totalrow.Date = "Total"; // Account Control Name
                        if (credit >= debit)
                        {
                            totalrow.Credit = Convert.ToString(debit - credit).Replace('-', ' ');
                        }
                        else if (credit <= debit)
                        {
                            totalrow.Debit = Convert.ToString(debit - credit).Replace('-', ' ');
                        }
                        totalrow.Date = "Total Balance"; // Account Control Name
                        ledger.Add(totalrow);
                        debit = 0;
                        credit = 0;
                    }


                    var headerrow = new AccountLedgerModel();

                    headerrow.SNo = sno;
                    sno = sno + 1;
                    headerrow.Account = Convert.ToString(row[7]); // Account Control Name
                    headerrow.Date = "Date";
                    headerrow.Description = "Description";
                    headerrow.Debit = "Debit";
                    headerrow.Credit = "Credit";
                    ledger.Add(headerrow);


                    var createrow = new AccountLedgerModel();
                    createrow.SNo = sno;
                    sno = sno + 1;
                    createrow.Date = Convert.ToString(row[9]); // TransetionDate
                    createrow.Description = Convert.ToString(row[10]); // TransectioTitle
                    decimal.TryParse(Convert.ToString(row[11]), out ldebit);
                    debit = debit + ldebit;
                    createrow.Debit = Convert.ToString(row[11]); // Debit
                    decimal.TryParse(Convert.ToString(row[12]), out lcredit);
                    credit = credit + lcredit;
                    createrow.Credit = Convert.ToString(row[12]); // Credit
                    ledger.Add(createrow);
                    accountname = Convert.ToString(row[7]).Trim();

                }
                totalrecods = totalrecods + 1;
                if (totalrecods == journaldb.Rows.Count)
                {
                    var totalrow = new AccountLedgerModel();
                    totalrow.SNo = sno;
                    sno = sno + 1;
                    totalrow.Date = "Total"; // Account Control Name
                    if (credit >= debit)
                    {
                        totalrow.Credit = Convert.ToString(debit - credit).Replace('-', ' ');
                    }
                    else if (credit <= debit)
                    {
                        totalrow.Debit = Convert.ToString(debit - credit).Replace('-', ' ');
                    }
                    totalrow.Date = "Total Balance"; // Account Control Name
                    ledger.Add(totalrow);
                    debit = 0;
                    credit = 0;
                }

            }

            return ledger;
        }
    }
}
