using GINS_Task.Models;
using GINS_Task.Utility;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata;

namespace GINS_Task.DAL
{
    public class MeetingDetailsDAL
    {
        string CNN = ConnectionString.CNN;
        public IEnumerable<Customer> GetCustomerName(string type)
        {
            List<Customer> customers = new List<Customer>();

            using (SqlConnection cnn = new SqlConnection(CNN))
            {
                SqlCommand sqlCommand = new SqlCommand("SPGetCustomerName", cnn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Type", type);
                cnn.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Customer customer = new Customer();

                    customer.CustomerName = reader["CustomerName"].ToString();

                    customers.Add(customer);
                }
                cnn.Close();
            }

            return customers;
        }

        public IEnumerable<ProductService> GetProductService()
        {
            List<ProductService> products = new List<ProductService>();

            using (SqlConnection cnn = new SqlConnection(CNN))
            {
                SqlCommand sqlCommand = new SqlCommand("SPGetProductService", cnn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                cnn.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    ProductService product = new ProductService();

                    product.Product = reader["ProductService"].ToString();
                    product.Unit = (int)reader["Unit"];

                    products.Add(product);
                }
                cnn.Close();
            }

            return products;
        }

        public int SaveMeetingMinutes(MeetingMinutesViewModel vModel)
        {
            int meetingMinuteID;

            using (SqlConnection cnn = new SqlConnection(CNN))
            {
                SqlCommand sqlCommand = new SqlCommand("Meeting_Minutes_Master_Save_SP", cnn);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                // Add parameters based on your stored procedure parameters
                sqlCommand.Parameters.Add(new SqlParameter("@CustomerType", SqlDbType.NVarChar, 100) { Value = vModel.CustomerType });
                sqlCommand.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.NVarChar, 255) { Value = vModel.CustomerName });
                sqlCommand.Parameters.Add(new SqlParameter("@MeetingDate", SqlDbType.DateTime) { Value = vModel.MeetingDate });
                sqlCommand.Parameters.Add(new SqlParameter("@MeetingPlace", SqlDbType.NVarChar, 150) { Value = vModel.MeetingPlace });
                sqlCommand.Parameters.Add(new SqlParameter("@AttendsClient", SqlDbType.NVarChar, -1) { Value = vModel.AttendsClient });
                sqlCommand.Parameters.Add(new SqlParameter("@AttendsHost", SqlDbType.NVarChar, -1) { Value = vModel.AttendsHost });
                sqlCommand.Parameters.Add(new SqlParameter("@MeetingAgenda", SqlDbType.NVarChar, 255) { Value = vModel.MeetingAgenda });
                sqlCommand.Parameters.Add(new SqlParameter("@MeetingDiscussion", SqlDbType.NVarChar, -1) { Value = vModel.MeetingDiscussion });
                sqlCommand.Parameters.Add(new SqlParameter("@MeetingDecision", SqlDbType.NVarChar, -1) { Value = vModel.MeetingDecision });

                // Output parameter to get the inserted ID
                sqlCommand.Parameters.Add(new SqlParameter("@OutputID", SqlDbType.Int) { Direction = ParameterDirection.Output });

                cnn.Open();
                sqlCommand.ExecuteNonQuery();

                if (sqlCommand.Parameters["@OutputID"].Value != DBNull.Value)
                {
                   meetingMinuteID = Convert.ToInt32(sqlCommand.Parameters["@OutputID"].Value);
                }
                else
                {
                    // Handle the case where the value is DBNull.Value (e.g., set a default value or throw an exception)
                    throw new InvalidOperationException("OutputID parameter has DBNull.Value");
                }
                    

                cnn.Close();

                return meetingMinuteID;
            }
        }

        public void InsertMeetingMinutesProducts(int meetingMinutesID, List<MeetingMunitesProduct> products)
        {
            // Convert the list to a DataTable
            DataTable details = ConvertToDataTable(products);

            using (SqlConnection cnn = new SqlConnection(CNN))
            {
                using (SqlCommand sqlCommand = new SqlCommand("Meeting_Minutes_Details_Save_SP", cnn))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    // Add TVP parameter
                    SqlParameter tvpParam = sqlCommand.Parameters.AddWithValue("@Details", details);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.MeetingMinutesDetailsType";

                    // Add other parameters
                    sqlCommand.Parameters.AddWithValue("@MeetingMinutesID", meetingMinutesID);

                    cnn.Open();
                    sqlCommand.ExecuteNonQuery();
                }


            }

        }
        // Helper method to convert List<MeetingMunitesProduct> to DataTable
        private DataTable ConvertToDataTable(List<MeetingMunitesProduct> products)
        {
            DataTable dataTable = new DataTable();

            // Add columns
            dataTable.Columns.Add("ProductService", typeof(string));
            dataTable.Columns.Add("Quantity", typeof(int));
            dataTable.Columns.Add("Unit", typeof(int));

            // Add rows
            foreach (var product in products)
            {
                dataTable.Rows.Add(product.ProductService, product.Quantity, product.Unit);
            }

            return dataTable;
        }
    }
}
