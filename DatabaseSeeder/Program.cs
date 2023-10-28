using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Data.SQLite;

namespace DatabaseSeeder
{
    internal class Program
    {
        static void GenerateDatabase()
        {
            using (var conn = new SQLiteConnection("Data Source=mydb.sqlite"))
            {
                conn.Open();

                string sql = "";
                using (var reader = new StreamReader("./Sqls/create.sql"))
                {
                    sql = reader.ReadToEnd();
                }
                Console.WriteLine(sql);

                using (var command = new SQLiteCommand(sql, conn))
                {
                    command.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        static void Seeding()
        {
            Dictionary<string, int> map = new Dictionary<string, int>();

            Console.WriteLine("ファイル一覧を取得");
            for (int i = 0; i < 180; ++i)
            {
                var img_files = Directory.GetFiles($"D:\\img\\{i}\\", "*");
                foreach (var file in img_files)
                {
                    var basename = file.Split('.')[0].Split('\\').Last();
                    map[basename] = i;
                }
            }
            Console.WriteLine("ファイルをマップに変換");

            Console.WriteLine("CSVファイルを展開");
            using (var reader = new StreamReader("D:\\mongo.csv"))
            {
                /*
                 * [0] 1,
                 * [1] d34e4cf0a437a5d65f8e82b7bcd02606,
                 * [2] jpg,
                 * [3] 127238,
                 * [4] 459,
                 * [5] 650,
                 * [6] 1girl ;p animal_ears bangs blue_bow blue_panties blue_ribbon blush bow bow_panties breasts brown_eyes cat_ears cat_girl cat_tail collarbone cowboy_shot eyebrows eyebrows_visible_through_hair kemonomimi_mode kousaka_tamaki kyougoku_shin large_breasts long_hair long_sleeves looking_at_viewer no_pants one_eye_closed orange_background panties red_hair ribbon school_uniform serafuku smile solo standing striped striped_background striped_panties tail thigh_gap thighhighs thighs to_heart_2 tongue tongue_out underwear very_long_hair white_legwear,
                 * [7] /data/__kousaka_tamaki_to_heart_2_drawn_by_kyougoku_shin__d34e4cf0a437a5d65f8e82b7bcd02606.jpg
                 */

                var line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    var sep = line.Split(',');
                    var hash = sep[1];
                    var ext = sep[2];

                    // たまに存在しないファイルがある
                    if (!map.ContainsKey(hash)) continue;

                    var path = $"D:\\img\\{map[hash]}\\{hash}.{ext}";

                }
            }
            Console.WriteLine("ファイルの取得が完了しました");
        }

        static void DropDatabase()
        {
            using (var conn = new SQLiteConnection("Data Source=mydb.sqlite"))
            {
                conn.Open();

                string sql = "";
                using (var reader = new StreamReader("./Sqls/drop.sql"))
                {
                    sql = reader.ReadToEnd();
                }
                Console.WriteLine(sql);

                using (var command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch(System.Data.SQLite.SQLiteException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                conn.Close();
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("モードを切り替えます");
            Console.WriteLine("1. データベース作成");
            Console.WriteLine("2. シード");
            Console.WriteLine("3. データベースをdropする");
            var s = Console.ReadLine();
            
            switch (s)
            {
                case "1":
                    GenerateDatabase();
                    break;
                case "2":
                    Seeding();
                    break;
                case "3":
                    DropDatabase();
                    break;
                default:
                    Console.WriteLine("終了します");
                    break;
            }

            Console.WriteLine("プログラムが終了しました");
            Environment.Exit(0);
        }
    }
}
