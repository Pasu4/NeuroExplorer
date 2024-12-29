using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class FakeDir
    {
        public string name;
        public string location;

        public FakeDir(string location, string name)
        {
            this.location = location;
            this.name = name;
        }
    }
}
