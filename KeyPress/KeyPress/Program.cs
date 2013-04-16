using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyPress
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();
                Console.WriteLine(keyinfo.Key + " was pressed");
            }
            while (1 == 1);
            
        }
    }
}
