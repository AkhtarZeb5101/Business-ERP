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
    public class SP_Purchase
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        public List<PurchasePaymentModel> ReamaningPaymentList(int CompanyID, int BranchID)
        {
            var remainingpaymentlist = new List<PurchasePaymentModel>();
            SqlCommand command = new SqlCommand("GetSupplierRemainingPaymentRecord", DatabaseQuery.ConnOpen());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.Parameters.AddWithValue("@CompanyID", CompanyID);
            var dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int supplierid = Convert.ToInt32(Convert.ToString(row[4]));
                var supplier = db.tblSuppliers.Find(supplierid);

                var payement = new PurchasePaymentModel();

                payement.SupplierInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
                payement.BranchID = Convert.ToInt32(Convert.ToString(row[1]));
                payement.CompanyID = Convert.ToInt32(Convert.ToString(row[2]));
                payement.InvoiceDate = Convert.ToDateTime(Convert.ToString(row[3]));
                payement.InvoiceNo = Convert.ToString(row[5]);

                double totalamount = 0;
                double.TryParse(Convert.ToString(row[6]), out totalamount);

                double returntotalamount = 0;
                double.TryParse(Convert.ToString(row[7]), out returntotalamount);

                double afterreturntotalamount = 0;
                double.TryParse(Convert.ToString(row[8]), out afterreturntotalamount);


                double payamount = 0;
                double.TryParse(Convert.ToString(row[9]), out payamount);

                double returnpayamount = 0;
                double.TryParse(Convert.ToString(row[10]), out returnpayamount);

                double remainingbalance = 0;
                double.TryParse(Convert.ToString(row[11]), out remainingbalance);





                payement.PaymentAmount = payamount;
                payement.RemainingBalance = remainingbalance;
                payement.SupplierContactNo = supplier.SupplierConatctNo;
                payement.SupplierAddress = supplier.SupplierAddress;
                payement.SupplierID = supplier.SupplierID;
                payement.SupplierName = supplier.SupplierName;
                payement.TotalAmount = totalamount;

                payement.ReturnProductAmount = returntotalamount;
                payement.ReturnPaymentAmount = returnpayamount;
                payement.AfterReturnTotalAmount = afterreturntotalamount;
                remainingpaymentlist.Add(payement);
            }


            return remainingpaymentlist;
        }

        public List<PurchasePaymentModel> CustomPurchaseList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate)
        {
            var remainingpaymentlist = new List<PurchasePaymentModel>();
            SqlCommand command = new SqlCommand("GetPurchasesHistory", DatabaseQuery.ConnOpen());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.Parameters.AddWithValue("@CompanyID", CompanyID);
            command.Parameters.AddWithValue("@FromDate", FromDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@ToDate", ToDate.ToString("yyyy-MM-dd"));
            var dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int supplierid = Convert.ToInt32(Convert.ToString(row[4]));
                var supplier = db.tblSuppliers.Find(supplierid);

                var payement = new PurchasePaymentModel();

                payement.SupplierInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
                payement.BranchID = Convert.ToInt32(Convert.ToString(row[1]));
                payement.CompanyID = Convert.ToInt32(Convert.ToString(row[2]));
                payement.InvoiceDate = Convert.ToDateTime(Convert.ToString(row[3]));
                payement.InvoiceNo = Convert.ToString(row[5]);

                double totalamount = 0;
                double.TryParse(Convert.ToString(row[6]), out totalamount);

                double returntotalamount = 0;
                double.TryParse(Convert.ToString(row[7]), out returntotalamount);

                double afterreturntotalamount = 0;
                double.TryParse(Convert.ToString(row[8]), out afterreturntotalamount);


                double payamount = 0;
                double.TryParse(Convert.ToString(row[9]), out payamount);

                double returnpayamount = 0;
                double.TryParse(Convert.ToString(row[10]), out returnpayamount);

                double remainingbalance = 0;
                double.TryParse(Convert.ToString(row[11]), out remainingbalance);


                


                payement.PaymentAmount = payamount;
                payement.RemainingBalance = remainingbalance;
                payement.SupplierContactNo = supplier.SupplierConatctNo;
                payement.SupplierAddress = supplier.SupplierAddress;
                payement.SupplierID = supplier.SupplierID;
                payement.SupplierName = supplier.SupplierName;
                payement.TotalAmount = totalamount;

                payement.ReturnProductAmount = returntotalamount;
                payement.ReturnPaymentAmount = returnpayamount;
                payement.AfterReturnTotalAmount = afterreturntotalamount;
                remainingpaymentlist.Add(payement);
            }


            return remainingpaymentlist;
        }


        public List<PurchasePaymentModel> PurchasePaymentHistory(int SupplierInvoiceID)
        {
            var remainingpaymentlist = new List<PurchasePaymentModel>();
            SqlCommand command = new SqlCommand("GetSupplierPaymentHistory", DatabaseQuery.ConnOpen());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@SupplierInvoiceID", SupplierInvoiceID);
            var dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int supplierid = Convert.ToInt32(Convert.ToString(row[4]));
                int userid = Convert.ToInt32(Convert.ToString(row[9]));
                var supplier = db.tblSuppliers.Find(supplierid);
                var user = db.tblUsers.Find(userid);

                var payement = new PurchasePaymentModel();

                payement.SupplierInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
                payement.BranchID = Convert.ToInt32(Convert.ToString(row[1]));
                payement.CompanyID = Convert.ToInt32(Convert.ToString(row[2]));
                payement.InvoiceDate = Convert.ToDateTime(Convert.ToString(row[3]));
                payement.InvoiceNo = Convert.ToString(row[5]);
                double payamount = 0;
                double.TryParse(Convert.ToString(row[7]), out payamount);

                double remainingbalance = 0;
                double.TryParse(Convert.ToString(row[8]), out remainingbalance);


                double totalamount = 0;
                double.TryParse(Convert.ToString(row[6]), out totalamount);


                payement.PaymentAmount = payamount;
                payement.RemainingBalance = remainingbalance;
                payement.SupplierContactNo = supplier.SupplierConatctNo;
                payement.SupplierAddress = supplier.SupplierAddress;
                payement.SupplierID = supplier.SupplierID;
                payement.SupplierName = supplier.SupplierName;
                payement.TotalAmount = totalamount;
                payement.UserID = user.UserID;
                payement.UserName = user.UserName;
                remainingpaymentlist.Add(payement);
            }


            return remainingpaymentlist;
        }



        public List<SupplierReturnInvoiceModel> PurchaseReturnPaymentPending(int? SupplierInvoiceID)
        {
            var remainingpaymentlist = new List<SupplierReturnInvoiceModel>();
            SqlCommand command = new SqlCommand("GetSupplierReturnPurchasePaymentPending", DatabaseQuery.ConnOpen());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@SupplierInvoiceID", (int)SupplierInvoiceID);
            var dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int supplierid = Convert.ToInt32(Convert.ToString(row[5]));
                int userid = Convert.ToInt32(Convert.ToString(row[10]));
                var supplier = db.tblSuppliers.Find(supplierid);
                var user = db.tblUsers.Find(userid);

                var payement = new SupplierReturnInvoiceModel();

                payement.SupplierReturnInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
                payement.SupplierInvoiceID = Convert.ToInt32(Convert.ToString(row[1]));
                payement.BranchID = Convert.ToInt32(Convert.ToString(row[2]));
                payement.CompanyID = Convert.ToInt32(Convert.ToString(row[3]));
                payement.InvoiceDate = Convert.ToDateTime(Convert.ToString(row[4]));
                payement.InvoiceNo = Convert.ToString(row[6]);
                double payamount = 0;
                double.TryParse(Convert.ToString(row[8]), out payamount);
                double remainingbalance = 0;
                double.TryParse(Convert.ToString(row[9]), out remainingbalance);
                double totalamount = 0;
                double.TryParse(Convert.ToString(row[7]), out totalamount);
                payement.ReturnPaymentAmount = payamount;
                payement.RemainingBalance = remainingbalance;
                payement.SupplierContactNo = supplier.SupplierConatctNo;
                payement.SupplierAddress = supplier.SupplierAddress;
                payement.SupplierID = supplier.SupplierID;
                payement.SupplierName = supplier.SupplierName;
                payement.ReturnTotalAmount = totalamount;
                payement.UserID = user.UserID;
                payement.UserName = user.UserName;
                remainingpaymentlist.Add(payement);
            }
            return remainingpaymentlist;
        }

        public List<PurchasePaymentModel> GetReturnPurchasesPaymentPending(int CompanyID, int BranchID)
        {
            var remainingpaymentlist = new List<PurchasePaymentModel>();
            SqlCommand command = new SqlCommand("GetReturnPurchasePaymentPending", DatabaseQuery.ConnOpen());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.Parameters.AddWithValue("@CompanyID", CompanyID);
            var dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int supplierid = Convert.ToInt32(Convert.ToString(row[4]));
                var supplier = db.tblSuppliers.Find(supplierid);

                var payement = new PurchasePaymentModel();

                payement.SupplierInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
                payement.BranchID = Convert.ToInt32(Convert.ToString(row[1]));
                payement.CompanyID = Convert.ToInt32(Convert.ToString(row[2]));
                payement.InvoiceDate = Convert.ToDateTime(Convert.ToString(row[3]));
                payement.InvoiceNo = Convert.ToString(row[5]);

                double totalamount = 0;
                double.TryParse(Convert.ToString(row[6]), out totalamount);

                double returntotalamount = 0;
                double.TryParse(Convert.ToString(row[7]), out returntotalamount);

                double afterreturntotalamount = 0;
                double.TryParse(Convert.ToString(row[8]), out afterreturntotalamount);


                double payamount = 0;
                double.TryParse(Convert.ToString(row[9]), out payamount);

                double returnpayamount = 0;
                double.TryParse(Convert.ToString(row[10]), out returnpayamount);

                double remainingbalance = 0;
                double.TryParse(Convert.ToString(row[11]), out remainingbalance);





                payement.PaymentAmount = payamount;
                payement.RemainingBalance = remainingbalance;
                payement.SupplierContactNo = supplier.SupplierConatctNo;
                payement.SupplierAddress = supplier.SupplierAddress;
                payement.SupplierID = supplier.SupplierID;
                payement.SupplierName = supplier.SupplierName;
                payement.TotalAmount = totalamount;

                payement.ReturnProductAmount = returntotalamount;
                payement.ReturnPaymentAmount = returnpayamount;
                payement.AfterReturnTotalAmount = afterreturntotalamount;
                remainingpaymentlist.Add(payement);
            }
            return remainingpaymentlist;
        }
    }
}
