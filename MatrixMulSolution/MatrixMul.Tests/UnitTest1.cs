using MatrixMul.App;
using Xunit;

namespace MatrixMul.Tests;

public sealed class MatrixTests
{
    [Fact]
    public void Multiply_SmallMatrices_MatchSequential()
    {
        var A = new IntMatrix(2, 3);
        A[0, 0] = 1; A[0, 1] = 2; A[0, 2] = 3;
        A[1, 0] = 4; A[1, 1] = 5; A[1, 2] = 6;

        var B = new IntMatrix(3, 2);
        B[0, 0] = 7; B[0, 1] = 8;
        B[1, 0] = 9; B[1, 1] = 10;
        B[2, 0] = 11; B[2, 1] = 12;

        var seq = SequentialMultiplier.Multiply(A, B);
        var par = ThreadedMultiplier.Multiply(A, B, threadCount: 3);

        Assert.Equal(seq.Rows, par.Rows);
        Assert.Equal(seq.Cols, par.Cols);
        for (int i = 0; i < seq.Rows; i++)
            for (int j = 0; j < seq.Cols; j++)
                Assert.Equal(seq[i, j], par[i, j]);
    }

    [Fact]
    public void Multiply_IncompatibleSizes_Throws()
    {
        var A = new IntMatrix(2, 3);
        var B = new IntMatrix(4, 5);
        Assert.Throws<ArgumentException>(() => SequentialMultiplier.Multiply(A, B));
        Assert.Throws<ArgumentException>(() => ThreadedMultiplier.Multiply(A, B, 2));
    }

    [Fact]
    public void Random_Generation_IsDeterministicBySeed()
    {
        var m1 = MatrixGen.CreateRandom(5, 7, seed: 123);
        var m2 = MatrixGen.CreateRandom(5, 7, seed: 123);
        for (int i = 0; i < m1.Rows; i++)
            for (int j = 0; j < m1.Cols; j++)
                Assert.Equal(m1[i, j], m2[i, j]);
    }
}
