using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Data;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MySqlConnection conn = ConDB.GetDBConnection();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProcessLog();
            Listboxes();
        }
        private void ProcessLog()
        {
            process_log_dg.ItemsSource = null;
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("select id_process, process_name, category_name, owning_division_name from process_log, `process`, category, owning_division where process_n = id_process and process_c = id_category and process_od = id_owning_division;", conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            process_log_dg.ItemsSource = table.DefaultView;
            conn.Close();
        }
        private void Listboxes()
        {
            conn.Open();
            MySqlCommand process_name = new MySqlCommand("SELECT process_name FROM zadanie.process;", conn);
            MySqlDataReader read_pn = process_name.ExecuteReader();
            while (read_pn.Read())
            {
                process_name_cb.Items.Add(read_pn.GetValue(0).ToString());
            }
            conn.Close();

            conn.Open();
            MySqlCommand category_name = new MySqlCommand("SELECT category_name FROM zadanie.category;", conn);
            MySqlDataReader read_cn = category_name.ExecuteReader();
            while (read_cn.Read())
            {
                category_name_cb.Items.Add(read_cn.GetValue(0).ToString());
            }
            conn.Close();

            conn.Open();
            MySqlCommand owning_division = new MySqlCommand("SELECT owning_division_name FROM zadanie.owning_division;", conn);
            MySqlDataReader read_od = owning_division.ExecuteReader();
            while (read_od.Read())
            {
                owning_division_cb.Items.Add(read_od.GetValue(0).ToString());
            }
            conn.Close();
        }
        private void reset_Click(object sender, RoutedEventArgs e)
        {
            ProcessLog();
            process_name_cb.Text = "";
            category_name_cb.Text = "";
            owning_division_cb.Text = "";
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            string process = "", category = "", ow_div = "";
            string select_command = "";

            if (process_name_cb.Text.Trim() != "")
            {
                process = process_name_cb.Text;
                select_command += String.Format(" and process_name = '{0}'", process);
            }
            if (category_name_cb.Text.Trim() != "")
            {
                category = category_name_cb.Text;
                select_command += String.Format(" and category_name = '{0}'", category);
            }
            if (owning_division_cb.Text.Trim() != "")
            {
                ow_div = owning_division_cb.Text;
                select_command += String.Format(" and owning_division_name = '{0}'", ow_div);
            }

            process_log_dg.ItemsSource = null;
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("select id_process, process_name, category_name, owning_division_name from process_log, `process`, category, owning_division where process_n = id_process and process_c = id_category and process_od = id_owning_division" + select_command + ";", conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            process_log_dg.ItemsSource = table.DefaultView;
            conn.Close();
        }
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
