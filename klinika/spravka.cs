using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace klinika
{
    public partial class spravka : Form
    {
        public spravka()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != "")
            {
                OleDbConnection connection = new OleDbConnection();
                connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=klinika.accdb";
                OleDbCommand command = new OleDbCommand();
                command.CommandText = "SELECT opis, nazv FROM info WHERE nazv Like '%" + textBox1.Text + "%'";
                command.Connection = connection;
                try
                {
                    connection.Open();
                    OleDbDataReader dr = command.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            richTextBox1.Text = Convert.ToString(dr["nazv"]) + "\r\n";
                            richTextBox1.Text += Convert.ToString(dr["opis"]);
                            break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Такого анализа нет.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка получения данных: " + Environment.NewLine + ex.ToString());
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Введите название анализа.");
            }
            
        }
    }
}
