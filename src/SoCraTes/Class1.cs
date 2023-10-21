using Xunit;

namespace SoCraTes;

public class Class1
{
    [Fact]
    void closure()
    {
        var a = 0;
        // int -> doubles
        double Twice(int i)
        {
            return i * 2 + a;
        }
        
        Assert.Equal(8, Twice(4));

        a = 2;
        
        Assert.Equal(8+2, Twice(4));

    }
}
