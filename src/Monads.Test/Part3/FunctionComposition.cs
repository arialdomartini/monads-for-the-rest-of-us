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
    
    // length           :: string -> int
    // halfOf           :: int    -> decimal
    // lengthThenHalfOf :: string -> decimal

    [Fact]
    void function_composition()
    {
        Func<string, int> length = s => s.Length;
        Func<int, decimal> halfOf = n => (decimal)n / 2;

        // (int -> decimal) -> (string -> int) -> (string -> decimal)
        Func<string, decimal> Compose(Func<int, decimal> f, Func<string, int> g) => s => f(g(s));
        
        Func<string, decimal> halfOfLength = Compose(halfOf, length);

        Assert.Equal(1.5M, halfOfLength("foo"));
    }
    
    [Fact]
    void generic_function_composition()
    {
        Func<string, int> length = s => s.Length;
        Func<int, decimal> halfOf = n => (decimal)n / 2;

        // Compose :: (B -> C) -> (A -> B) -> (A -> C)
        Func<A, C> Compose<A, B, C>(Func<B, C> g, Func<A, B> f) => a => g(f(a));
        
        Func<string, decimal> halfOfLength = Compose(halfOf, length);

        Assert.Equal(1.5M, halfOfLength("foo"));
    }
    
    [Fact]
    void function_composition_as_an_extension_method()
    {
        Func<string, int> length = s => s.Length;
        Func<int, decimal> halfOf = n => (decimal)n / 2;

        Func<string, decimal> halfOfLength = halfOf.ComposedWith(length);

        Assert.Equal(1.5M, halfOfLength.Apply("foo"));
    }
}


static class FunctionCompositionExtensions
{
    internal static Func<A, C> ComposedWith<A, B, C>(this Func<B, C> g, Func<A, B> f) => a => g(f(a));
}
