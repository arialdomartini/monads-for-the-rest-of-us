using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Monads.Test.Part4;

record IO<B>(Func<B> f)
{
    internal B Run() => f();
}
static class IOExtensions
{
    internal static Func<IO<A>, IO<B>> Map<A, B>(this Func<A, B> f) =>
        ioa => new IO<B>(() => f(ioa.Run()));
 
    internal static IO<B> Map<A, B>(this IO<A> ioa, Func<A, B> f) =>
        f.Map()(ioa);
    
    internal static Func<IO<A>, IO<B>> Map2<A, B>(this Func<A, B> f) =>
        ioa => new IO<B>(() => f(ioa.Run()));
}

public class IOSideEffects : IDisposable
{
    private readonly string _someFile = TestHelper.RandomFileName();

    void IDisposable.Dispose()
    {
        _someFile.Delete();
    }
    
    [Fact]
    void map_test()
    {
        var io = new IO<string>(() =>
        {
            File.WriteAllText(_someFile, "I'm a side effect");
            return "foo";
        });

        Func<string, int> length = s => s.Length;
        {
            var lengthM = length.Map();

            var l = lengthM(io);
            
            var result = l.Run();
            Assert.Equal(3, result);
            Assert.Equal("I'm a side effect", File.ReadAllText(_someFile));
        }
        {
            List<string> s = new List<string>();
            s.Select(length);
            IO<int> l = io.Map(length);
            
            var result = l.Run();
            Assert.Equal(3, result);
            Assert.Equal("I'm a side effect", File.ReadAllText(_someFile));
        }
    }

    [Fact]
    void running_the_IO_monad()
    {
        IO<int> CalculateWithSideEffect(string s) =>
            new IO<int>(() =>
            {
                File.WriteAllText(_someFile, "I'm a side effect!");
                return s.Length;
            });

        // This is still a pure function
        IO<int> monadicValue = CalculateWithSideEffect("foo");

        // Indeed, no file has been created yet
        Assert.False(File.Exists(_someFile));

        // Finally, the IO monadic value is run
        var result = monadicValue.Run();

        Assert.Equal(3, result);
        Assert.Equal("I'm a side effect!", File.ReadAllText(_someFile));
    }

    /*
Eff<int> Computation1() => ...
Eff<int> Computation2() => ...
Eff<int> Computation3() => ...

Eff<int> result =
    from value1 in Computation1()
    from value2 in Computation2(value1)
    from value3 in Computation3(value2, value1)
    select value2;



Option<int> Computation1() => ...
Option<int> Computation2() => ...
Option<int> Computation3() => ...

Option<int> result =
    from value1 in Computation1()
    from value2 in Computation2(value1)
    from value3 in Computation3(value2, value1)
    select value2;


Either<Error, int> Computation1() => ...
Either<Error, int> Computation2() => ...
Either<Error, int> Computation3() => ...

Either<Error, int> result =
    from value1 in Computation1()
    from value2 in Computation2(value1)
    from value3 in Computation3(value2, value1)
    select value2;


IEnumerable<int> Computation1() => ...
IEnumerable<int> Computation2() => ...
IEnumerable<int> Computation3() => ...

IEnumerable<int> result =
    from value1 in Computation1()
    from value2 in Computation2(value1)
    from value3 in Computation3(value2, value1)
    select value2;

*/
}
