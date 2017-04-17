using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScannerDriver
{
    class Program
    {
        public static void Main()
        {
            var cmd = CommandRunner.Create();
            cmd.Start();
        }
    }
}
