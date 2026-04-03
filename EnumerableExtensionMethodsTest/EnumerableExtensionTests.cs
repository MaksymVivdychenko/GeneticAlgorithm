using System.Xml.Schema;
using GeneticAlgorithm.ExtensionMethods;

namespace EnumerableExtensionMethodsTest;

public class EnumerableExtensionTests
{
    [Fact]
    public void TakeMax_ValidData_ReturnsCorrectResult()
    {
        int[] data = [1, 15, 7, 10, 12];
        int n = 2;
        int[] expectedResult = [12, 15];
        var result = data.TakeMax(x => x, n);
        Assert.Equivalent(expectedResult, result);
    }

    [Fact]
    public void TakeMax_InvalidArgument_ThrowsArgumentException()
    {
        int[] data = [1, 15, 7, 10, 12];
        int n = -2;

        Assert.Throws<ArgumentException>(() => data.TakeMax(x => x, n));
    }

    [Fact]
    public void TakeMax_ZeroArgument_ReturnsEmpty()
    {
        int[] data = [1, 15, 7, 10, 12];
        int n = 0;

        var result = data.TakeMax(x => x, n);
        Assert.True(!result.Any());
    }
}