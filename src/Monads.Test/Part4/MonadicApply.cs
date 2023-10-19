using System;
using System.IO;
using Xunit;

namespace Monads.Test.Part4;

static class IOMonadicFunctionExtensions
{
    internal static IO<B> Apply<A, B>(this Func<A, IO<B>> f, IO<A> a) => 
        new IO<B>(() =>
        {
            A aResult = a.Run();
            IO<B> bResult = f(aResult);
            return bResult.Run();
        });
}

public class MonadicApply
{
    public MonadicApply()
    {
        File.Delete("output.txt");
    }
    
    [Fact]
    void native_csharp_apply()
    {
        // Apply :: (A -> B) -> A -> B
        // Apply :: (A -> IO<B>) -> IO<A> -> IO<B>

        // length :: string -> int
        // double :: int -> double
        Func<string, int> length = s => s.Length;
        Func<int, decimal> @double = n => (decimal)n * 2;

        var doubleTheLength = @double(length("foo"));

        Assert.Equal(6, doubleTheLength);
    }
    
    [Fact]
    void monadic_apply()
    {
        // Apply :: (A -> B) -> A -> B
        // Apply :: (A -> IO<B>) -> IO<A> -> IO<B>

        // length :: string -> IO<int>
        // double :: int -> IO<double>
        Func<string, IO<int>> length = s =>
            new IO<int>(() =>
            {
                File.WriteAllText("output.txt", "I'm a side effect!");
                return s.Length;
            });

        Func<int, IO<double>> @double = n =>
            new IO<double>(() =>
            {
                File.AppendAllText("output.txt", "I'm another side effect!");
                return n * 2;
            });

        IO<int> monadicLength = length("foo");
        IO<double> monadicResult = @double.Apply(monadicLength);

        // Indeed, no file has been created yet
        Assert.False(File.Exists("output.txt"));

        var result = monadicResult.Run();

        Assert.Equal(3*2, result);
        Assert.Equal("I'm a side effect!I'm another side effect!", File.ReadAllText("output.txt"));

    }
}
