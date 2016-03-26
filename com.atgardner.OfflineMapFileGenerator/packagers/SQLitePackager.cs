namespace com.atgardner.OMFG.packagers
{
    using tiles;
    using utils;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.IO;

    public abstract class SQLitePackager : IPackager
    {
        protected abstract string TABLE_DDL { get; }
        protected abstract string INDEX_DDL { get; }
        protected abstract string INSERT_SQL { get; }
        protected abstract Task UpdateTileMetaInfoAsync();
        public abstract Task AddTileAsync(Tile tile, byte[] data);

        private readonly string METADATA_DDL = "CREATE TABLE IF NOT EXISTS android_metadata (locale TEXT)";
        private readonly string METADATA_SELECT = "SELECT count(*) FROM android_metadata";
        private readonly string METADATA_INSERT = "INSERT INTO android_metadata VALUES (@locale)";
        private bool disposed = false;

        protected readonly string dbFile;
        protected readonly Database database;
        protected readonly string attribution;

        protected virtual string FileExtension
        {
            get { return "sqlitedb"; }
        }

        public SQLitePackager(string fileName, string attribution)
        {
            dbFile = Path.ChangeExtension(fileName, FileExtension);
            database = new Database(dbFile);
            this.attribution = attribution;
        }

        public Task InitAsync()
        {
            database.Open();
            return CreateTablesAsync();
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
                    var allTypes = Enum.GetValues(type.GetType()).Cast<FormatType>();
                    var packagers = from t in allTypes where (type & t) != FormatType.None select GetPackager(t, fileName, attribution);
                    return new CompositePackager(packagers.ToArray());
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

        public async Task DoneAsync()
        {
            await UpdateTileMetaInfoAsync();
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (disposed)
            {
                return;
            }

            if (isDisposing)
            {
                if (database != null)
                {
                    database.Dispose();
                }
            }

            disposed = true;
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
