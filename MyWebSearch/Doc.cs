using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebSearch
{
    class MyDoc: IComparable<MyDoc>
    {

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string group;

        public string Group
        {
            get { return group; }
            set { group = value; }
        }

        private bool activ;

        public bool Activ
        {
            get { return activ; }
            set { activ = value; }
        }

        public override string ToString()
        {
            string s = "true";

            if (this.activ == false)
            {
                s = "false";
            }
            return Group + "\t" + Name + "\t" + s;
        }

        public int CompareTo(MyDoc comparePart)
        {
            // A null value means that this object is greater.
            if (comparePart == null)
                return 1;

            else
                return (this.group + this.Name).CompareTo(comparePart.Group + comparePart.Name);
            //return this.Name.CompareTo(comparePart.Name);
        }
    }
}
