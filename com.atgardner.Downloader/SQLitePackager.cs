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
        private static readonly string TABLE_DDL = "CREATE TABLE IF NOT EXISTS tiles (x int, y int, z int, s int, image blob, PRIMARY KEY (x,y,z,s))";
        private static readonly string INDEX_DDL = "CREATE INDEX IF NOT EXISTS IND on tiles (x, y, z, s)";
        private static readonly string INSERT_SQL = "INSERT or REPLACE INTO tiles (x, y, z, s, image) VALUES (@x, @y, @z, 0, @image)";
        private static readonly string RMAPS_TABLE_INFO_DDL = "CREATE TABLE IF NOT EXISTS info AS SELECT 99 AS minzoom, 0 AS maxzoom";
        private static readonly string RMAPS_CLEAR_INFO_SQL = "DELETE FROM info;";
        private static readonly string RMAPS_UPDATE_INFO_MINMAX_SQL = "insert into info(minzoom, maxzoom) values((select min(z) from tiles), (select max(z) from tiles));";
        private static readonly string METADATA_DDL = "CREATE TABLE IF NOT EXISTS android_metadata (locale TEXT)";
        private static readonly string METADATA_SELECT = "SELECT count(*) FROM android_metadata";
        private static readonly string METADATA_INSERT = "INSERT INTO android_metadata VALUES (@locale)";

        private readonly DbConnection connection;

        public SQLitePackager(string sourceFile)
        {
            var dbFile = Path.ChangeExtension(Path.GetFileNameWithoutExtension(sourceFile), "sqlitedb");
            connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", dbFile));
        }

        public Task Init()
        {
            connection.Open();
            return CreateTables();
        }

        public async void AddTile(Tile tile, string tileFile)
        {
            var command = connection.CreateCommand();
            command.CommandText = INSERT_SQL;
            AddParameter(command, DbType.UInt32, "x", tile.X);
            AddParameter(command, DbType.UInt32, "y", tile.Y);
            AddParameter(command, DbType.UInt32, "z", tile.Zoom);
            AddParameter(command, DbType.Binary, "image", File.ReadAllBytes(tileFile));
            await command.ExecuteNonQueryAsync();
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
                AddParameter(command, DbType.String, "locale", CultureInfo.CurrentCulture.Name);
                await command.ExecuteNonQueryAsync();
                command.Parameters.Clear();
            }
        }

        private async void UpdateTileMetaInfo()
        {
            using (var scope = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = RMAPS_CLEAR_INFO_SQL;
                await command.ExecuteNonQueryAsync();
                command.CommandText = RMAPS_UPDATE_INFO_MINMAX_SQL;
                await command.ExecuteNonQueryAsync();
                scope.Commit();
            }
        }

        private static void AddParameter(DbCommand command, DbType type, string name, object value)
        {
            var param = command.CreateParameter();
            param.DbType = type;
            param.ParameterName = name;
            param.Value = value;
            command.Parameters.Add(param);
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
