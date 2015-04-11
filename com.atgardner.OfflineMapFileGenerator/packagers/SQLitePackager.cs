namespace com.atgardner.OMFG.packagers
{
    using com.atgardner.OMFG.tiles;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class SQLitePackager : IPackager
    {
        protected abstract string TABLE_DDL { get; }
        protected abstract string INDEX_DDL { get; }
        protected abstract string INSERT_SQL { get; }
        protected abstract string GetDbFileName(string fileName);
        protected abstract Task UpdateTileMetaInfo();
        public abstract Task AddTile(Tile tile);

        private string METADATA_DDL = "CREATE TABLE IF NOT EXISTS android_metadata (locale TEXT)";
        private string METADATA_SELECT = "SELECT count(*) FROM android_metadata";
        private string METADATA_INSERT = "INSERT INTO android_metadata VALUES (@locale)";

        protected DbConnection Connection { get; private set; }

        public SQLitePackager(string fileName)
        {
            var dbFile = GetDbFileName(fileName);
            var directoryName = Path.GetDirectoryName(dbFile);
            Directory.CreateDirectory(directoryName);
            Connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", dbFile));
        }

        public async Task Init()
        {
            Connection.Open();
            await CreateTables();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static IPackager GetPackager(FormatType type, string fileName, Map map)
        {
            switch (type)
            {
                case FormatType.BCNav:
                    return new BCNavPackager(fileName);
                case FormatType.OruxMaps:
                    return new OruxPackager(fileName, map);
                default:
                    throw new ArgumentException("Type must be either BCNav or OruxMaps", "type");
            }
        }

        protected virtual async void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (Connection != null)
            {
                await UpdateTileMetaInfo();
                Connection.Dispose();
            }
        }

        private async Task CreateTables()
        {
            using (var scope = Connection.BeginTransaction())
            {
                var command = Connection.CreateCommand();
                command.CommandText = TABLE_DDL;
                await command.ExecuteNonQueryAsync();
                command.CommandText = INDEX_DDL;
                await command.ExecuteNonQueryAsync();
                await UpdateLocale(command);
                scope.Commit();
            }
        }

        private async Task UpdateLocale(DbCommand command)
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

        protected static void AddParameter(DbCommand command, DbType type, string name, object value)
        {
            var param = command.CreateParameter();
            param.DbType = type;
            param.ParameterName = name;
            param.Value = value;
            command.Parameters.Add(param);
        }
    }
}
