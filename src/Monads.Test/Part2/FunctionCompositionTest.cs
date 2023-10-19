using System;
using System.IO;
using Xunit;

namespace Monads.Test.Part2;

record IO<B>(Func<B> f);

public class FunctionCompositionTest
{
    internal FunctionCompositionTest()
    {
        File.Delete("output.txt");
    }

    [Fact]
    void side_effects()
    {
        int LengthWithSideEffects(string s)
        {
            File.WriteAllText("output.txt", "I'm a side effect!");
            return s.Length;
        }

        var length = LengthWithSideEffects("foo");

        Assert.Equal(3, length);
        Assert.Equal("I'm a side effect!", File.ReadAllText("output.txt"));
    }

    [Fact]
    void to_monadic_function()
    {
        IO<int> LengthWithSideEffects(string s) =>
            new IO<int>(() =>
            {
                File.WriteAllText("output.txt", "I'm a side effect!");
                return s.Length;
            });

        IO<int> length = LengthWithSideEffects("foo");

        // does not compile
        // Assert.Equal(3, length);
        Assert.Equal("I'm a side effect!", File.ReadAllText("output.txt"));
    }

    [Fact]
    void function_application()
    {
        // string -> int
        int LengthWithSideEffects(string s)
        {
            Console.Write("I'm a side effect!");
            return s.Length;
        }

        // int -> double
        double Double(int i) => i * 2;

        // string -> int -> double
        double doubleTheLength = Double(LengthWithSideEffects("foo"));

        Assert.Equal(6, doubleTheLength);
    }
    
    // Argument type `IO<int>` is not assignable to parameter type `int`
}
