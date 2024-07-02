using System;
using Xunit;
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantLambdaParameterType
// ReSharper disable PatternAlwaysMatches
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

namespace Monads.Test.Part4;

internal abstract record Maybe<A>{
    internal B Run<B>(Func<A, B> just, Func<B> nothing) =>
        this switch
        {
            Just<A> a => just(a.Value),
            Nothing<A> => nothing()
        };
}

internal static class MaybeExtensions
{
    internal static Func<Maybe<A>, Maybe<B>> Bind<A, B>(this Func<A, Maybe<B>> f) => (Maybe<A> a) =>
        a.Run(
            just: f,
            nothing: () => new Nothing<B>());

    internal static Func<Maybe<A>, Maybe<B>> Map<A, B>(this Func<A, B> f) =>
        maybeA =>
            maybeA.Run<Maybe<B>>(
                just: a => new Just<B>(f(a)),
                nothing: () => new Nothing<B>());

    internal static Maybe<B> Map<A, B>(this Maybe<A> maybeA, Func<A, B> f) =>
        Map(f)(maybeA);

}


internal record Just<A>(A Value) : Maybe<A>;
internal record Nothing<A> : Maybe<A>;

public class MaybeMonadTest
{
    [Fact]
    void map()
    {
        Maybe<string> maybeAString = new Just<string>("foo");

        Func<string, int> length = s => s.Length;

        Func<Maybe<string>,Maybe<int>> lengthF = MaybeExtensions.Map(length);

        var maybeLength = lengthF(maybeAString);

        Assert.IsType<Just<int>>(maybeLength);
        Assert.Equal(3, ((Just<int>) maybeLength).Value);
    }
    
    [Fact]
    void map_as_a_box()
    {
        Maybe<string> maybeAString = new Just<string>("foo");

        Func<string, int> length = s => s.Length;


        var maybeLength = maybeAString.Map(length);

        Assert.IsType<Just<int>>(maybeLength);
        Assert.Equal(3, ((Just<int>) maybeLength).Value);
    }
    
    Maybe<A> Return<A>(A value) => new Just<A>(value);
    
    [Fact]
    void combinator()
    {
        Func<string, Maybe<int>> length = s =>
            s == null
                ? new Nothing<int>()
                : new Just<int>(s.Length);

        var elevatedLength = length.Bind();

        Maybe<int> monadicResult = elevatedLength(Return("foo"));

        var result = monadicResult switch
        {
            Nothing<int> => "got nothing",
            Just<int> { Value: int value } => $"got a {value}"
        };

        Assert.Equal("got a 3", result);
    }
}
