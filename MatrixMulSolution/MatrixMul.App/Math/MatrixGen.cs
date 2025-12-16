using System;

namespace MatrixMul.App;

public static class MatrixGen
{
    // Генерирует матрицу rows×cols со значениями из диапазона [-9, 9]
    public static IntMatrix CreateRandom(int rows, int cols, int seed)
    {
        var rnd = new Random(seed);
        var m = new IntMatrix(rows, cols);
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                m[i, j] = rnd.Next(-9, 10);
        return m;
    }
}
