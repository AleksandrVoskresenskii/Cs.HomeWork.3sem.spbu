using System;
using System.Globalization;
using System.IO;

namespace MatrixMul.App;

public static class MatrixFile
{
    // Формат: первая строка "rows cols", затем rows строк по cols целых чисел.
    public static IntMatrix Read(string path)
    {
        using var sr = new StreamReader(path);
        string? header = sr.ReadLine();
        if (string.IsNullOrWhiteSpace(header))
            throw new InvalidDataException("Matrix file is empty.");

        var parts = header.Split((char[])null!, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
            throw new InvalidDataException("First line must be: <rows> <cols>.");

        if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int rows) ||
            !int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int cols))
            throw new InvalidDataException("Rows/cols must be integers.");

        var m = new IntMatrix(rows, cols);

        for (int i = 0; i < rows; i++)
        {
            string? line = sr.ReadLine();
            if (line is null) throw new InvalidDataException($"Not enough rows: expected {rows}.");
            var nums = line.Split((char[])null!, StringSplitOptions.RemoveEmptyEntries);
            if (nums.Length != cols)
                throw new InvalidDataException($"Row {i} has {nums.Length} values, expected {cols}.");

            for (int j = 0; j < cols; j++)
                m[i, j] = int.Parse(nums[j], CultureInfo.InvariantCulture);
        }

        return m;
    }

    public static void Write(string path, IntMatrix m)
    {
        using var sw = new StreamWriter(path);
        sw.WriteLine($"{m.Rows} {m.Cols}");
        for (int i = 0; i < m.Rows; i++)
        {
            for (int j = 0; j < m.Cols; j++)
            {
                if (j > 0) sw.Write(' ');
                sw.Write(m[i, j].ToString(CultureInfo.InvariantCulture));
            }
            sw.WriteLine();
        }
    }
}
