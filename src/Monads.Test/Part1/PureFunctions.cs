using System.Collections.Generic;
using Xunit;

namespace Monads.Test.Part1;

public class PureFunctions
{
    [Fact]
    void functions_are_equivalent_to_dictionaries()
    {
        int Double(int i) => i * 2;

        Dictionary<int, int> Codomain = new Dictionary<int, int>
        {
            [-1] = -2,
            [0] = 0,
            [1] = 2,
            [2] = 4,
            [3] = 6,
        };
        
        Assert.Equal(Double(2), Codomain[2]);
        Assert.Equal(Double(3), Codomain[3]);
        Assert.Equal(Double(-1), Codomain[-1]);
    }
}
