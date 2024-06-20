using System;
using System.IO;

namespace Monads.Test.Part4;

static class TestHelper
{
    internal static string RandomFileName() => 
        Guid.NewGuid().ToString();

    internal static void Delete(this string fileName)
    {
        File.Delete(fileName);
    } 
}