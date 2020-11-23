using System;
using System.Collections.Generic;
using System.Text;

namespace MinimSharp
{
    public class MinimSharp
    {
        public static bool DEBUG = false;

        public MinimSharp()
        {

        }

        public static void Error(string message)
        {
            Console.WriteLine("=== Minim Error ===");
            Console.WriteLine("=== " + message);
            Console.WriteLine();
        }

        public static void Debug(string message)
        {
            if (DEBUG)
            {
                string[] lines = message.Split("\n");
                Console.WriteLine("=== Minim Debug ===");
                for (int i = 0; i < lines.Length; i++)
                {
                    Console.WriteLine("=== " + lines[i]);
                }
                Console.WriteLine();
            }
        }
        
        public void DebugOn()
        {
            DEBUG = true;
        }

        public void DebugOff()
        {
            DEBUG = false;
        }
    } // class 
} // namespace
