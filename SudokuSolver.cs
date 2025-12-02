using System.Buffers;

namespace Sudoku_Solver;

public static class CellExtensions
{
    extension(uint p)
    {
        public int Remaining => (int)uint.PopCount(p);
        public bool IsSolved => uint.PopCount(p) == 1;
        public bool IsEmpty => uint.PopCount(p) == 0;
        public uint Remove(int v) => p & ~(1u << (v - 1));
        public bool TryGetValue(out int v)
        {
            if (p.IsSolved)
            {
                v = (int)uint.TrailingZeroCount(p) + 1;
                return true;
            }

            v = -1;
            return false;
        }
        public bool Contains(int v) => (p & 1 << (v - 1)) != 0;

        public int GetValue => (int)uint.TrailingZeroCount(p) + 1;
    }
}