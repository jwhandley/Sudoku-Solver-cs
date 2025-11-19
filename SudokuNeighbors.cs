using System.Diagnostics;

namespace Sudoku_Solver;

public static class SudokuNeighbors
{
    public const int Size = 9;
    public const int Cells = Size * Size;
    public const int NeighborCount = 20;
    public const int BoxAllCount = 8;
    public const int ColCount = 8;
    public const int RowCount = 8;


    public static readonly int[] Neighbors = Build();
    public static readonly int[] BoxAll = BuildBoxAll();

    private static int[] Build()
    {
        var a = new int[Cells * NeighborCount];

        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
            {
                int k = 0;
                int baseOffset = (r * Size + c) * NeighborCount;

                // Same column
                for (int nr = 0; nr < Size; nr++)
                    if (nr != r) a[baseOffset + k++] = nr * Size + c;

                // Same row
                for (int nc = 0; nc < Size; nc++)
                    if (nc != c) a[baseOffset + k++] = r * Size + nc;


                // Same square
                int sRow = (r / 3) * 3;
                int sCol = (c / 3) * 3;
                for (int dr = 0; dr < 3; dr++)
                    for (int dc = 0; dc < 3; dc++)
                    {
                        int rr = sRow + dr, cc = sCol + dc;
                        if (rr == r || cc == c) continue;
                        a[baseOffset + k++] = rr * Size + cc;
                    }

                Debug.Assert(k == NeighborCount);
            }

        return a;
    }

    private static int[] BuildBoxAll()
    {
        var a = new int[Cells * BoxAllCount];
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
            {
                int baseOffset = (r * Size + c) * BoxAllCount;
                int k = 0;
                int sRow = (r / 3) * 3;
                int sCol = (c / 3) * 3;
                for (int dr = 0; dr < 3; dr++)
                    for (int dc = 0; dc < 3; dc++)
                    {
                        int rr = sRow + dr, cc = sCol + dc;
                        if (rr == r && cc == c) continue;      // skip self only
                        a[baseOffset + k++] = rr * Size + cc;
                    }
            }
        return a;
    }



    public static ReadOnlySpan<int> Get(int cellIndex)
    {
        int offset = cellIndex * NeighborCount;
        return Neighbors.AsSpan(offset, NeighborCount);
    }

    public static ReadOnlySpan<int> GetColumn(int cell)
    => Neighbors.AsSpan(cell * NeighborCount + 0, ColCount);

    public static ReadOnlySpan<int> GetRow(int cell)
        => Neighbors.AsSpan(cell * NeighborCount + ColCount, RowCount);

    public static ReadOnlySpan<int> GetBoxAll(int cell)
    => BoxAll.AsSpan(cell * BoxAllCount, BoxAllCount);

}