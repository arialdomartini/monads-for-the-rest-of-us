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
    
    [Fact]
    void closures_are_impure()
    {
        var b = string.Empty;

        // `string -> int`
        int Closure(string a) => a.Length + b.Length;
        Assert.Equal(3, Closure("foo"));

        b = "wat?";
        Assert.Equal(7, Closure("foo"));
    }
}
