using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT
{
    public class Attribute
    {
        private string name;
        private List<string> values;

        public Attribute(string name, List<string> values)
        {
            this.Name = name;
            this.Values = values;
        }


        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<string> Values
        {
            get { return values; }
            set {
                this.values = new List<string>(value);
            }
        }
    }
}
