using System.Collections.Generic;
using UnityEngine;

namespace Tile
{
    public class TileFactory : MonoBehaviour
    {
        [Header("Data")]
        public TileSet tileSet;              
        public Transform parent;              

        private readonly Dictionary<TileTypeId, Queue<TileView>> _pools = new();

        public void WarmPool(TileTypeId type, int count)
        {
            EnsurePool(type);
            var entry = tileSet.GetEntry(type);
            for (int i = 0; i < count; i++)
            {
                var view = Instantiate(entry.prefab).GetComponent<TileView>();
                if (view == null) view = view.gameObject.AddComponent<TileView>();
                view.gameObject.SetActive(false);
                view.transform.SetParent(transform, false);
                _pools[type].Enqueue(view);
            }
        }

        public TileView CreateAt(Vector3 pos, TileTypeId type, int r, int c)
        {
            EnsurePool(type);

            TileView view;
            if (_pools[type].Count > 0)
            {
                view = _pools[type].Dequeue();
                view.transform.SetParent(parent, false);
                view.transform.SetPositionAndRotation(pos, Quaternion.identity);
                view.gameObject.SetActive(true);
            }
            else
            {
                var entry = tileSet.GetEntry(type);
                var go = Instantiate(entry.prefab, pos, Quaternion.identity, parent);
                view = go.GetComponent<TileView>();
                if (view == null)
                {
                    Debug.LogError($"Prefab for {type} is missing TileView. Adding one.");
                    view = go.AddComponent<TileView>();
                }
            }

            view.Initialize(type, r, c);
            return view;
        }

        public void Recycle(TileView view)
        {
            if (view == null) return;
            EnsurePool(view.Type);
            view.gameObject.SetActive(false);
            view.transform.SetParent(transform, false);
            _pools[view.Type].Enqueue(view);
        }

        public void ClearAll()
        {
            // Fully destroy everything under parent (used when regenerating the whole board)
            if (parent != null)
            {
                for (int i = parent.childCount - 1; i >= 0; i--)
                    DestroyImmediate(parent.GetChild(i).gameObject);
            }
            // Also clear pooled items
            foreach (var q in _pools.Values)
                while (q.Count > 0) DestroyImmediate(q.Dequeue().gameObject);
            _pools.Clear();
        }

        private void EnsurePool(TileTypeId type)
        {
            if (!_pools.ContainsKey(type))
                _pools[type] = new Queue<TileView>();
        }
    }
}
