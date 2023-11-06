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
using System.Text.RegularExpressions;

namespace TLCViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool initialized = false;
        Image[] thumbnails = new Image[0];
        Image[] pictures = new Image[0];

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

            // 窓の準備をする
            var item = new HorizontalStack()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            listingPictures.Children.Add(item);
            initialized = true;
        }

        private string GetDatabasePath()
        {
            // ネットワークドライブの場合、なぜかエスケープする必要がある
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var path = config.AppSettings.Settings["DatabasePath"].Value;
            if (Regex.IsMatch(path, "^//"))
            {
                path = "//" + path;
            }
            else if (Regex.IsMatch(path, "^\\\\"))
            {
                path = "\\\\" + path;
            }
            return path;
        }

        private void GetPictures()
        {
            // データベースを操作する
            using (var conn = new SQLiteConnection("Data Source=" + GetDatabasePath()))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {

                }
                conn.Close();
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!initialized) return;   // ValueChangedがWindowLoadedを呼び出す

            // 既にpicturesなどには値が代入されているものとして処理する

            // 行数を計算する
            int rows = 0;
            if (pictures.Length % (int)HorizontalSlider.Value > 0)
            {
                rows = pictures.Length / (int)HorizontalSlider.Value + 1;
            }
            else
            {
                rows = pictures.Length / (int)HorizontalSlider.Value;
            }

            listingPictures.Children.Clear();
            
            for (int i = 0; i < rows; ++i)
            {
                var item = new HorizontalStack(i, rows, (int)HorizontalSlider.Value, thumbnails, pictures);
                //var item = new HorizontalStack();
                listingPictures.Children.Add(item);
            }
        }
    }
}
