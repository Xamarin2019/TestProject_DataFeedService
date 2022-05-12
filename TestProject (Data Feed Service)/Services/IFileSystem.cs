using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppNetMQ
{
    public interface IFileSystem
    {
        string[] GetFileEntries();
    }
}
