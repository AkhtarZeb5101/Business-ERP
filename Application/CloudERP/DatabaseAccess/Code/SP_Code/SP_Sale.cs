using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace DatabaseAccess.Code.SP_Code
{
    public class SP_Sale
    {
        private CloudErpV1Entities db = new CloudErpV1Entities();
        public List<SalePaymentModel> ReamaningPaymentList(int CompanyID, int BranchID)
        {
            var remainingpaymentlist = new List<SalePaymentModel>();
            SqlCommand command = new SqlCommand("GetCustomerRemainingPaymentRecord", DatabaseQuery.ConnOpen());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.Parameters.AddWithValue("@CompanyID", CompanyID);
            var dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int customerID = Convert.ToInt32(Convert.ToString(row[4]));
                var customer = db.tblCustomers.Find(customerID);

                var payement = new SalePaymentModel();

                payement.CustomerInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
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
                payement.CustomerContactNo = customer.CustomerContact;
                payement.CustomerAddress = customer.CustomerAddress;
                payement.CustomerID = customer.CustomerID;
                payement.CustomerName = customer.Customername;
                payement.TotalAmount = totalamount;
                remainingpaymentlist.Add(payement);
            }

            return remainingpaymentlist;
        }

        public List<SalePaymentModel> CustomSalesList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate)
        {
            var remainingpaymentlist = new List<SalePaymentModel>();
            SqlCommand command = new SqlCommand("GetSalesHistory", DatabaseQuery.ConnOpen());
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
                int customerID = Convert.ToInt32(Convert.ToString(row[4]));
                var customer = db.tblCustomers.Find(customerID);

                var payement = new SalePaymentModel();
                payement.CustomerInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
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
                payement.CustomerContactNo = customer.CustomerContact;
                payement.CustomerAddress = customer.CustomerAddress;
                payement.CustomerID = customer.CustomerID;
                payement.CustomerName = customer.Customername;
                payement.TotalAmount = totalamount;
                payement.ReturnProductAmount = returntotalamount;
                payement.ReturnPaymentAmount = returnpayamount;
                payement.AfterReturnTotalAmount = afterreturntotalamount;
                remainingpaymentlist.Add(payement);

            }

            return remainingpaymentlist;
        }

        public List<SalePaymentModel> SalePaymentHistory(int CustomerInvoiceID)
        {
            var remainingpaymentlist = new List<SalePaymentModel>();
            SqlCommand command = new SqlCommand("GetCustomerPaymentHistory", DatabaseQuery.ConnOpen());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CustomerInvoiceID", CustomerInvoiceID);
            var dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int customerid = Convert.ToInt32(Convert.ToString(row[4]));
                int userid = Convert.ToInt32(Convert.ToString(row[9]));
                var customer = db.tblCustomers.Find(customerid);
                var user = db.tblUsers.Find(userid);

                var payement = new SalePaymentModel();

                payement.CustomerInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
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
                payement.CustomerContactNo = customer.CustomerContact;
                payement.CustomerAddress = customer.CustomerAddress;
                payement.CustomerID = customer.CustomerID;
                payement.CustomerName = customer.Customername;
                payement.TotalAmount = totalamount;
                payement.UserID = user.UserID;
                payement.UserName = user.UserName;
                remainingpaymentlist.Add(payement);
            }


            return remainingpaymentlist;
        }


        public List<SalePaymentModel> GetReturnSaleAmountPending(int CompanyID, int BranchID)
        {
            var remainingpaymentlist = new List<SalePaymentModel>();
            SqlCommand command = new SqlCommand("GetReturnSaleAmountPending", DatabaseQuery.ConnOpen());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.Parameters.AddWithValue("@CompanyID", CompanyID);
            var dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int customerID = Convert.ToInt32(Convert.ToString(row[4]));
                var customer = db.tblCustomers.Find(customerID);

                var payement = new SalePaymentModel();
                payement.CustomerInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
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
                payement.CustomerContactNo = customer.CustomerContact;
                payement.CustomerAddress = customer.CustomerAddress;
                payement.CustomerID = customer.CustomerID;
                payement.CustomerName = customer.Customername;
                payement.TotalAmount = totalamount;
                payement.ReturnProductAmount = returntotalamount;
                payement.ReturnPaymentAmount = returnpayamount;
                payement.AfterReturnTotalAmount = afterreturntotalamount;
                remainingpaymentlist.Add(payement);
            }
            return remainingpaymentlist;
        }


        public List<CustomerReturnInvoiceModel> SalesReturnAmountPending(int? CustomerInvoiceID)
        {
            var remainingpaymentlist = new List<CustomerReturnInvoiceModel>();
            SqlCommand command = new SqlCommand("GetCustomerReturnSalePaidPending", DatabaseQuery.ConnOpen());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CustomerInvoiceID", (int)CustomerInvoiceID);
            var dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int customerid = Convert.ToInt32(Convert.ToString(row[5]));
                int userid = Convert.ToInt32(Convert.ToString(row[10]));
                var customer = db.tblCustomers.Find(customerid);
                var user = db.tblUsers.Find(userid);

                var payement = new CustomerReturnInvoiceModel();

                payement.CustomerReturnInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
                payement.CustomerInvoiceID = Convert.ToInt32(Convert.ToString(row[1]));
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
                payement.CustomerContactNo = customer.CustomerContact;
                payement.CustomerAddress = customer.CustomerAddress;
                payement.CustomerID = customer.CustomerID;
                payement.CustomerName = customer.Customername;
                payement.ReturnTotalAmount = totalamount;
                payement.UserID = user.UserID;
                payement.UserName = user.UserName;
                remainingpaymentlist.Add(payement);
            }
            return remainingpaymentlist;
        }
    }
}
