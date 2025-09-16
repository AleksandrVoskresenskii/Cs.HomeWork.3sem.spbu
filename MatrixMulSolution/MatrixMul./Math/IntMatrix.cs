using System;

namespace MatrixMul.App;

public sealed class IntMatrix
{
    private readonly int[] _data;

    public int Rows { get; }
    public int Cols { get; }

    public IntMatrix(int rows, int cols)
    {
        if (rows <= 0) throw new ArgumentOutOfRangeException(nameof(rows));
        if (cols <= 0) throw new ArgumentOutOfRangeException(nameof(cols));

        Rows = rows;
        Cols = cols;
        _data = new int[rows * cols];
    }

    public int this[int r, int c]
    {
        get
        {
            if ((uint)r >= (uint)Rows) throw new ArgumentOutOfRangeException(nameof(r));
            if ((uint)c >= (uint)Cols) throw new ArgumentOutOfRangeException(nameof(c));
            return _data[r * Cols + c];
        }
        set
        {
            if ((uint)r >= (uint)Rows) throw new ArgumentOutOfRangeException(nameof(r));
            if ((uint)c >= (uint)Cols) throw new ArgumentOutOfRangeException(nameof(c));
            _data[r * Cols + c] = value;
        }
    }
}
