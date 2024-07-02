using System;
using System.IO;
using Monads.Test.Part4;
using Xunit;
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeObjectCreationWhenTypeEvident
// ReSharper disable MoveLocalFunctionAfterJumpStatement

namespace Monads.Test.Part2;

internal record IO<A>(Func<A> action)
{
    internal A Run()
    {
        var output = action.Invoke();
        return output;
    }
}

public class FunctionCompositionTest : IDisposable
{
    private readonly string _someFile = TestHelper.RandomFileName();

    void IDisposable.Dispose()
    {
        _someFile.Delete();
    }

    [Fact]
    void side_effects()
    {
        // string -> int
        int LengthWithSideEffects(string s)
        {
            File.WriteAllText(_someFile, "I'm a side effect!");
            return s.Length;
        }

        var length = LengthWithSideEffects("foo");

        Assert.Equal(3, length);
        Assert.Equal("I'm a side effect!", File.ReadAllText(_someFile));
    }

    [Fact]
    void length_monadic()
    {
        IO<int> Length(string s)
        {
            return new IO<int>(() =>
            {
                File.WriteAllText(_someFile, "I'm a side effect!");
                return s.Length;
            });
        }

        IO<int> monadicLength = Length("foo");
        var length = monadicLength.Run();

        Assert.Equal(3, length);
        Assert.Equal("I'm a side effect!", File.ReadAllText(_someFile));
    }

    [Fact]
    void function_application()
    {
        int Length(string s) => s.Length;

        int Twice(int i) => i * 2;

        int length = Apply(Twice, Apply(Length, "foo"));

        Assert.Equal(6, length);
    }

    B Apply<A, B>(Func<A, B> f, A a)
    {
        return f(a);
    }
    
    [Fact]
    void single_function_application()
    {
        int Length(string s) => s.Length;

        int length = Apply(Length, "foo");

        Assert.Equal(3, length);
    }
    
    [Fact]
    void two_function_application()
    {
        int Length(string s) => s.Length;
        int Twice(int s) => s *2;

        int length = Apply(Twice, Apply(Length, "foo"));

        Assert.Equal(6, length);
    }
    
    [Fact]
    void to_monadic_function()
    {
        IO<int> LengthWithSideEffects(string s) =>
            new IO<int>(() =>
            {
                File.WriteAllText(_someFile, "I'm a side effect!");
                return s.Length;
            });

        IO<string> Return(string s)
        {
            return new IO<string>(() =>s);
        }

        // (A -> IO<B>) -> IO<A> -> IO<B>
        IO<B> Apply<A, B>(Func<A, IO<B>> f, IO<A> value)
        {
            return new IO<B>(() =>
            {
                A a = value.Run();
                IO<B> bMonadic = f(a);
                return bMonadic.Run();
            });
        }
        
        // length >>= return "foo";
        IO<int> length = Apply(LengthWithSideEffects, Return("foo"));

        // does not compile
        // Assert.Equal(3, length);
        
        // this fails
        // Assert.Equal("I'm a side effect!", File.ReadAllText(_someFile));
    }
    
    // Argument type `IO<int>` is not assignable to parameter type `int`
}
