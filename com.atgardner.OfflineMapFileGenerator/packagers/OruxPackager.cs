namespace com.atgardner.OMFG.packagers
{
    using com.atgardner.OMFG.Properties;
    using com.atgardner.OMFG.tiles;
    using com.atgardner.OMFG.utils;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using System.Data;

    class OruxPackager : SQLitePackager
    {
        private const int Tile_Size = 256;
        private readonly string targetPath;
        private readonly IDictionary<int, Bounds> layers;

        private string MapName
        {
            get
            {
                return SecurityElement.Escape(Path.GetFileNameWithoutExtension(targetPath));
            }
        }

        protected override string TABLE_DDL
        {
            get { return "CREATE TABLE IF NOT EXISTS tiles (x int, y int, z int, image blob, PRIMARY KEY (x,y,z))"; }
        }

        protected override string INDEX_DDL
        {
            get { return "CREATE INDEX IF NOT EXISTS IND on tiles (x,y,z)"; }
        }

        protected override string INSERT_SQL
        {
            get { return "INSERT or IGNORE INTO tiles (x,y,z,image) VALUES (@x, @y, @z, @image)"; }
        }

        private string RMAPS_TABLE_INFO_DDL = "CREATE TABLE IF NOT EXISTS info AS SELECT 99 AS minzoom, 0 AS maxzoom";
        private string RMAPS_CLEAR_INFO_SQL = "DELETE FROM info;";
        private string RMAPS_UPDATE_INFO_MINMAX_SQL = "insert into info(minzoom, maxzoom) values((select min(z) from tiles), (select max(z) from tiles));";

        public OruxPackager(string targetPath)
            : base(targetPath)
        {
            layers = new Dictionary<int, Bounds>();
            this.targetPath = targetPath;
        }

        protected override string GetDbFileName(string path)
        {
            return Path.Combine(path, "OruxMapsImages.db");
        }

        public override async Task AddTile(Tile tile)
        {
            Bounds bounds;
            if (!layers.TryGetValue(tile.Zoom, out bounds))
            {
                bounds = new Bounds(tile.Zoom);
                layers[tile.Zoom] = bounds;
            }

            bounds.AddTile(tile);
            var command = Connection.CreateCommand();
            command.CommandText = INSERT_SQL;
            AddParameter(command, DbType.Int32, "x", tile.X);
            AddParameter(command, DbType.Int32, "y", tile.Y);
            AddParameter(command, DbType.Int32, "z", tile.Zoom);
            AddParameter(command, DbType.Binary, "image", tile.Image);
            await command.ExecuteNonQueryAsync();
        }

        protected override async Task UpdateTileMetaInfo()
        {
            var sb = new StringBuilder();
            var zoomLevels = layers.Keys.OrderBy(c => c).ToArray();
            foreach (var zoom in zoomLevels)
            {
                var bounds = layers[zoom];
                var tl = bounds.TL.ToCoordinates();
                var tlLongitude = tl.Longitude == 180D ? -tl.Longitude.Degrees : tl.Longitude.Degrees;
                var br = bounds.BR.ToCoordinates();
                var width = bounds.Width * Tile_Size;
                var height = bounds.Height * Tile_Size;
                var xMax = (width + Tile_Size - 1) / Tile_Size;
                var yMax = (height + Tile_Size - 1) / Tile_Size;
                sb.AppendFormat(Resources.OruxLayerTemplate, MapName, zoom, xMax, yMax, height, width, br.Latitude.Degrees, tl.Latitude.Degrees, tlLongitude, br.Longitude.Degrees);
            }

            var contents = string.Format(Resources.OruxMapTemplate, MapName, sb);
            var otrkFileName = Path.ChangeExtension(Path.Combine(targetPath, MapName), ".otrk2.xml");
            await Task.Factory.StartNew(() => File.WriteAllText(otrkFileName, contents));
        }
    }
}
