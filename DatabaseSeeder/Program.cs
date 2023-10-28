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

            Console.WriteLine("プログラムが修了しました");
            Environment.Exit(0);
        }
    }
}
