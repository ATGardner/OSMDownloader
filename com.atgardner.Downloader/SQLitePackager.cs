using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace com.atgardner.Downloader
{
    class SQLitePackager : IDisposable
    {
        private static readonly String TABLE_DDL = "CREATE TABLE IF NOT EXISTS tiles (x int, y int, z int, s int, image blob, PRIMARY KEY (x,y,z,s))";
        private static readonly String INDEX_DDL = "CREATE INDEX IF NOT EXISTS IND on tiles (x,y,z,s)";
        private static readonly String INSERT_SQL = "INSERT or REPLACE INTO tiles (x,y,z,s,image) VALUES (?,?,?,0,?)";
        private static readonly String RMAPS_TABLE_INFO_DDL = "CREATE TABLE IF NOT EXISTS info AS SELECT 99 AS minzoom, 0 AS maxzoom";
        private static readonly String RMAPS_CLEAR_INFO_SQL = "DELETE FROM info;";
        private static readonly String RMAPS_UPDATE_INFO_MINMAX_SQL = "INSERT INTO info (minzoom,maxzoom) VALUES (?,?);";
        private static readonly String RMAPS_INFO_MAX_SQL = "SELECT DISTINCT z FROM tiles ORDER BY z DESC LIMIT 1;";
        private static readonly String RMAPS_INFO_MIN_SQL = "SELECT DISTINCT z FROM tiles ORDER BY z ASC LIMIT 1;";

        private readonly DbConnection connection;
        public SQLitePackager(string sourceFile)
        {
            var dbFile = Path.ChangeExtension(sourceFile, "sqlitedb");
            SQLiteConnection.CreateFile(dbFile);
            connection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
        }

        public void Init()
        {
            connection.Open();
            CreateTables();
        }

        public void AddTile(string tileFile)
        {
            using (var scope = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = TABLE_DDL;
                command.ExecuteNonQuery();
                command.CommandText = INDEX_DDL;
                command.ExecuteNonQuery();
                command.CommandText = RMAPS_TABLE_INFO_DDL;
                command.ExecuteNonQuery();

            }
        }

        private void CreateTables()
        {

        }

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Dispose();
            }
        }
    }
}
