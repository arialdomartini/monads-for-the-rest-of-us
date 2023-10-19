using System;
using System.IO;
using Xunit;

namespace Monads.Test.Part4;

static partial class IOMonadicFunctionExtensions
{
    internal static Func<A, IO<C>> ComposedWith<A, B, C>(this Func<B, IO<C>> g, Func<A, IO<B>> f)
    {
        return a =>
        {
            IO<B> ioB = f(a);
            B b = ioB.Run();
            IO<C> c = g(b);
            return c;
        };
    }
    
    internal static Func<A, IO<C>> ComposedWithBasedOnApply<A, B, C>(this Func<B, IO<C>> f, Func<A, IO<B>> g)
    {
        return a =>
        {
            IO<B> ioB = g(a);
            IO<C> ioC = f.Apply(ioB);
            return ioC;
        };
    }
}

public class MonadicComposition
{
    [Fact]
    void IO_monadic_function_composition()
    {
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

        var composed = @double.ComposedWith(length);

        IO<double> monadicResult = composed("foo");
        var result = monadicResult.Run();

        Assert.Equal(3*2, result);
        Assert.Equal("I'm a side effect!I'm another side effect!", File.ReadAllText("output.txt"));
    }

    [Fact]
    void IO_monadic_function_composition_base_on_apply()
    {
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

        var composed = @double.ComposedWithBasedOnApply(length);

        IO<double> monadicResult = composed("foo");
        var result = monadicResult.Run();

        Assert.Equal(3*2, result);
        Assert.Equal("I'm a side effect!I'm another side effect!", File.ReadAllText("output.txt"));
    }
}
