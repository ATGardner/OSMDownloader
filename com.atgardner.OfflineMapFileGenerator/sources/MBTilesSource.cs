namespace com.atgardner.OMFG.sources
{
    using com.atgardner.OMFG.tiles;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class MBTilesSource : ITileSource
    {
        private readonly string SELECT_SQL = "select tile_data from tiles where tile_column = @tile_column and tile_row = @tile_row and zoom_level = @zoom_level;";
        private bool initialized;
        protected DbConnection Connection { get; private set; }

        public MBTilesSource(SourceDescriptor source)
        {
            Connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", source.Address));
        }

        public void Init()
        {
            Connection.Open();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<Tile> GetTileData(Tile tile)
        {
            if (!initialized)
            {
                Init();
                initialized = true;
            }

            var command = Connection.CreateCommand();

            //switching the tile_row direction
            var y = (1 << tile.Zoom) - tile.Y - 1;
            command.CommandText = SELECT_SQL;
            AddParameter(command, DbType.Int32, "tile_column", tile.X);
            AddParameter(command, DbType.Int32, "tile_row", y);
            AddParameter(command, DbType.Int32, "zoom_level", tile.Zoom);
            tile.Image = (byte[])await command.ExecuteScalarAsync();
            return tile;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (Connection != null)
            {
                Connection.Dispose();
            }
        }

        private static void AddParameter(DbCommand command, DbType type, string name, object value)
        {
            var param = command.CreateParameter();
            param.DbType = type;
            param.ParameterName = name;
            param.Value = value;
            command.Parameters.Add(param);
        }
    }
}
