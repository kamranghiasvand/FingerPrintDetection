using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FingerPrintDetectionWeb.Models
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class DatatablesParam
    {
        public int draw { get; set; }
        private int _start;

        public int start
        {
            get
            {
                if (_start < 0)
                    _start = 0;
                return _start;
            }
            set { _start = value; }
        }

        private int _lenght;

        public int length
        {
            get
            {
                if (_lenght < 0)
                    _lenght = 25;
                return _lenght;
            }
            set { _lenght = value; }
        }

        public List<Ordering> order = new List<Ordering>();
        public List<Column> columns = new List<Column>();
        public string customSearch { get; set; }
        public Dictionary<string, string> customSearchDic { get; set; }

        public DatatablesParam()
        {
            length = 25;
            customSearchDic = new Dictionary<string, string>();
        }

    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Ordering
    {
        public int column { get; set; }
        public string dir { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Column
    {

        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public Dictionary<string, object> search { get; set; } = new Dictionary<string, object>();

    }
}
