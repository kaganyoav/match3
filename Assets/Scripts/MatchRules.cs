using System.Collections.Generic;
using Board;

namespace DefaultNamespace
{
    public static class MatchRules
    {
        // Return all positions that belong to any 3+-in-a-row/column.
        public static HashSet<(int r, int c)> FindAllMatches(BoardModel board)
        {
            var matched = new HashSet<(int, int)>();

            // Horizontal
            for (int r = 0; r < board.Rows; r++)
            {
                int runStart = 0;
                for (int c = 1; c <= board.Cols; c++)
                {
                    bool same = c < board.Cols &&
                               !board.Tiles[r, c].IsEmpty &&
                               board.Tiles[r, c].Type == board.Tiles[r, c - 1].Type;

                    if (!same)
                    {
                        int runLen = c - runStart;
                        if (runLen >= 3)
                            for (int k = runStart; k < c; k++) matched.Add((r, k));
                        runStart = c;
                    }
                }
            }

            // Vertical
            for (int c = 0; c < board.Cols; c++)
            {
                int runStart = 0;
                for (int r = 1; r <= board.Rows; r++)
                {
                    bool same = r < board.Rows &&
                               !board.Tiles[r, c].IsEmpty &&
                               board.Tiles[r, c].Type == board.Tiles[r - 1, c].Type;

                    if (!same)
                    {
                        int runLen = r - runStart;
                        if (runLen >= 3)
                            for (int k = runStart; k < r; k++) matched.Add((k, c));
                        runStart = r;
                    }
                }
            }

            return matched;
        }

        // Fast legality check: does swapping a<->b create any match?
        public static bool SwapCreatesMatch(BoardModel board, (int r, int c) a, (int r, int c) b)
        {
            board.Swap(a, b);

            // Only check lines crossing a and b for performance
            bool result =
                   HasMatchAt(board, a.r, a.c)
                || HasMatchAt(board, b.r, b.c);

            board.Swap(a, b); // revert
            return result;
        }

        public static bool HasMatchAt(BoardModel board, int r, int c)
        {
            if (!board.InBounds(r, c) || board.Tiles[r, c].IsEmpty) return false;
            var t = board.Tiles[r, c].Type;

            // Horizontal run length with (r,c) included
            int cnt = 1;
            for (int x = c - 1; x >= 0 && board.Tiles[r, x].Type == t; x--) cnt++;
            for (int x = c + 1; x < board.Cols && board.Tiles[r, x].Type == t; x++) cnt++;
            if (cnt >= 3) return true;

            // Vertical run length
            cnt = 1;
            for (int y = r - 1; y >= 0 && board.Tiles[y, c].Type == t; y--) cnt++;
            for (int y = r + 1; y < board.Rows && board.Tiles[y, c].Type == t; y++) cnt++;
            return cnt >= 3;
        }
    }

}
