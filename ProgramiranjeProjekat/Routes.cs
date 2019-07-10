using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramiranjeProjekat
{
    class Routes
    {
        public class coordinates
        {
            public string lat { get; set; }
            public string lon { get; set; }
        }

        public class lines
        {
            public string name { get; set; }
            public string description { get; set; }
            public List<coordinates> coordinates { get; set; }
            public List<string> timeTable { get; set; }
        }

        public class stops
        {
            public string name { get; set; }
            public string lat { get; set; }
            public string lon { get; set; }
            public List<string> lines { get; set; }
        }

        public class RootObject
        {
            public List<lines> lines { get; set; }
            public List<stops> stops { get; set; }
        }
    }
}
