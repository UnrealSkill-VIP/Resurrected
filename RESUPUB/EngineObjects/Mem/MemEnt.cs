using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB.EngineObjects.Mem
{
    public class MemEnt
    {
        public int BaseAddr;

        public bool IsValid = false;

        public MemEnt(int BaseAddr)
        {
            if(BaseAddr > 0)
            {
                IsValid = false;
            }
            else
            {
                IsValid = true;
            }
        }

    }
}
