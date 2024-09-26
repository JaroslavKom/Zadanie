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
using System.Data;
using Microsoft.Win32;
using Mysqlx;
using System.IO;
using System.Text;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Подлючение к базе данных
        MySqlConnection conn = ConDB.GetDBConnection();
        //Массивы для работы с загруженной таблицей
        string[,] category_mass = new string[0, 2];
        string[,] process_mass = new string[0, 2];
        string[,] owning_div_mass = new string[0, 2];

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProcessLog();
            Listboxes();
            LoadMassive();
        }
        //Отображение таблицы из базы данных
        private void ProcessLog()
        {
            process_log_dg.ItemsSource = null;
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("select id_process, process_name, category_name, owning_division_name from process_log, `process`, category, owning_division where process_n = id_process and process_c = id_category and process_od = id_owning_division;", conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable table_view = new DataTable();
            adapter.Fill(table_view);
            process_log_dg.ItemsSource = table_view.DefaultView;
            conn.Close();
        }
        //Выпадающие списки для поиска записей в таблице
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
        //Массивы для последующей работы с загруженной таблицей
        private void LoadMassive()
        {
            int c = 0, p = 0, od = 0;

            conn.Open();
            MySqlCommand process_cmd = new MySqlCommand("SELECT id_process, process_name FROM zadanie.process;", conn);
            MySqlDataReader read_p = process_cmd.ExecuteReader();
            while (read_p.Read())
            {
                ResizeArray(ref process_mass, ++p, 2);
                for (int i = 0; i < 2; i++)
                {
                    process_mass[p - 1, i] = read_p.GetValue(i).ToString();
                }
            }
            conn.Close();

            conn.Open();
            MySqlCommand category_cmd = new MySqlCommand("SELECT id_category, category_name FROM zadanie.category;", conn);
            MySqlDataReader read_c = category_cmd.ExecuteReader();
            while (read_c.Read())
            {
                ResizeArray(ref category_mass, ++c, 2);
                for (int i = 0; i < 2; i++)
                {
                    category_mass[c - 1, i] = read_c.GetValue(i).ToString();
                }
            }
            conn.Close();

            conn.Open();
            MySqlCommand owning_division_cmd = new MySqlCommand("SELECT id_owning_division, owning_division_name FROM zadanie.owning_division;", conn);
            MySqlDataReader read_od = owning_division_cmd.ExecuteReader();
            while (read_od.Read())
            {
                ResizeArray(ref owning_div_mass, ++od, 2);
                for (int i = 0; i < 2; i++)
                {
                    owning_div_mass[od - 1, i] = read_od.GetValue(i).ToString();
                }
            }
            conn.Close();
        }
        //Метод для изменения размерности массива
        private void ResizeArray<T>(ref T[,] array, int size1, int size2)
        {
            T[,] new_array = new T[size1, size2];
            size1 = Math.Min(array.GetLength(0), size1);
            size2 = Math.Min(array.GetLength(1), size2);
            for (int i = 0; i < size1; i++)
            {
                for (int j = 0; j < size2; j++) new_array[i, j] = array[i, j];
            }
            array = new_array;
        }
        //Кнопка сброса поиска
        private void reset_Click(object sender, RoutedEventArgs e)
        {
            ProcessLog();
            process_name_cb.Text = "";
            category_name_cb.Text = "";
            owning_division_cb.Text = "";
        }
        //Кнопка поиска по выбранным параметрам
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
        //Кнопка выхода из программы
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //Кнопка загрузки таблицы в базу данных
        private void add_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog.Title = "Выберите выгруженный файл в формате .csv";

            if (openFileDialog.ShowDialog() == true)
            {
                DataTable csvData = ReadCSVFile(openFileDialog.FileName);
                ConvertToUTF8(csvData);
                UpdateDataTable(csvData);
                InsertDB(csvData);
            }
        }
        //Чтение таблицы
        private DataTable ReadCSVFile(string filePath)
        {
            var datatable = new DataTable();
            using (StreamReader reader = new StreamReader(filePath, Encoding.GetEncoding("windows-1251")))
            {
                var headers = reader.ReadLine().Split(';');
                foreach (var header in headers)
                {
                    datatable.Columns.Add(header);
                }

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');
                    datatable.Rows.Add(values);
                }
            }
            return datatable;
        }
        //Конвертация кодировки
        private DataTable ConvertToUTF8(DataTable datatable)
        {
            foreach (DataRow row in datatable.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (row[i] is string)
                    {
                        byte[] win1251 = Encoding.GetEncoding("windows-1251").GetBytes((string)row[i]);
                        byte[] utf8 = Encoding.Convert(Encoding.GetEncoding("windows-1251"), Encoding.UTF8, win1251);
                        row[i] = Encoding.UTF8.GetString(utf8);
                    }
                }
            }
            return datatable;
        }
        //Изменение таблицы в подходящий для переноса в базу данных вид
        private DataTable UpdateDataTable(DataTable dataTable)
        {
            int a = 0;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < category_mass.GetLength(0); j++)
                {
                    if (dataTable.Rows[i][0].ToString() == category_mass[j, 1].ToString())
                    {
                        dataTable.Rows[i][0] = category_mass[j, 0].ToString();
                        a++;
                    }
                }
                if (a == 0)
                {
                    conn.Open();
                    MySqlCommand cmd_newcategory = new MySqlCommand(String.Format("insert into category (category_name) values ('{0}');", dataTable.Rows[i][0]), conn);
                    cmd_newcategory.ExecuteNonQuery();
                    conn.Close();
                    conn.Open();
                    MySqlCommand category_cmd = new MySqlCommand(String.Format("SELECT id_category FROM zadanie.category where category_name = '{0}';", dataTable.Rows[i][0]), conn);
                    MySqlDataReader read_c = category_cmd.ExecuteReader();
                    while (read_c.Read())
                    {
                        dataTable.Rows[i][0] = read_c["id_category"].ToString();
                    }
                    conn.Close();
                }
                else
                    a = 0;

                for (int j = 0; j < process_mass.GetLength(0); j++)
                {
                    if (dataTable.Rows[i][1].ToString() == process_mass[j, 0].ToString())
                    {
                        a++;
                    }
                }
                if (a == 0)
                {
                    conn.Open();
                    MySqlCommand cmd_newprocess = new MySqlCommand(String.Format("insert into `process` (id_process,process_name) values ('{0}','{1}');", dataTable.Rows[i][1], dataTable.Rows[i][2]), conn);
                    cmd_newprocess.ExecuteNonQuery();
                    conn.Close();
                }
                else
                    a = 0;


                for (int j = 0; j < owning_div_mass.GetLength(0); j++)
                {
                    if (dataTable.Rows[i][3].ToString() == owning_div_mass[j, 1].ToString())
                    {
                        dataTable.Rows[i][3] = owning_div_mass[j, 0].ToString();
                        a++;
                    }
                }
                if (a == 0)
                {
                    conn.Open();
                    MySqlCommand cmd_newowningdivision = new MySqlCommand(String.Format("insert into owning_division (owning_division_name) values ('{0}');", dataTable.Rows[i][3]), conn);
                    cmd_newowningdivision.ExecuteNonQuery();
                    conn.Close();
                    conn.Open();
                    MySqlCommand owning_division_cmd = new MySqlCommand(String.Format("SELECT id_owning_division FROM owning_division where owning_division_name = '{0}';", dataTable.Rows[i][3]), conn);
                    MySqlDataReader read_od = owning_division_cmd.ExecuteReader();
                    while (read_od.Read())
                    {
                        dataTable.Rows[i][3] = read_od["id_owning_division"].ToString();
                    }
                    conn.Close();
                }
                else
                    a = 0;

                LoadMassive();
            }
            return dataTable;
        }
        //Добавление таблицы в базу данных
        private void InsertDB(DataTable datatable)
        {
            foreach (DataRow row in datatable.Rows)
            {
                conn.Open();
                MySqlCommand cmd_newlog = new MySqlCommand(String.Format("insert into process_log (process_n, process_c, process_od) values('{0}',{1},{2});", row[1], row[0], row[3]), conn);
                cmd_newlog.ExecuteNonQuery();
                conn.Close();
            }
            ProcessLog();
            Listboxes();
            MessageBox.Show("Таблица успешно загружена в базу данных");
        }

    }
}
