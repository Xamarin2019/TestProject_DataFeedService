using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppNetMQ
{
    public interface IConsole
    {
        void Write(string value);
        void WriteLine(string value);
        void WriteLine(string format, object arg0);
        ConsoleKeyInfo ReadKey();
        string ReadLine();
        void Clear();
        void SetCursorPosition(int left, int top);
    }

    class ConsoleImpl : IConsole
    {
        public void Clear()
        {
            System.Console.Clear();
        }

        public ConsoleKeyInfo ReadKey()
        {
            return System.Console.ReadKey();
        }

        public string ReadLine()
        {
            return System.Console.ReadLine();
        }

        public void SetCursorPosition(int left, int top)
        {
            System.Console.SetCursorPosition(left, top);
        }

        public void Write(string value)
        {
            System.Console.Write(value);
        }

        public void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }

        public void WriteLine(string format, object arg0)
        {
            System.Console.WriteLine(format, arg0);
        }
    }

    public static class Console
    {
        public static IConsole console = new ConsoleImpl();

        public static void Clear()
        {
            console.Clear();
        }

        public static ConsoleKeyInfo ReadKey()
        {
            return console.ReadKey();
        }

        public static string ReadLine()
        {
            return console.ReadLine();
        }

        public static void SetCursorPosition(int left, int top)
        {
            console.SetCursorPosition(left, top);
        }

        public static void Write(string value)
        {
            console.Write(value);
        }

        public static void WriteLine(string value)
        {
            console.WriteLine(value);
        }

        public static void WriteLine(string format, object arg0)
        {
            console.WriteLine(format, arg0);
        }
    }
}
