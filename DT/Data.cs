using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DT
{
    public class Data
    {
        public SqlConnection connection;
        public DataTable inputsTable = new DataTable();
              

        public SqlConnection GetSqlConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"DESKTOP-3Q54CAU\SQLEXPRESS";   // update me
            builder.UserID = "sa";              // update me
            builder.Password = "qazXcde3@1";      // update me
            builder.InitialCatalog = "desicion_tree_demo";

            connection = new SqlConnection(builder.ConnectionString);//сделать проверку на ошибки соединения
            connection.Open();//тут добавить сообщение об ошибках в какой-нить лог.
            //MessageBox.Show("Success!");
            return connection;
        }
    }
}
