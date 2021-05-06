using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class Viewmodel
    {
        private bool runEventHandler = true;

        public bool RunEventHandler
        {
            get { return runEventHandler; }
            set { runEventHandler = value; }
        }

    }
}
