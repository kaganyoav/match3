using System;
using RNG;
using Tile;
using UnityEngine;


namespace Board
    {
    public class Spawner
    {
        private readonly TileSet _tileSet;
        private readonly IRng _rng;

        public Spawner(TileSet tileSet, IRng rng)
        {
            _tileSet = tileSet;
            _rng     = rng;
        }
        
        public void FillNoInitialMatches(BoardModel board, int maxRerollsPerCell = 20)
        {
            for (int r = 0; r < board.Rows; r++)
            {
                for (int c = 0; c < board.Cols; c++)
                {
                    var type = PickTypeAvoidingLeftUp(board, r, c, maxRerollsPerCell);
                    board.Tiles[r, c] = new TileModel(type);
                }
            }
        }

        private TileTypeId PickTypeAvoidingLeftUp(BoardModel board, int r, int c, int maxRerolls)
        {
            for (int tries = 0; tries < maxRerolls; tries++)
            {
                var type = _tileSet.GetRandomWeighted(_rng);
                if (!WouldFormStreak(board, r, c, type))
                    return type;
            }
            
            return _tileSet.AnyKeyOrDefault();
        }
        
        private bool WouldFormStreak(BoardModel b, int r, int c, TileTypeId t)
        {
            // Horizontal: check left-left pattern t t _
            if (c >= 2 &&
                b.Tiles[r, c - 1].Type == t &&
                b.Tiles[r, c - 2].Type == t)
                return true;

            // Vertical: check up-up pattern
            if (r >= 2 &&
                b.Tiles[r - 1, c].Type == t &&
                b.Tiles[r - 2, c].Type == t)
                return true;

            // (Optional stricter rules to also avoid sandwiches at init:
            // if (c >= 1 && c + 1 < b.Cols && b.Tiles[r, c - 1].Type == t && b.Tiles[r, c + 1].Type == t) return true;
            // if (r >= 1 && r + 1 < b.Rows && b.Tiles[r - 1, c].Type == t && b.Tiles[r + 1, c].Type == t) return true;
            // Usually not needed for initial fill and can over-constrain small boards.)

            return false;
        }
    }
}
