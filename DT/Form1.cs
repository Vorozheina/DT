using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


using System.Drawing.Drawing2D;
using System.Drawing.Text;



namespace DT
{
    

    public partial class Form1 : Form
    {
        public DesicionTreeDemoEntities demoEntities;

        public Tree<CircleNode> tree = new Tree<CircleNode>();

        //public SqlConnection connection;
        public DataTable inputsTable = new DataTable();


        public Form1()
        {
            InitializeComponent();
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"DESKTOP-3Q54CAU\SQLEXPRESS";   // update me
            builder.UserID = "sa";              // update me
            builder.Password = "qazXcde3@1";      // update me
            builder.InitialCatalog = "desicion_tree_demo";

            Global.Connection = new SqlConnection(builder.ConnectionString);//сделать проверку на ошибки соединения
            
            Global.Connection.Open();//тут добавить сообщение об ошибках в какой-нить лог.

            

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT * FROM Inputs", Global.Connection);//этот блок выполняется только при успешном соединении с бд
            sqlDataAdapter.Fill(Global.InputsTable);
            Global.InputsTable.Columns.Remove("id");

            


            Global.RichTextBox = richTextBox1;

            List<Attribute> attributes = new List<Attribute>();
            foreach (DataColumn dc in Global.InputsTable.Columns)
            {
                if (dc.ColumnName != "Result")
                {
                    List<string> atValues = new List<string>();
                    foreach(DataRow dataRow in dc.Table.Rows)
                    {
                        atValues.Add(dataRow[dc.ColumnName].ToString());
                    }
                    Attribute at = new Attribute(dc.ColumnName, atValues.Distinct().ToList());
                    attributes.Add(at);
                }
            }

            /*обход дерева и печать.
            Tree<string> tree = new Tree<string>();
            tree.Root = tree.create_id3_tree(Global.InputsTable, attributes, "");
            tree.TraverseDFS();
            */

            tree.Root = tree.create_id3_tree(Global.InputsTable, attributes, "");
            //*/
            ArrangeTree();

            DataView dataView = new DataView(Global.InputsTable);

            dataGridView1.DataSource = dataView;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            tree.Root.DrawTree(e.Graphics);
        }

        private void ArrangeTree()
        {
            using (Graphics gr = pictureBox1.CreateGraphics())
            {
                // Arrange the tree once to see how big it is.
                float xmin = 0, ymin = 0;
                tree.Root.Arrange(gr, ref xmin, ref ymin);

                // Arrange the tree again to center it horizontally.
                xmin = (this.ClientSize.Width - xmin) / 2;
                ymin = 10;
                tree.Root.Arrange(gr, ref xmin, ref ymin);
            }

            pictureBox1.Refresh();
        }


    }
}
