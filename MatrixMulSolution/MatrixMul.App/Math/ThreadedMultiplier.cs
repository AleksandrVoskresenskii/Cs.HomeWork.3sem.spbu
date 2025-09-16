using System;
using System.Threading;

namespace MatrixMul.App;

public static class ThreadedMultiplier
{
    public static IntMatrix Multiply(IntMatrix a, IntMatrix b, int threadCount)
    {
        if (a is null) throw new ArgumentNullException(nameof(a));
        if (b is null) throw new ArgumentNullException(nameof(b));
        if (a.Cols != b.Rows)
            throw new ArgumentException("Incompatible sizes: a.Cols must equal b.Rows.");
        if (threadCount <= 0) throw new ArgumentOutOfRangeException(nameof(threadCount));

        var c = new IntMatrix(a.Rows, b.Cols);

        int rows = a.Rows;
        int chunks = Math.Min(threadCount, rows);              // если строк меньше, чем потоков
        int block = (rows + chunks - 1) / chunks;              // «потолок»(rows/chunks)
        var threads = new Thread[chunks];

        int t = 0;
        for (int start = 0; start < rows; start += block)
        {
            int i0 = start;
            int i1 = Math.Min(start + block, rows);            // [i0, i1)

            threads[t] = new Thread(() => Worker(a, b, c, i0, i1));
            threads[t].Start();
            t++;
        }

        // Ждём завершения всех потоков
        for (int i = 0; i < t; i++)
            threads[i].Join();

        return c;
    }

    private static void Worker(IntMatrix a, IntMatrix b, IntMatrix c, int i0, int i1)
    {
        for (int i = i0; i < i1; i++)
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
    }
}
