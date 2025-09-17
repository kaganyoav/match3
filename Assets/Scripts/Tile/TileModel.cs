namespace Tile
{
    public struct TileModel
    {
        public TileTypeId Type;
        public bool IsEmpty => Type == TileTypeId_Empty;
        // Internal sentinel for “no tile”
        public const TileTypeId TileTypeId_Empty = (TileTypeId)(-1);

        public static TileModel Empty => new TileModel { Type = TileTypeId_Empty };
        public TileModel(TileTypeId t) { Type = t; }
    }

}
