using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.IO;
using System.Runtime.InteropServices;

namespace WpfApp1
{
    internal class Connect_to_db
    {
        public static MySqlConnection GetDBConnection(string host, string port, string database, string username, string password)
        {
            String connString = "Server=" + host + ";Database=" + database + ";port=" + port + ";User Id=" + username + ";password=" + password;
            MySqlConnection conn = new MySqlConnection(connString);
            return conn;
        }
    }
    class ConDB
    {
        public static MySqlConnection GetDBConnection()
        {
            INIManager manager = new INIManager(Path.Combine(Environment.CurrentDirectory, "connect.ini"));
            string host = manager.GetPrivateString("main", "ip");
            string port = manager.GetPrivateString("main", "port");
            string database = manager.GetPrivateString("main", "name_db");
            string username = manager.GetPrivateString("main", "profile");
            string password = manager.GetPrivateString("main", "pass");
            return Connect_to_db.GetDBConnection(host, port, database, username, password);
        }
    }
    public class INIManager
    {
        private const int SIZE = 1024;
        private string path = null;
        public INIManager(string aPath)
        {
            path = aPath;
        }
        public INIManager() : this("") { }
        public string GetPrivateString(string aSection, string aKey)
        {
            StringBuilder buffer = new StringBuilder(SIZE);
            GetPrivateString(aSection, aKey, null, buffer, SIZE, path);
            return buffer.ToString();
        }
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]
        private static extern int GetPrivateString(string section, string key, string def, StringBuilder buffer, int size, string path);
    }
}
