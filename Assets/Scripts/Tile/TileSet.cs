using System.Collections.Generic;
using RNG;
using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace Tile
{
    [System.Serializable]
    public class TilePrefabEntry
    {
        public GameObject prefab;
        public int weight = 1;
    }

    [CreateAssetMenu(menuName = "Match3/Tile Set")]
    public class TileSet : ScriptableObject
    {
        [SerializeField]
        private SerializedDictionary<TileTypeId, TilePrefabEntry> tiles 
            = new SerializedDictionary<TileTypeId, TilePrefabEntry>();

        public TilePrefabEntry GetEntry(TileTypeId id) => tiles[id];
        public GameObject GetPrefab(TileTypeId id) => tiles[id].prefab;
        public int GetWeight(TileTypeId id) => tiles[id].weight;
        
        public TileTypeId GetRandomWeighted(IRng rng)
        {
            int total = 0;
            foreach (var kvp in tiles) total += Mathf.Max(1, kvp.Value.weight);
            int roll = rng.Next(0, total);

            foreach (var kvp in tiles)
            {
                int w = Mathf.Max(1, kvp.Value.weight);
                if (roll < w) return kvp.Key;
                roll -= w;
            }
            return tiles.Keys.Count > 0 ? new List<TileTypeId>(tiles.Keys)[0] : default;
        }
        
        public TileTypeId AnyKeyOrDefault()
        {
            foreach (var k in tiles.Keys) return k;
            return default;
        }
    }


}
