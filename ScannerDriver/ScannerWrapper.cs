using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suprema;

namespace ScannerDriver
{
    public class ScannerWrapper
    {
        private UFScanner scanner;

        public ScannerWrapper(UFScanner scanner)
        {
            if (scanner == null)
                throw new ArgumentNullException(nameof(scanner));
            this.scanner = scanner;
        }
    }
}
