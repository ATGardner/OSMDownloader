
namespace com.atgardner.OMFG.packagers
{
    using tiles;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.IO;

    class MBTilesPackager : SQLitePackager
    {
        protected override string INDEX_DDL
        {
            get { return "CREATE INDEX IF NOT EXISTS IND on tiles (tile_column, tile_row, zoom_level)"; }
        }

        protected override string INSERT_SQL
        {
            get { return "INSERT or REPLACE INTO tiles (tile_column, tile_row, zoom_level, tile_data) VALUES (@tile_column, @tile_row, @zoom_level, @tile_data)"; }
        }

        protected override string TABLE_DDL
        {
            get { return "CREATE TABLE IF NOT EXISTS tiles (zoom_level integer, tile_column integer, tile_row integer, tile_data blob, PRIMARY KEY (tile_column, tile_row, zoom_level));"; }
        }

        private string METADATA_TABLE_DDL = "CREATE TABLE IF NOT EXISTS metadata (name text, value text, PRIMARY KEY (name));";
        private string METADATA_INSERT_SQL = "insert into metadata(name, value) values(@name, @value);";
        private readonly string name;

        public MBTilesPackager(string name) : base(name)
        {
            this.name = name;
        }

        public override async Task AddTile(Tile tile)
        {
            //switching the tile_row direction
            var tile_row = (1 << tile.Zoom) - tile.Y - 1;
            var parameters = new Dictionary<string, object> {
                { "tile_column", tile.X },
                { "tile_row", tile_row },
                { "zoom_level", tile.Zoom },
                { "tile_data", tile.Image }
            };
            await database.ExecuteNonQueryAsync(INSERT_SQL, parameters);
        }

        protected override string GetDbFileName(string fileName)
        {
            var fullPath = Path.GetFullPath(fileName);
            return Path.ChangeExtension(fullPath, "sqlitedb");
        }

        protected override async Task UpdateTileMetaInfo()
        {
            await database.ExecuteNonQueryAsync(METADATA_TABLE_DDL);
            await database.ExecuteNonQueryAsync(METADATA_INSERT_SQL, new Dictionary<string, object> { { "name", "name" }, { "value", name } });
            await database.ExecuteNonQueryAsync(METADATA_INSERT_SQL, new Dictionary<string, object> { { "name", "type" }, { "value", "baselayer" } }); /**/
            await database.ExecuteNonQueryAsync(METADATA_INSERT_SQL, new Dictionary<string, object> { { "name", "version" }, { "value", "1" } }); /**/
            await database.ExecuteNonQueryAsync(METADATA_INSERT_SQL, new Dictionary<string, object> { { "name", "description" }, { "value", "description" } }); /**/
            await database.ExecuteNonQueryAsync(METADATA_INSERT_SQL, new Dictionary<string, object> { { "name", "format" }, { "value", "png" } }); /**/
            await database.ExecuteNonQueryAsync(METADATA_INSERT_SQL, new Dictionary<string, object> { { "name", "bounds" }, { "value", "-180.0,-85,180,85" } }); /**/
            await database.ExecuteNonQueryAsync(METADATA_INSERT_SQL, new Dictionary<string, object> { { "name", "attribution" }, { "value", "attribution" } }); /**/
        }
    }
}
