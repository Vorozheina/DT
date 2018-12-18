using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace DT
{
    public static class Global
    {
        private static SqlConnection _connection;
        private static DataTable _inputsTable = new DataTable();
        private static RichTextBox _richTextBox = new RichTextBox();


        public static SqlConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        public static DataTable InputsTable
        {
            get { return _inputsTable; }
            set { _inputsTable = value; }
        }

        public static RichTextBox RichTextBox
        {
            get { return _richTextBox; }
            set { _richTextBox = value; }
        }
    }
}
