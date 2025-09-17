using RNG;
using Tile;
using UnityEngine;

namespace Board
{ 
    public class BoardController : MonoBehaviour
    {
        [Header("Config")]
        public BoardConfig config;

        [Header("Parents (auto-created if null)")]
        public Transform cellsRoot;
        public Transform tilesRoot;

        [Header("Camera Fit")]
        public bool fitCameraToBoard = true;
        public float cameraPaddingWorld = 0.5f;
        
        public BoardModel Model { get; private set; }
        private TileView[,] _tileViews;
        
        public TileFactory tileFactory; 

        public void GenerateBoard()
        {
            if (config == null || config.cellPrefab == null)
            {
                Debug.LogError("BoardController: Missing config or cell prefab.");
                return;
            }

            // Clear previous
            ClearChildren(ref cellsRoot);
            ClearChildren(ref tilesRoot);

            Model = new BoardModel(config.rows, config.cols);
            _tileViews = new TileView[config.rows, config.cols];

            // Create roots if needed
            if (cellsRoot == null)
            {
                var go = new GameObject("CellsRoot");
                cellsRoot = go.transform;
                cellsRoot.SetParent(transform, false);
            }
            if (tilesRoot == null)
            {
                var go = new GameObject("TilesRoot");
                tilesRoot = go.transform;
                tilesRoot.SetParent(transform, false);
            }

            // Build cells in world space with spacing
            Vector2 totalSize = new Vector2(
                config.cols * config.cellSize + (config.cols - 1) * config.cellSpacing.x,
                config.rows * config.cellSize + (config.rows - 1) * config.cellSpacing.y
            );

            Vector2 origin = (Vector2)transform.position - totalSize * 0.5f + new Vector2(config.cellSize, config.cellSize) * 0.5f;

            for (int r = 0; r < config.rows; r++)
            {
                for (int c = 0; c < config.cols; c++)
                {
                    Vector2 pos = origin + new Vector2(
                        c * (config.cellSize + config.cellSpacing.x),
                        r * (config.cellSize + config.cellSpacing.y)
                    );

                    var cell = Instantiate(config.cellPrefab, pos, Quaternion.identity, cellsRoot);
                    cell.transform.localScale = Vector3.one * config.cellSize; // uniform scaling
                    cell.name = $"Cell_{r}_{c}";
                    cell.Init(r, c);
                }
            }

            if (fitCameraToBoard) FitCamera(totalSize);

            Debug.Log("Board generated.");
        }
        
        public void PopulateTiles()
        {
            if (config == null || tileFactory == null || tileFactory.tileSet == null)
            {
                Debug.LogError("PopulateTiles: missing config/tileFactory/tileSet");
                return;
            }
            
            var spawner = new Spawner(tileFactory.tileSet, new UnityRng());
            spawner.FillNoInitialMatches(Model);
            
            if (tilesRoot == null)
            {
                var go = new GameObject("TilesRoot");
                tilesRoot = go.transform; tilesRoot.SetParent(transform, false);
            }
            for (int i = tilesRoot.childCount - 1; i >= 0; i--) DestroyImmediate(tilesRoot.GetChild(i).gameObject);
            
            tileFactory.parent = tilesRoot;

            for (int r = 0; r < Model.Rows; r++)
            {
                for (int c = 0; c < Model.Cols; c++)
                {
                    var cell = cellsRoot.Find($"Cell_{r}_{c}");
                    var type = Model.Tiles[r, c].Type;
                    var view = tileFactory.CreateAt(cell.position, type, r, c);
                    view.transform.localScale = Vector3.one * config.cellSize;
                    _tileViews[r, c] = view;
                }
            }

            Debug.Log("Tiles populated with per-type prefabs (no initial matches).");
        }

        private void FitCamera(Vector2 boardSize)
        {
            var cam = Camera.main;
            if (cam == null || !cam.orthographic) return;

            float targetWidth = boardSize.x + cameraPaddingWorld * 2f;
            float targetHeight = boardSize.y + cameraPaddingWorld * 2f;

            float aspect = cam.aspect;
            float orthoSizeByHeight = targetHeight * 0.5f;
            float orthoSizeByWidth = (targetWidth * 0.5f) / aspect;

            cam.orthographicSize = Mathf.Max(orthoSizeByHeight, orthoSizeByWidth);
            cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);
        }

        private void ClearChildren(ref Transform root)
        {
            if (root == null) return;
            for (int i = root.childCount - 1; i >= 0; i--)
                DestroyImmediate(root.GetChild(i).gameObject);
        }

        // Quick editor/testing buttons
    #if UNITY_EDITOR
        [ContextMenu("Generate Board")]
        private void _EditorGenerate() => GenerateBoard();

        [ContextMenu("Populate Tiles (Random)")]
        private void _EditorPopulate() => PopulateTiles();
    #endif
    }
}
