﻿using System;
using Xunit;
// ReSharper disable InconsistentNaming

namespace Monads.Test.Part3;

public class FunctionApplication
{
    [Fact]
    void native_csharp_function_application()
    {
        Func<string, int> myLength = s => s.Length;

        var length = myLength("foo");

        Assert.Equal(3, length);
    }

    [Fact]
    void reimplementing_native_csharp_function_application()
    {
        Func<string, int> myLength = s => s.Length;

        int Apply(Func<string, int> f, string a) => f(a);

        var length = Apply(myLength, "foo");

        Assert.Equal(3, length);
    }
    
    [Fact]
    void reimplementing_native_csharp_function_application_as_an_extension_method()
    {
        Func<string, int> myLength = s => s.Length;

        var length = myLength.Apply("foo");

        Assert.Equal(3, length);
        
        myLength("foo");
        myLength.Apply("foo");
        myLength.Invoke("foo");
    }
    
    [Fact]
    void extending_apply()
    {
        B Apply<A, B>(Func<A, B> f, A a)
        {
            Log.Information("Got a value {A}", a);
            var b = f(a);
            Log.Information("Returning a value {B}", b);
            return b;
        }
        
        Func<string, int> myLength = s => s.Length;

        // also logs
        var length = Apply(myLength, "foo");

        Assert.Equal(3, length);
    }
    
    [Fact]
    void multiple_parameters()
    {
        Func<string, string, int> f = (s, z) => s.Length + z.Length;

        // this does not compile
        // f.Apply("foo", "bar");
    }
    
    [Fact]
    void manual_currying()
    {
        Func<string, Func<string, int>> f = s => z => s.Length + z.Length;

        Assert.Equal(
            f.Apply("foo").Apply("bar"),
            f("foo")("bar"));
    }
}

static class FunctionExtensions
{
    internal static B Apply<A,B>(this Func<A, B> f, A s) => f(s);
}

static class Log
{
    internal static void Information<A>(string s, A a)
    {
        // here the logging logic
    }
}
