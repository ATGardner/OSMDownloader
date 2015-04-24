namespace com.atgardner.OMFG.packagers
{
    using com.atgardner.OMFG.sources;
    using com.atgardner.OMFG.tiles;
    using System;
    using System.Data;
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
            : base(sourceName)
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

            var command = Connection.CreateCommand();
            command.CommandText = SELECT_SQL;
            AddParameter(command, DbType.Int32, "x", tile.X);
            AddParameter(command, DbType.Int32, "y", tile.Y);
            AddParameter(command, DbType.Int32, "z", tile.Zoom);
            tile.Image = (byte[])await command.ExecuteScalarAsync();
        }

        public async Task PutData(Tile tile)
        {
            if (tile == null)
            {
                return;
            }

            var command = Connection.CreateCommand();
            command.CommandText = INSERT_SQL;
            AddParameter(command, DbType.Int32, "x", tile.X);
            AddParameter(command, DbType.Int32, "y", tile.Y);
            AddParameter(command, DbType.Int32, "z", tile.Zoom);
            AddParameter(command, DbType.Binary, "image", tile.Image);
            await command.ExecuteNonQueryAsync();
        }
    }
}
