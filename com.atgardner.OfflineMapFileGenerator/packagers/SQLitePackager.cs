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
        protected abstract Task UpdateTileMetaInfoAsync();
        public abstract Task AddTileAsync(Tile tile, byte[] data);

        private readonly string METADATA_DDL = "CREATE TABLE IF NOT EXISTS android_metadata (locale TEXT)";
        private readonly string METADATA_SELECT = "SELECT count(*) FROM android_metadata";
        private readonly string METADATA_INSERT = "INSERT INTO android_metadata VALUES (@locale)";
        protected readonly Database database;
        protected readonly string attribution;

        public SQLitePackager(string fileName, string attribution)
        {
            var dbFile = GetDbFileName(fileName);
            database = new Database(dbFile);
            this.attribution = attribution;
        }

        public Task InitAsync()
        {
            database.Open();
            return CreateTablesAsync();
        }

        public async Task InitAsync2()
        {
            database.Open();
            await CreateTablesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static IPackager GetPackager(FormatType type, string fileName, string attribution)
        {
            switch (type)
            {
                case FormatType.BCNav:
                    return new BCNavPackager(fileName, attribution);
                case FormatType.MBTiles:
                    return new MBTilesPackager(fileName, attribution);
                default:
                    throw new ArgumentException("Type must be either BCNav or OruxMaps", "type");
            }
        }

        public async Task AddTileAsync(Tile tile, Task<byte[]> futureData)
        {
            var data = await futureData;
            if (data != null)
            {
                await AddTileAsync(tile, data);
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
                await UpdateTileMetaInfoAsync();
                database.Dispose();
            }
        }

        private async Task CreateTablesAsync()
        {
            await database.ExecuteNonQueryAsync(TABLE_DDL);
            await database.ExecuteNonQueryAsync(INDEX_DDL);
            await UpdateLocaleAsync();
        }

        private async Task UpdateLocaleAsync()
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
