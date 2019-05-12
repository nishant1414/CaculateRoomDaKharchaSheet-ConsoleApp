using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
           // Console.WriteLine("Hello World!");
            var c = new User();
            var data = c.CalculateSheet();
            Console.WriteLine(data);

        }

       
    }
}

