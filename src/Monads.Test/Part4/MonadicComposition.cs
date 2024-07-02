using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Xunit;
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers

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

[SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
public class MonadicComposition : IDisposable
{
    private readonly string _someFile = TestHelper.RandomFileName();

    void IDisposable.Dispose()
    {
        _someFile.Delete();
    }

    [Fact]
    void IO_monadic_function_composition()
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

        var composed = @double.ComposedWith(length);

        IO<double> monadicResult = composed("foo");
        var result = monadicResult.Run();

        Assert.Equal(3*2, result);
        Assert.Equal("I'm a side effect!I'm another side effect!", File.ReadAllText(_someFile));
    }

    [Fact]
    void IO_monadic_function_composition_base_on_apply()
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

        var composed = @double.ComposedWithBasedOnApply(length);

        IO<double> monadicResult = composed("foo");
        var result = monadicResult.Run();

        Assert.Equal(3*2, result);
        Assert.Equal("I'm a side effect!I'm another side effect!", File.ReadAllText(_someFile));
    }
}
