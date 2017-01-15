

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
