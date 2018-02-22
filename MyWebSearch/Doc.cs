using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebSearch
{
    class Doc
    {

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
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
            return Name + "\t" + s;
        }
    }
}
