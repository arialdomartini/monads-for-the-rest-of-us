using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Monads.Test.Part1;

public class PureFunctions
{
    [Fact]
    void functions_are_equivalent_to_dictionaries()
    {
        int Double(int i) => i * 2;

        Dictionary<int, int> Codomain = new Dictionary<int, int>
        {
            [-1] = -2,
            [0] = 0,
            [1] = 2,
            [2] = 4,
            [3] = 6,
        };
        
        Assert.Equal(Double(2), Codomain[2]);
        Assert.Equal(Double(3), Codomain[3]);
        Assert.Equal(Double(-1), Codomain[-1]);
    }
    
    [Fact]
    void closures_are_impure()
    {
        var b = string.Empty;

        // `string -> int`
        int Closure(string a) => a.Length + b.Length;
        Assert.Equal(3, Closure("foo"));

        b = "wat?";
        Assert.Equal(7, Closure("foo"));
    }
    
    [Fact]
    void exceptions_make_functions_dishonest()
    {
        decimal Divide(decimal n, decimal d) => n / d;

        Assert.Equal(3M, Divide(9M, 3M));
        Assert.Equal(4.5M, Divide(9M, 2M));

        Assert.Throws<DivideByZeroException>(() => Divide(9M, 0M));
    }
    
    // (decimal | DivideByZeroException) Divide(decimal n, decimal d) => n / d;
    
    // static double divide(int numerator, int denominator) throws ArithmeticException {
    //         if (denominator == 0) {
    //         throw new ArithmeticException("Denominator cannot be zero");
    //     }
    //     return (double) numerator / denominator;
    // }

    [Fact]
    void functions_with_IO_side_effects_are_dishonest()
    {
        var oldConsole = Console.Out;
        var output = new StringBuilder();
        Console.SetOut(new StringWriter(output));
        
        int Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            return 0;
        }

        Main(Array.Empty<string>());
        
        Assert.Equal("Hello, World!\r\n", output.ToString());
        
        Console.SetOut(oldConsole);
    }
    
/*    
    | Case                                                                  | Example of type                           |
    |-----------------------------------------------------------------------|-------------------------------------------|
    | A function that depends (reads) an extra string parameter             | `string --[Reader<String>]--> int`        |
    | A function that might raise an exception                              | `decimal -> decimal --[Error]--> decimal` |
    | A function that also writes to the Console                            | `string[] --[IO]--> int`                  |
    | A function that could fail to return a value                          | `string --[Maybe]--> int`                 |
    | A function returning nondeterministic values                          | `string --[Nondeterministic]--> int`      |
    | A function returning a value and also writing a double somewhere else | `string --[Writer<double>] --> int`       |
    | A function which depends and updates a shared state                   | `string --[State<MyState>]--> int`        |
*/

/*
    | Case                                                                  | Example of type                   |
    |-----------------------------------------------------------------------|-----------------------------------|
    | A function that depends (reads) an extra string parameter             | `string -> Reader<String, int>`   |
    | A function that might raise an exception                              | `decimal -> Error<decimal>`       |
    | A function that also writes to the Console                            | `string[] -> IO<int>`             |
    | A function that could fail to return a value                          | `string -> Maybe<int>`            |
    | A function returning nondeterministic values                          | `string -> Nondeterministic<int>` |
    | A function returning a value and also writing a double somewhere else | `string -> Writer<double, int>`   |
    | A function which depends and updates a shared state                   | `string -> State<MyState, int`    |
*/
}
