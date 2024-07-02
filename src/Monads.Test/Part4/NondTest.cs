using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
// ReSharper disable ArrangeTypeModifiers
// ReSharper disable InconsistentNaming

namespace Monads.Test.Part4;

static class NondExtensions
{
    static Nond<T> Return<T>(T value) => new(new List<T>{value});

    internal static Func<Nond<A>, Nond<B>> Map<A, B>(this Func<A, B> f) => nondA => 
        new Nond<B>(nondA.Items.Select(f));
}

record Nond<T>(IEnumerable<T> Items)
{
    internal IEnumerable<T> Run() => Items;
}

public class NondTest
{
    [Fact]
    void map_for_Nond()
    {
        var nondeterministicString = new Nond<string>(new[] 
            { "foo", "bar", "barbaz" });

        Func<string, int> length = s => s.Length;

        var lengthM = NondExtensions.Map(length);

        var results = lengthM(nondeterministicString).Run();
        
        Assert.Equal(new []{3, 3, 6}, results);
    }
}
