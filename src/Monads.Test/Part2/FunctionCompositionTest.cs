using System;
using System.IO;
using Xunit;

namespace Monads.Test.Part2;

record IO<B>(Func<B> f);

public class FunctionCompositionTest
{
    public FunctionCompositionTest()
    {
        File.Delete("output.txt");
    }

    [Fact]
    void side_effects()
    {
        // string -> int
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
    void length_monadic()
    {
        IOMonad<int> Length(string s)
        {
            return new IOMonad<int>(() =>
            {
                File.WriteAllText("output.txt", "I'm a side effect!");
                return s.Length;
            });
        }

        int Double(int i)
        {
            return i * 2;
        }

        IOMonad<int> monadicLength = Length("foo");
        var length = monadicLength.Run();

        Assert.Equal(3, length);
        Assert.Equal("I'm a side effect!", File.ReadAllText("output.txt"));
    }

    [Fact]
    void function_application()
    {
        int Length(string s) => s.Length;

        int Double(int i) => i * 2;

        int length = Apply(Double, Apply(Length, "foo"));

        Assert.Equal(3, length);
        Assert.Equal("I'm a side effect!", File.ReadAllText("output.txt"));
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
        int Double(int s) => s *2;

        int length = Apply(Double, Apply(Length, "foo"));

        Assert.Equal(6, length);
    }
    
    [Fact]
    void to_monadic_function()
    {
        IOMonad<int> LengthWithSideEffects(string s) =>
            new IOMonad<int>(() =>
            {
                File.WriteAllText("output.txt", "I'm a side effect!");
                return s.Length;
            });

        IOMonad<string> Return(string s)
        {
            return new IOMonad<string>(() =>s);
        }

        // (A -> IO<B>) -> IO<A> -> IO<B>
        IOMonad<B> Apply<A, B>(Func<A, IOMonad<B>> f, IOMonad<A> value)
        {
            return new IOMonad<B>(() =>
            {
                A a = value.Run();
                IOMonad<B> bMonadic = f(a);
                return bMonadic.Run();
            });
        }
        
        // length >>= return "foo";
        IOMonad<int> length = Apply(LengthWithSideEffects, Return("foo"));

        // does not compile
        // Assert.Equal(3, length);
        
        // this fails
        // Assert.Equal("I'm a side effect!", File.ReadAllText("output.txt"));
    }
    
    // Argument type `IO<int>` is not assignable to parameter type `int`
}


record IOMonad<A>(Func<A> action)
{
    public A Run()
    {
        var output = action.Invoke();
        return output;
    }
}
