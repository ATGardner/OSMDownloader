namespace com.atgardner.OMFG.packagers
{
    using tiles;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    class BCNavPackager : SQLitePackager
    {
        protected override string TABLE_DDL
        {
            get { return "CREATE TABLE IF NOT EXISTS tiles (x int, y int, z int, s int, image blob, PRIMARY KEY (x,y,z,s))"; }
        }

        protected override string INDEX_DDL
        {
            get { return "CREATE INDEX IF NOT EXISTS IND on tiles (x, y, z, s)"; }
        }

        protected override string INSERT_SQL
        {
            get { return "INSERT or REPLACE INTO tiles (x, y, z, s, image) VALUES (@x, @y, @z, 0, @image)"; }
        }

        private string RMAPS_TABLE_INFO_DDL = "CREATE TABLE IF NOT EXISTS info AS SELECT 99 AS minzoom, 0 AS maxzoom";
        private string RMAPS_CLEAR_INFO_SQL = "DELETE FROM info;";
        private string RMAPS_UPDATE_INFO_MINMAX_SQL = "insert into info(minzoom, maxzoom) values((select min(z) from tiles), (select max(z) from tiles));";

        public BCNavPackager(string sourceFile, string attribution) : base(sourceFile, attribution) { }

        protected override string GetDbFileName(string fileName)
        {
            var fullPath = Path.GetFullPath(fileName);
            return Path.ChangeExtension(fullPath, "sqlitedb");
        }

        public override async Task AddTile(Tile tile)
        {
            var parameters = new Dictionary<string, object> {
                { "x", tile.X },
                { "y", tile.Y },
                { "z", 17 - tile.Zoom },
                { "image", tile.Image }
            };
            await database.ExecuteNonQueryAsync(INSERT_SQL, parameters);
        }

        protected override async Task UpdateTileMetaInfo()
        {
            await database.ExecuteNonQueryAsync(RMAPS_TABLE_INFO_DDL);
            await database.ExecuteNonQueryAsync(RMAPS_CLEAR_INFO_SQL);
            await database.ExecuteNonQueryAsync(RMAPS_UPDATE_INFO_MINMAX_SQL);
        }
    }
}
