using System;
using System.IO;
using Xunit;

namespace Monads.Test.Part4;

record IO<B>(Func<B> f)
{
    internal B Run() => f();
}

public class IOSideEffects
{
    public IOSideEffects()
    {
        File.Delete("output.txt");
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
