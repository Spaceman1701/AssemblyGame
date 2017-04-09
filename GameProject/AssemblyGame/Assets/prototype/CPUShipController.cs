using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace prototype
{
    class CPUShipController : ship.ShipController
    {
        protected override void DoStart()
        {
            health = 50;
        }

        protected override void DoUpdate()
        {
            
        }
    }
}
