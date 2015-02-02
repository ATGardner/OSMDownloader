namespace com.atgardner.OMFG.tiles
{
    using com.atgardner.OMFG.sources;

    interface IDataCache
    {
        bool HasData(MapSource source, Tile tile);

        byte[] GetData(MapSource source, Tile tile);

        void PutData(MapSource source, Tile tile);
    }
}
