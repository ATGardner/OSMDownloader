namespace com.atgardner.OMFG.packagers
{
    using tiles;
    using utils;
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public abstract class SQLitePackager : IPackager
    {
        protected abstract string TABLE_DDL { get; }
        protected abstract string INDEX_DDL { get; }
        protected abstract string INSERT_SQL { get; }
        protected abstract string GetDbFileName(string fileName);
        protected abstract Task UpdateTileMetaInfo();
        public abstract Task AddTile(Tile tile);

        private readonly string METADATA_DDL = "CREATE TABLE IF NOT EXISTS android_metadata (locale TEXT)";
        private readonly string METADATA_SELECT = "SELECT count(*) FROM android_metadata";
        private readonly string METADATA_INSERT = "INSERT INTO android_metadata VALUES (@locale)";
        protected readonly Database database;

        public SQLitePackager(string fileName)
        {
            var dbFile = GetDbFileName(fileName);
            database = new Database(dbFile);
        }

        public async Task Init()
        {
            database.Open();
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
                case FormatType.MBTiles:
                    return new MBTilesPackager(fileName);
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

            if (database != null)
            {
                await UpdateTileMetaInfo();
                database.Dispose();
            }
        }

        private async Task CreateTables()
        {
            await database.ExecuteNonQueryAsync(TABLE_DDL);
            await database.ExecuteNonQueryAsync(INDEX_DDL);
            await UpdateLocale();
        }

        private async Task UpdateLocale()
        {
            await database.ExecuteNonQueryAsync(METADATA_DDL);
            var count = await database.ExecuteScalarAsync(METADATA_SELECT);
            if ((long)count == 0)
            {
                await database.ExecuteNonQueryAsync(METADATA_INSERT, new Dictionary<string, object> {
                    { "locale", CultureInfo.CurrentCulture.Name }
                });
            }
        }
    }
}
