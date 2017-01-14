using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.Execute
{
    class Register
    {
        private ushort data;
        
        public ushort Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }
    }
}
