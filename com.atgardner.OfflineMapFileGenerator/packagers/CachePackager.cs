namespace com.atgardner.OMFG.packagers
{
    using sources;
    using tiles;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    class CachePackager : SQLitePackager, IDataCache
    {
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
            : base(sourceName, string.Empty)
        {
            initialized = false;
        }

        protected override string GetDbFileName(string fileName)
        {
            var applicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            fileName = Path.ChangeExtension(fileName, "sqlitedb");
            var dbFile = Path.Combine(applicationData, "OMFG", "cache", fileName);
            isNew = !File.Exists(dbFile);
            return dbFile;
        }

        public override async Task AddTile(Tile tile)
        {
            await PutData(tile);
        }

        protected override Task UpdateTileMetaInfo()
        {
            return Task.FromResult(0);
        }

        public async Task GetData(Tile tile)
        {
            if (!initialized)
            {
                await Init();
                initialized = true;
            }

            if (isNew)
            {
                return;
            }

            tile.Image = (byte[])await database.ExecuteScalarAsync(SELECT_SQL, new Dictionary<string, object> {
                { "x", tile.X },
                { "y", tile.Y },
                { "z", tile.Zoom }
            });
        }

        public async Task PutData(Tile tile)
        {
            if (tile == null)
            {
                return;
            }

            await database.ExecuteNonQueryAsync(INSERT_SQL, new Dictionary<string, object> {
                { "x", tile.X },
                { "y", tile.Y },
                { "z", tile.Zoom },
                { "image", tile.Image }
            });
        }
    }
}
