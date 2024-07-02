using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Xunit;
// ReSharper disable InconsistentNaming

namespace Monads.Test.Part4;

static partial class IOMonadicFunctionExtensions
{
    internal static IO<B> Apply<A, B>(this Func<A, IO<B>> f, IO<A> a) => 
        new(() => f(a.Run()).Run());

    internal static Func<IO<A>, IO<B>> Bind<A, B>(this Func<A, IO<B>> f) => a =>
            new(() => f(a.Run()).Run());
}

[SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
public class MonadicApply : IDisposable
{
    private readonly string _someFile = TestHelper.RandomFileName();

    void IDisposable.Dispose()
    {
        _someFile.Delete();
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
                File.WriteAllText(_someFile, "I'm a side effect!");
                return s.Length;
            });

        Func<int, IO<double>> @double = n =>
            new IO<double>(() =>
            {
                File.AppendAllText(_someFile, "I'm another side effect!");
                return n * 2;
            });

        IO<int> monadicLength = length("foo");
        IO<double> monadicResult = @double.Apply(monadicLength);

        // Indeed, no file has been created yet
        Assert.False(File.Exists(_someFile));

        var result = monadicResult.Run();

        Assert.Equal(3 * 2, result);
        Assert.Equal("I'm a side effect!I'm another side effect!", File.ReadAllText(_someFile));
    }

    [Fact]
    void monadic_apply_as_combinator()
    {
        Func<string, IO<int>> length = s =>
            new IO<int>(() =>
            {
                File.WriteAllText(_someFile, "I'm a side effect!");
                return s.Length;
            });

        Func<int, IO<double>> @double = n =>
            new IO<double>(() =>
            {
                File.AppendAllText(_someFile, "I'm another side effect!");
                return n * 2;
            });

        IO<int> monadicLength = length("foo");
        IO<double> monadicResult = @double.Apply(monadicLength);

        // Indeed, no file has been created yet
        Assert.False(File.Exists(_someFile));

        var result = monadicResult.Run();

        Assert.Equal(3 * 2, result);
        Assert.Equal("I'm a side effect!I'm another side effect!", File.ReadAllText(_someFile));
    }

    [Fact]
    void apply_with_LINQ()
    {
        // Eff<int> monadicResult =
        //     from len in length("foo")
        //     from d in double(len)
        //     select d;
    }

}
