using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;


namespace DatabaseSeeder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var conn = new SQLiteConnection("Data Source=mydb.sqlite"))
            {
                conn.Open();

                conn.Close();
            }

            Console.WriteLine("プログラムが修了しました");
            Console.ReadLine();
        }
    }
}
