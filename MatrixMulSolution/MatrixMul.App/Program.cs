using System;
using MatrixMul.App;

static void PrintUsage()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run --project MatrixMul.App -- gen  <rows> <cols> <out.txt> [seed]");
    Console.WriteLine("  dotnet run --project MatrixMul.App -- mul  <A.txt> <B.txt> <Out.txt> [threads]");
    Console.WriteLine("  dotnet run --project MatrixMul.App -- bench <m> <k> <n> <runs> [threads]");
}

if (args.Length == 0)
{
    PrintUsage();
    return;
}

var cmd = args[0].ToLowerInvariant();

try
{
    switch (cmd)
    {
        case "gen":
            {
                if (args.Length != 4 && args.Length != 5) { PrintUsage(); return; }
                int rows = int.Parse(args[1]);
                int cols = int.Parse(args[2]);
                string path = args[3];
                int seed = args.Length == 5 ? int.Parse(args[4]) : 12345;

                var m = MatrixGen.CreateRandom(rows, cols, seed);
                MatrixFile.Write(path, m);
                Console.WriteLine($"OK: generated {rows}x{cols} to {path} (seed={seed})");
                return;
            }

        case "mul":
            {
                if (args.Length != 4 && args.Length != 5) { PrintUsage(); return; }
                string aPath = args[1];
                string bPath = args[2];
                string outPath = args[3];
                int threads = args.Length == 5 ? int.Parse(args[4]) : Environment.ProcessorCount;

                var A = MatrixFile.Read(aPath);
                var B = MatrixFile.Read(bPath);
                var C = ThreadedMultiplier.Multiply(A, B, threads);
                MatrixFile.Write(outPath, C);

                Console.WriteLine($"OK: result written to {outPath} (threads={threads})");
                return;
            }

        case "bench":
            {
                if (args.Length != 5 && args.Length != 6) { PrintUsage(); return; }
                int m = int.Parse(args[1]);
                int k = int.Parse(args[2]);
                int n = int.Parse(args[3]);
                int runs = int.Parse(args[4]);
                int threadCount = args.Length == 6 ? int.Parse(args[5]) : Environment.ProcessorCount;

                Benchmark.Run(m, k, n, runs, threadCount);
                return;
            }
        case "sweep":
            {
                // Usage: sweep <sizes_csv> <runs> <threads_csv>
                // e.g.:  sweep 256,384,512 7 1,2,4,8
                if (args.Length != 4) { PrintUsage(); return; }

                static int[] ParseCsvInts(string s)
                    => Array.ConvertAll(s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries), int.Parse);

                int[] sizes = ParseCsvInts(args[1]);
                int runs = int.Parse(args[2]);
                int[] threads = ParseCsvInts(args[3]);

                foreach (int sz in sizes)
                {
                    int m = sz, k = sz, n = sz;
                    foreach (int t in threads)
                    {
                        Console.WriteLine($"[SWEEP] size={sz} runs={runs} threads={t}");
                        Benchmark.Run(m, k, n, runs, t); // результаты пишутся/дописываются в bench_results.csv
                    }
                }
                Console.WriteLine("Sweep done. See bench_results.csv");
                return;
            }

        default:
            PrintUsage();
            return;
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine("[ERROR] " + ex.GetType().Name + ": " + ex.Message);
}
