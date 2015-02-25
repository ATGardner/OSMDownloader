using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.atgardner.OMFG.packagers
{
    class OruxPackager : SQLitePackager
    {
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
            get { return "INSERT or IGNORE INTO tiles (x,y,z,image) VALUES (@x, @y, @z, 0, @image)"; }
        }

        private string RMAPS_TABLE_INFO_DDL = "CREATE TABLE IF NOT EXISTS info AS SELECT 99 AS minzoom, 0 AS maxzoom";
        private string RMAPS_CLEAR_INFO_SQL = "DELETE FROM info;";
        private string RMAPS_UPDATE_INFO_MINMAX_SQL = "insert into info(minzoom, maxzoom) values((select min(z) from tiles), (select max(z) from tiles));";

        public OruxPackager(string sourceFile)
            : base(sourceFile)
        {
        }

        protected override string GetDbFileName(string path)
        {
            return Path.Combine(path, "Layer", "OruxMapsImages.db");
        }

        protected override async Task UpdateTileMetaInfo()
        {
            using (var scope = Connection.BeginTransaction())
            {
                var command = Connection.CreateCommand();
                command.CommandText = RMAPS_TABLE_INFO_DDL;
                await command.ExecuteNonQueryAsync();
                command.CommandText = RMAPS_CLEAR_INFO_SQL;
                await command.ExecuteNonQueryAsync();
                command.CommandText = RMAPS_UPDATE_INFO_MINMAX_SQL;
                await command.ExecuteNonQueryAsync();
                scope.Commit();
            }
        }
    }
}
