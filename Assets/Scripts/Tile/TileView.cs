using UnityEngine;

namespace Tile
{
    public class TileView : MonoBehaviour
    {
        public TileTypeId Type { get; private set; }
        public int Row { get; private set; }
        public int Col { get; private set; }

        public void Initialize(TileTypeId type, int r, int c)
        {
            Type = type; Row = r; Col = c;
            name = $"Tile_{type}_{r}_{c}";
        }

        public void SetCoord(int r, int c) { Row = r; Col = c; }
    }
}
