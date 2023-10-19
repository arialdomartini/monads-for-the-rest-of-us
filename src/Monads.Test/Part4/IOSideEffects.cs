using System;
using System.IO;
using Xunit;

namespace Monads.Test.Part4;

public class IOSideEffects
{
    record IO<B>(Func<B> f)
    {
        internal B Run() => f();
    }

    [Fact]
    void running_the_IO_monad()
    {
        IO<int> CalculateWithSideEffect(string s) =>
            new IO<int>(() =>
            {
                File.WriteAllText("output.txt", "I'm a side effect!");
                return s.Length;
            });

        // This is still a pure function
        IO<int> monadicValue = CalculateWithSideEffect("foo");

        // Indeed, no file has been created yet
        Assert.False(File.Exists("output.txt"));

        // Finally, the IO monadic value is run
        var result = monadicValue.Run();

        Assert.Equal(3, result);
        Assert.Equal("I'm a side effect!", File.ReadAllText("output.txt"));
    }
}
