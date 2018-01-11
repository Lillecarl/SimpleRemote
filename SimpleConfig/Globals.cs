using System;
using System.IO;

namespace SimpleConfig
{
    public static class Globals
    {
        public static string AppData { get; private set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimpleConfig");
    }
}
