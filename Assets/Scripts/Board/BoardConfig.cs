using Tile;
using UnityEngine;

namespace Board
{
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "Match3/Board Config")]
    public class BoardConfig : ScriptableObject
    {
        [Min(1)] public int rows = 8;
        [Min(1)] public int cols = 8;
        [Min(0.1f)] public float cellSize = 1f;
        public Vector2 cellSpacing = new Vector2(0.05f, 0.05f);

        [Header("Prefabs")]
        public CellView cellPrefab;
        public TileView[] tilePrefabs;
    }

}
