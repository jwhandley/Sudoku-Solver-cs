using System.Buffers;
using System.Text;
namespace Sudoku_Solver;

public sealed class Grid : IDisposable
{
    private static readonly ArrayPool<uint> arrayPool = ArrayPool<uint>.Shared;
    private readonly uint[] values;
    const int full = 0b111_111_111;
    public Grid()
    {
        values = arrayPool.Rent(81);
        for (int i = 0; i < 81; i++)
        {
            values[i] = full;
        }
    }

    public void CloneFrom(Grid other)
    {
        Array.Copy(other.values, values, 81);
    }

    public static Grid Parse(string s)
    {
        s = s.Replace(" ", "");
        var g = new Grid();

        for (int i = 0; i < 81; i++)
        {

            if (char.IsDigit(s[i]))
            {
                int v = s[i] - '0';
                g.Fill(i, v);
            }
        }

        return g;
    }

    private bool Eliminate(int i, int v)
    {
        if (!values[i].Contains(v)) return true;
        values[i] = values[i].Remove(v);
        if (values[i].IsEmpty) return false;

        if (values[i].TryGetValue(out int n))
        {
            foreach (var j in SudokuNeighbors.Get(i))
            {
                if (!Eliminate(j, n)) return false;
            }
        }

        // Deal with col, row, and box neighbors
        if (!HandleNeighbors(SudokuNeighbors.GetRow(i), v)) return false;
        if (!HandleNeighbors(SudokuNeighbors.GetColumn(i), v)) return false;
        if (!HandleNeighbors(SudokuNeighbors.GetBoxAll(i), v)) return false;

        return true;
    }

    private bool HandleNeighbors(in ReadOnlySpan<int> nbr, int v)
    {
        int count = 0;
        int found = -1;
        foreach (var i in nbr)
        {
            if (values[i].Contains(v))
            {
                count++;
                found = i;
            }
        }
        if (count == 0) return false;
        if (count == 1 && !Fill(found, v)) return false;

        return true;
    }

    private bool Fill(int i, int v)
    {
        for (int n = 1; n <= 9; n++)
        {
            if (n == v) continue;
            if (!Eliminate(i, n)) return false;
        }

        return true;
    }

    public bool Search()
    {
        if (!SelectSquare(out int idx))
        {
            return true;
        }

        using var result = new Grid();
        for (int p = 1; p <= 9; p++)
        {
            if (!values[idx].Contains(p)) continue;

            result.CloneFrom(this);
            if (result.Fill(idx, p) && result.Search())
            {
                CloneFrom(result);
                return true;
            }
        }

        return false;
    }
    private bool SelectSquare(out int idx)
    {
        int lowest = int.MaxValue;
        idx = -1;

        for (int i = 0; i < 81; i++)
        {
            int remaining = values[i].Remaining;
            if (remaining > 1 && remaining < lowest)
            {
                lowest = remaining;
                idx = i;
            }
        }

        return lowest < int.MaxValue;
    }

    public bool Validate()
    {
        for (int i = 0; i < 81; i++)
        {
            if (!values[i].IsSolved)
            {
                Console.WriteLine("Not all values are solved");
                return false;
            }
        }


        bool[] seen = new bool[9];

        // Check rows
        for (int r = 0; r < 9; r++)
        {
            Array.Fill(seen, false);
            for (int c = 0; c < 9; c++)
            {
                seen[values[r * 9 + c].GetValue - 1] = true;
            }

            if (!seen.All(v => v))
            {
                Console.WriteLine($"Repeted values in row {r}");
                return false;
            }
        }

        // Check columns
        for (int c = 0; c < 9; c++)
        {
            Array.Fill(seen, false);
            for (int r = 0; r < 9; r++)
            {
                seen[values[r * 9 + c].GetValue - 1] = true;
            }

            if (!seen.All(v => v))
            {
                Console.WriteLine($"Repeted values in col {c}");
                return false;
            }
        }

        // Check squares
        for (int sr = 0; sr < 9; sr += 3)
        {
            for (int sc = 0; sc < 9; sc += 3)
            {
                Array.Fill(seen, false);
                for (int dr = 0; dr < 3; dr++)
                {
                    for (int dc = 0; dc < 3; dc++)
                    {
                        seen[values[(sr + dr) * 9 + sc + dc].GetValue - 1] = true;
                    }
                }
                if (!seen.All(v => v))
                {
                    Console.WriteLine($"Repeted values in square ({sr},{sc})");
                    return false;
                }
            }
        }

        return true;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (values[r * 9 + c].TryGetValue(out int v))
                {
                    sb.Append(v);
                }
                else
                {
                    sb.Append('.');
                }
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public void Dispose()
    {
        if (values is not null) arrayPool.Return(values, false);
    }
}