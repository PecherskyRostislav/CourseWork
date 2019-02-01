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
    public partial class analiz : Form
    {
        public analiz()
        {
            InitializeComponent();           
        }
        //сюда сохраняем цены
        int[] price = new int[20];
        //выбор пользователя
        bool[,,] sohr_vibor = new bool[13,20,20];
        //храним тут общую сумму пользователя
        int sum;
        //тут мы храним номер выбранного коня и ветви
        int nom_kor, nom_vetki;
        //запоминаем мы добавили клиена или нашли страрого
        bool new_klient = false;
        //очищает или эдиты или выбранные анализы
        public void obnulenie(int per)
        {
            new_klient = false;
            if(per == 1)
            {
                sum = 0;
                for (int z = 0; z < 13; z++)
                    for (int i = 0; i < 20; i++)
                        for (int j = 0; j < 20; j++)
                            sohr_vibor[z, i, j] = false;
                label5.Text = Convert.ToString(sum);

                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, sohr_vibor[nom_kor, nom_vetki, i]);
                }
                listBox2.Items.Clear();
            }
            else
            {
                foreach(TextBox box in Controls.OfType<TextBox>())
                    {
                        box.Text = "";        
                    }
                comboBox1.SelectedIndex = -1;
            }
        }
        //вывод на экран нужных анализов
        public void vivod_analiz(int a, int b, int z, int z1) 
        {
            int i = 0;       
          
            OleDbConnection connection = new OleDbConnection();
            connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=klinika.accdb";
            OleDbCommand command = new OleDbCommand();
            command.CommandText = "SELECT info.nazv, price.price FROM info, price WHERE price.kod_analiza = info.kod AND (info.kod > " 
                + Convert.ToString(a) + " and " + Convert.ToString(b) + " > info.kod)";
            //command.CommandText = "SELECT info.nazv FROM info WHERE (info.kod > " + Convert.ToString(a) + " and " + Convert.ToString(b) + " > info.kod)";
            command.Connection = connection;
            try
            {
                connection.Open();
                OleDbDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        price[i] = Convert.ToInt32(dr["price"]);
                        listBox1.Items.Add(Convert.ToString(price[i]));
                        checkedListBox1.Items.Add(dr["nazv"]);
                        i++;
                    }
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

            nom_vetki = z1;
            nom_kor = z;

            i = 0;
            //выводин выбранные анализы
            for (i = 0; i < (b - a - 1); i++)
            {
                checkedListBox1.SetItemChecked(i, sohr_vibor[z, z1, i]);
            }
        }
        //при выборе в дереве
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            label2.Text = e.Node.Text;
            listBox1.Items.Clear();
            checkedListBox1.Items.Clear();
            if (e.Node.Text == treeView1.Nodes[0].Nodes[0].Text)
            {
                vivod_analiz(0, 6, 0, 0);
            }
            if (e.Node.Text == treeView1.Nodes[0].Nodes[1].Text)
            {
                vivod_analiz(5, 8, 0, 1);
            }
            if (e.Node.Text == treeView1.Nodes[0].Nodes[2].Text)
            {
                vivod_analiz(7, 16, 0, 2);
            }
        }

        //подсчитываем цену, выводим информацию о подготовке
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //подсчитываем цену
            listBox1.SelectedIndex = checkedListBox1.SelectedIndex;
            if (checkedListBox1.GetItemChecked(checkedListBox1.SelectedIndex))
            {
                sum += price[checkedListBox1.SelectedIndex];
                sohr_vibor[nom_kor, nom_vetki, checkedListBox1.SelectedIndex] = true;
                listBox2.Items.Add(checkedListBox1.Text);
            }
            else
            {
                sum -= price[checkedListBox1.SelectedIndex];
                sohr_vibor[nom_kor, nom_vetki, checkedListBox1.SelectedIndex] = false;
                listBox2.Items.Remove(checkedListBox1.Text);
            }                
            label5.Text = Convert.ToString(sum);
            //выводим информацию о подготовке
            OleDbConnection connection = new OleDbConnection();
            connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=klinika.accdb";
            OleDbCommand command = new OleDbCommand();
            command.CommandText = "SELECT prep FROM info WHERE nazv = '" + checkedListBox1.Text + "'";
            command.Connection = connection;
            try
            {
                connection.Open();
                OleDbDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                       textBox1.Text = Convert.ToString(dr["prep"]);
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
        //синхронизируем чеклист с лист
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkedListBox1.SelectedIndex = listBox1.SelectedIndex;
        }
        //при загрузки формы обнуляем и загружаем в комбобокс пользователей
        private void analiz_Load(object sender, EventArgs e)
        {
            obnulenie(1);

            OleDbConnection connection = new OleDbConnection();
            connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=klinika.accdb";
            OleDbCommand command = new OleDbCommand();
            command.CommandText = "SELECT surname FROM klienti";
            command.Connection = connection;
            try
            {
                connection.Open();
                OleDbDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        comboBox1.Items.Add(Convert.ToString(dr["surname"]));
                    }
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
        //кнопка очистки анализа
        private void button1_Click(object sender, EventArgs e)
        {
            obnulenie(1);
        }
        //кнопка очистки клиента
        private void button2_Click(object sender, EventArgs e)
        {
            obnulenie(2);
        }
        //выход
        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //открытие справки
        private void button3_Click(object sender, EventArgs e)
        {
            Form f = new spravka();
            f.Show();
        }
        //занесение в БД
        private void button5_Click(object sender, EventArgs e)
        {
            //в ид сохраняем ид пользователя
            //в к заносим проверку на заполненость
            // в тру проверку был выбран хоть 1 анализ
            int k = 0, tru = 0;
            string id, kod;
            //анализ
            for (int z = 0; z < 13; z++)
                for (int i = 0; i < 20; i++)
                    for (int j = 0; j < 20; j++)
                        if (sohr_vibor[z, i, j] == true)
                            tru++;
            //клиент
            foreach (TextBox box in Controls.OfType<TextBox>())
            {
                if (box.Text == "")
                    k++;
            }
            //занесение исли проверка прошла
            if(tru > 0)
            if(k == 0)
            {
                id = poisk_id_klienta();
                if (new_klient)
                {
                    id = poisk_id_klienta();
                }   
                //MessageBox.Show(listBox2.Items[0].ToString());
                for(int i = 0; i < listBox2.Items.Count; i++)
                {
                    kod = poisk_kod_analiza(listBox2.Items[i].ToString());
                    zapolnenie_analizi(id, kod, poisk_price_analiza(kod));
                }                
            }
            else
            {
                MessageBox.Show("Заполните все данные о пользователе!");
            }
            else
            {
                MessageBox.Show("Вы не выбрали ни одного анализа!");
            }
        }
        //функция для занесения инфы о пользователе
        public void dobavlenie()
        {
            OleDbConnection connection = new OleDbConnection();
            connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=klinika.accdb";
            OleDbCommand command = new OleDbCommand();
            command.CommandText = "INSERT INTO klienti(surname, first_name, second_name, data, adres, telefon) VALUES ('" + comboBox1.Text + "', '" + textBox2.Text +
                           "', '" + textBox3.Text + "', '" + textBox4.Text + "','" + textBox6.Text + "' , '" + textBox5.Text + "')";
            command.Connection = connection;
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
        //занесения нового пользователя или заполнение данных о уже существующем
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            OleDbConnection connection = new OleDbConnection();
            connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=klinika.accdb";
            OleDbCommand command = new OleDbCommand();
            command.CommandText = "SELECT * FROM klienti WHERE surname = '"+comboBox1.Text+"'";
            command.Connection = connection;
            try
            {
                connection.Open();
                OleDbDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        textBox2.Text = Convert.ToString(dr["first_name"]);
                        textBox3.Text = Convert.ToString(dr["second_name"]);
                        textBox4.Text = otrez(Convert.ToString(dr["data"]));
                        textBox5.Text = Convert.ToString(dr["telefon"]);
                        textBox6.Text = Convert.ToString(dr["adres"]);
                        break;
                    }
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
        //обрезает в дате время
        public static string otrez(string perem)
        {
            char[] preobr = (perem).ToCharArray();
            string chto = "";
            for (int j = 0; j < 10; j++)
            {
                chto += preobr[j];
            }
            return chto;
        }
        //функция поиска ид клинта
        public string poisk_id_klienta()
        {
            string id = "";
            OleDbConnection connection = new OleDbConnection();
            connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=klinika.accdb";
            OleDbCommand command = new OleDbCommand();
            command.CommandText = "SELECT id FROM klienti WHERE surname = '" + comboBox1.Text + "' and telefon = '" + textBox5.Text + "' ";
            command.Connection = connection;
            try
            {
                connection.Open();
                OleDbDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        id = Convert.ToString(dr["id"]);
                    }
                }
                else
                {
                    dobavlenie();
                    new_klient = true;
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

            return id;
        }
        //поиск кода анализа
        public string poisk_kod_analiza(string s)
        {
            string id = "";
            OleDbConnection connection = new OleDbConnection();
            connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=klinika.accdb";
            OleDbCommand command = new OleDbCommand();
            command.CommandText = "SELECT kod FROM info WHERE nazv = '" + s + "'";
            command.Connection = connection;
            try
            {
                connection.Open();
                OleDbDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        id = Convert.ToString(dr["kod"]);
                    }
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

            return id;
        }
        //поиск цены анализа
        public string poisk_price_analiza(string s)
        {
            string id = "";
            OleDbConnection connection = new OleDbConnection();
            connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=klinika.accdb";
            OleDbCommand command = new OleDbCommand();
            command.CommandText = "SELECT price FROM price WHERE kod_analiza = " + s;
            command.Connection = connection;
            try
            {
                connection.Open();
                OleDbDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        id = Convert.ToString(dr["price"]);
                    }
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

            return id;
        }
        public void zapolnenie_analizi(string id, string kod, string price)
        {
            OleDbConnection connection = new OleDbConnection();
            connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=klinika.accdb";
            OleDbCommand command = new OleDbCommand();
            command.CommandText = "INSERT INTO analizi(id, kod, data, price) VALUES ( " + id + ", " + kod + ", '" + DateTime.Now.ToShortDateString() + "', '" + price + "')";
            command.Connection = connection;
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
