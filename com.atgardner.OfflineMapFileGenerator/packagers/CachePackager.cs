namespace com.atgardner.OMFG.packagers
{
    using sources;
    using tiles;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using NLog;

    class CachePackager : SQLitePackager, IDataCache
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string SELECT_SQL = "SELECT image FROM tiles WHERE x = @x AND y = @y AND z = @z";
        private bool isNew;
        private bool initialized;

        protected override string TABLE_DDL
        {
            get { return "CREATE TABLE IF NOT EXISTS tiles (x int, y int, z int, image blob, PRIMARY KEY (x, y, z))"; }
        }

        protected override string INDEX_DDL
        {
            get { return "CREATE INDEX IF NOT EXISTS IND on tiles (x, y, z)"; }
        }

        protected override string INSERT_SQL
        {
            get { return "INSERT or IGNORE INTO tiles (x, y, z, image) VALUES (@x, @y, @z, @image)"; }
        }

        public CachePackager(string sourceName)
            : base(Path.Combine("cache", sourceName), string.Empty)
        {
            initialized = false;
            isNew = !File.Exists(dbFile);
        }

        public override async Task AddTileAsync(Tile tile, byte[] data)
        {
            await PutDataAsync(tile, data);
        }

        protected override Task UpdateTileMetaInfoAsync()
        {
            return Task.FromResult(0);
        }

        public async Task<byte[]> GetDataAsync(Tile tile)
        {
            if (!initialized)
            {
                await InitAsync();
                initialized = true;
            }

            if (isNew)
            {
                return null;
            }

            logger.Debug("Tile {0} - getting data from cache", tile);
            var data = (byte[])await database.ExecuteScalarAsync(SELECT_SQL, new Dictionary<string, object> {
                { "x", tile.X },
                { "y", tile.Y },
                { "z", tile.Zoom }
            });
            logger.Debug("Tile {0} - got data from cache, found: {1}", tile, data != null);
            return data;
        }

        public async Task PutDataAsync(Tile tile, byte[] data)
        {
            logger.Debug("Tile {0} - putting data into cache", tile);
            await database.ExecuteNonQueryAsync(INSERT_SQL, new Dictionary<string, object> {
                { "x", tile.X },
                { "y", tile.Y },
                { "z", tile.Zoom },
                { "image", data }
            });
            logger.Debug("Tile {0} - done putting data into cache", tile);
        }
    }
}
