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
using System.Data.SQLite;
using System.IO;
using System.Configuration;

namespace TLCViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static SQLiteConnection GetSQLiteConnection()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return new SQLiteConnection("Data Source=" + config.AppSettings.Settings["DatabasePath"].Value);
        }

        public MainWindow()
        {
            InitializeComponent();


        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 初期設定を行う
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings.Count <= 0)
            {
                config.AppSettings.Settings.Add("DatabasePath", "./database.db");
                config.Save();
            }
        }
    }
}
