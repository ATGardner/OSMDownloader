namespace DatabaseTest
{
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Data.SQLite;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            Task.WaitAll(TestDatabase(true), TestDatabase(false));
        }

        private static async Task TestDatabase(bool sqLite)
        {
            Console.WriteLine("Testing database, sqLite: {0}", sqLite);
            using (var connection = CreateConnection(sqLite))
            {
                connection.Open();
                var task = ExecuteNonQueryAsync(connection);
                Console.WriteLine("2");
                await task;
                Console.WriteLine("4");
            }
        }

        private static DbConnection CreateConnection(bool sqLite)
        {
            return sqLite ?
                (DbConnection)new SQLiteConnection(string.Format("Data Source=:memory:;")) :
                new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\DatabaseTest.mdf;Integrated Security=True;Connect Timeout=30");
        }

        private static async Task ExecuteNonQueryAsync(DbConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE test (col1 integer);";
            Console.WriteLine("1");
            await command.ExecuteNonQueryAsync();
            Console.WriteLine("3");
        }
    }
}
