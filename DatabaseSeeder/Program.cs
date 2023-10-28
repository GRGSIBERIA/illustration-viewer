using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;


namespace DatabaseSeeder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var conn = new SqlConnection("Data Source=mydb.sqlite");
            using (var command = conn.CreateCommand())
            {
                conn.Open();

                conn.Close();
            }


            conn.Close();
        }
    }
}
