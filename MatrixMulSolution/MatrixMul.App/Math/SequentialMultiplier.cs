using System;

namespace MatrixMul.App;

public static class SequentialMultiplier
{
    // Классическое умножение; порядок i-k-j — дружелюбный к кэшу.
    public static IntMatrix Multiply(IntMatrix a, IntMatrix b)
    {
        if (a is null) throw new ArgumentNullException(nameof(a));
        if (b is null) throw new ArgumentNullException(nameof(b));
        if (a.Cols != b.Rows)
            throw new ArgumentException("Incompatible sizes: a.Cols must equal b.Rows.");

        var c = new IntMatrix(a.Rows, b.Cols);

        for (int i = 0; i < a.Rows; i++)
        {
            for (int k = 0; k < a.Cols; k++)
            {
                int aik = a[i, k];
                for (int j = 0; j < b.Cols; j++)
                {
                    c[i, j] += aik * b[k, j];
                }
            }
        }

        return c;
    }
}
