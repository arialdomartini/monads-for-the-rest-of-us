using System;
using Xunit;

namespace Monads.Test.Part3;

public class FunctionComposition
{
    [Fact]
    void composing_pure_type_compatible_functions()
    {
        Func<string, int> length = s => s.Length;
        Func<int, decimal> halfOf = n => (decimal)n / 2;

        decimal halfTheLength = halfOf(length("foo"));
        
        Assert.Equal(1.5M, halfTheLength);
    }
    
    [Fact]
    void manually_writing_the_composite_function()
    {
        Func<string, decimal> lengthThenHalfOf = s =>
        {
            var l = s.Length;
            var halfOfIt = (decimal)l / 2;
            return halfOfIt;
        };

        var halfTheLength = lengthThenHalfOf("foo");

        Assert.Equal(1.5M, halfTheLength);
    }
}
