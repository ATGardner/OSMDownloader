using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace com.atgardner.Downloader
{
    public class SQLitePackager : IDisposable
    {
        private static readonly String TABLE_DDL = "CREATE TABLE IF NOT EXISTS tiles (x int, y int, z int, s int, image blob, PRIMARY KEY (x,y,z,s))";
        private static readonly String INDEX_DDL = "CREATE INDEX IF NOT EXISTS IND on tiles (x,y,z,s)";
        private static readonly String INSERT_SQL = "INSERT or REPLACE INTO tiles (x,y,z,s,image) VALUES (?,?,?,0,?)";
        private static readonly String RMAPS_TABLE_INFO_DDL = "CREATE TABLE IF NOT EXISTS info AS SELECT 99 AS minzoom, 0 AS maxzoom";
        private static readonly String RMAPS_CLEAR_INFO_SQL = "DELETE FROM info;";
        private static readonly String RMAPS_UPDATE_INFO_MINMAX_SQL = "INSERT INTO info (minzoom,maxzoom) VALUES (?,?);";
        private static readonly String RMAPS_INFO_MAX_SQL = "SELECT DISTINCT z FROM tiles ORDER BY z DESC LIMIT 1;";
        private static readonly String RMAPS_INFO_MIN_SQL = "SELECT DISTINCT z FROM tiles ORDER BY z ASC LIMIT 1;";
        private static readonly string METADATA_DDL = "CREATE TABLE IF NOT EXISTS android_metadata (locale TEXT)";
        private static readonly string METADATA_SELECT = "SELECT count(*) FROM android_metadata";
        private static readonly string METADATA_INSERT = "INSERT INTO android_metadata VALUES (@locale)";

        private readonly DbConnection connection;
        public SQLitePackager(string sourceFile)
        {
            var dbFile = Path.ChangeExtension(Path.GetFileNameWithoutExtension(sourceFile), "sqlitedb");
            //dbFile = "a.sqlitedb";
            //if (!File.Exists(dbFile))
            //{
            //    SQLiteConnection.CreateFile(dbFile);
            //}

            connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", dbFile));
        }

        public Task Init()
        {
            connection.Open();
            return CreateTables();
        }

        public void AddTile(string tileFile)
        {
            
        }

        private async Task CreateTables()
        {
            using (var scope = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = TABLE_DDL;
                await command.ExecuteNonQueryAsync();
                command.CommandText = INDEX_DDL;
                await command.ExecuteNonQueryAsync();
                command.CommandText = RMAPS_TABLE_INFO_DDL;
                await command.ExecuteNonQueryAsync();
                await UpdateLocale(command);
                scope.Commit();
            }
        }

        private static async Task UpdateLocale(DbCommand command)
        {
            command.CommandText = METADATA_DDL;
            await command.ExecuteNonQueryAsync();
            command.CommandText = METADATA_SELECT;
            var count = await command.ExecuteScalarAsync();
            if ((long)count == 0)
            {
                command.CommandText = METADATA_INSERT;
                var param = command.CreateParameter();
                param.DbType = DbType.String;
                param.ParameterName = "locale";
                param.Value = CultureInfo.CurrentCulture.Name;
                command.Parameters.Add(param);
                await command.ExecuteNonQueryAsync();
                command.Parameters.Clear();
            }
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
