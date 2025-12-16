namespace MatrixMul.App;

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

public static class Benchmark
{
    public sealed class Stats
    {
        public double MeanMs { get; init; }
        public double StdMs { get; init; }
    }

    private static Stats TimeMany(int runs, Func<IntMatrix> job)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(runs);

        // тёплый прогон, чтобы разогреть JIT и кэш
        _ = job();

        var times = new double[runs];
        for (int i = 0; i < runs; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var sw = Stopwatch.StartNew();
            _ = job();                       // считаем, результат не используем
            sw.Stop();
            times[i] = sw.Elapsed.TotalMilliseconds;
        }

        double n = runs;
        double sum = 0;
        for (int i = 0; i < runs; i++) sum += times[i];
        double mean = sum / n;

        double s2 = 0;
        for (int i = 0; i < runs; i++)
        {
            double d = times[i] - mean;
            s2 += d * d;
        }
        double std = runs > 1 ? Math.Sqrt(s2 / (n - 1)) : 0.0;

        return new Stats { MeanMs = mean, StdMs = std };
    }

    public static void Run(int m, int k, int n, int runs, int threads, string csvPath = "bench_results.csv")
    {
        // Фиксированные сиды — чтобы A и B неизменны между прогонами
        var A = MatrixGen.CreateRandom(m, k, seed: 12345);
        var B = MatrixGen.CreateRandom(k, n, seed: 67890);

        // Прогоним разогрев явно
        _ = SequentialMultiplier.Multiply(A, B);
        _ = ThreadedMultiplier.Multiply(A, B, threads);

        var seq = TimeMany(runs, () => SequentialMultiplier.Multiply(A, B));
        var par = TimeMany(runs, () => ThreadedMultiplier.Multiply(A, B, threads));

        double speedup = seq.MeanMs / par.MeanMs;

        // Печать мини-таблицы в консоль
        Console.WriteLine("=== Benchmark (m x k) · (k x n) ===");
        Console.WriteLine($"Size: ({m} x {k}) · ({k} x {n}), runs: {runs}, threads: {threads}");
        Console.WriteLine($"Sequential: mean = {seq.MeanMs:F2} ms, std = {seq.StdMs:F2} ms");
        Console.WriteLine($"Threaded  : mean = {par.MeanMs:F2} ms, std = {par.StdMs:F2} ms");
        Console.WriteLine($"Speedup   : {speedup:F2}x");
        Console.WriteLine();

        // Запись/добавление в CSV для графиков
        bool writeHeader = !File.Exists(csvPath);
        using var sw = new StreamWriter(csvPath, append: true);
        if (writeHeader)
        {
            sw.WriteLine("m,k,n,runs,threads,seq_mean_ms,seq_std_ms,par_mean_ms,par_std_ms,speedup");
        }
        sw.WriteLine(string.Join(",",
            m, k, n, runs, threads,
            seq.MeanMs.ToString(CultureInfo.InvariantCulture),
            seq.StdMs.ToString(CultureInfo.InvariantCulture),
            par.MeanMs.ToString(CultureInfo.InvariantCulture),
            par.StdMs.ToString(CultureInfo.InvariantCulture),
            speedup.ToString(CultureInfo.InvariantCulture)));
    }
}
