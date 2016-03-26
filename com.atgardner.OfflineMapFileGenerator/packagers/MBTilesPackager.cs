namespace com.atgardner.OMFG.packagers
{
    using tiles;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.IO;
    using System;
    using NLog;

    class MBTilesPackager : SQLitePackager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected override string FileExtension
        {
            get { return "mbtiles"; }
        }

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
        private string METADATA_INSERT_SQL = "INSERT or REPLACE INTO metadata(name, value) VALUES(@name, @value);";
        private readonly string name;

        public MBTilesPackager(string name, string attribution) : base(name, attribution)
        {
            this.name = Path.GetFileNameWithoutExtension(name);
        }

        public override async Task AddTileAsync(Tile tile, byte[] data)
        {
            logger.Debug("Tile {0} - Adding tile async", tile);
            //switching the tile_row direction
            var tile_row = (1 << tile.Zoom) - tile.Y - 1;
            var parameters = new Dictionary<string, object> {
                { "tile_column", tile.X },
                { "tile_row", tile_row },
                { "zoom_level", tile.Zoom },
                { "tile_data", data }
            };
            await database.ExecuteNonQueryAsync(INSERT_SQL, parameters);
            logger.Debug("Tile {0} - Done adding tile async", tile);
        }

        protected override async Task UpdateTileMetaInfoAsync()
        {
            await database.ExecuteNonQueryAsync(METADATA_TABLE_DDL);
            await database.ExecuteNonQueryAsync(METADATA_INSERT_SQL, new Dictionary<string, object> { { "name", "name" }, { "value", name } });
            await database.ExecuteNonQueryAsync(METADATA_INSERT_SQL, new Dictionary<string, object> { { "name", "type" }, { "value", "baselayer" } });
            await database.ExecuteNonQueryAsync(METADATA_INSERT_SQL, new Dictionary<string, object> { { "name", "version" }, { "value", "1.2" } });
            await database.ExecuteNonQueryAsync(METADATA_INSERT_SQL, new Dictionary<string, object> { { "name", "description" }, { "value", string.Format("{0} created on {1} by OMFG", name, DateTime.Now) } });
            await database.ExecuteNonQueryAsync(METADATA_INSERT_SQL, new Dictionary<string, object> { { "name", "format" }, { "value", "png" } }); /**/
            await database.ExecuteNonQueryAsync(METADATA_INSERT_SQL, new Dictionary<string, object> { { "name", "bounds" }, { "value", "-180.0,-85,180,85" } }); /**/
            await database.ExecuteNonQueryAsync(METADATA_INSERT_SQL, new Dictionary<string, object> { { "name", "attribution" }, { "value", Attribution } });
        }
    }
}
