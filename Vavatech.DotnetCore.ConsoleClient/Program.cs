using System;

namespace Vavatech.DotnetCore.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello .NET!");
           // OverflowTest();

            DivideByZeroTest();
        }

        private static void OverflowTest()
        {
            byte x = 255;

            checked
            {
                x++;
                x++;
            }
        }

        private static void DivideByZeroTest()
        {
            float x = 10;
            float y = 0;

            float result = x / y;
        }
    }
}
