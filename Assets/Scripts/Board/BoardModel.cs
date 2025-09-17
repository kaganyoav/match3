using Tile;

namespace Board
{
    public class BoardModel
    {
        public readonly int Rows;
        public readonly int Cols;
        public TileModel[,] Tiles;

        public BoardModel(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Tiles = new TileModel[rows, cols];
            // default is empty structs; be explicit if you prefer:
            for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                Tiles[r, c] = TileModel.Empty;
        }

        public bool InBounds(int r, int c) => r >= 0 && r < Rows && c >= 0 && c < Cols;

        public void Swap((int r, int c) a, (int r, int c) b) => (Tiles[a.r, a.c], Tiles[b.r, b.c]) = (Tiles[b.r, b.c], Tiles[a.r, a.c]);
    }

}
