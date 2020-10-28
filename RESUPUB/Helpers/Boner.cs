using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB.Helpers
{
    public class Boner
    {
        public static int[] GetBonesByModel(string model)
        {
            if (model.Contains("ctm_idf"))
                return new int[] { 0, 6, 39, 9, 77, 71 };
            else if (model.Contains("ctm_st6"))
                return new int[] { 0, 6, 36, 8, 79, 66 };
            else if (model.Contains("ctm_sas"))
                return new int[] { 0, 6, 36, 8, 79, 66 };
            else if (model.Contains("ctm_swat"))
                return new int[] { 0, 6, 38, 10, 74, 68 };
            else if (model.Contains("tm_leet"))
                return new int[] { 0, 6, 36, 8, 79, 66 };
            else if (model.Contains("tm_balkan"))
                return new int[] { 0, 6, 36, 8, 79, 66 };
            else if (model.Contains("tm_seperatist"))
                return new int[] { 0, 6, 36, 8, 79, 66 };
            else if (model.Contains("tm_professional"))
                return new int[] { 0, 6, 37, 9, 77, 71 };
            return new int[] { 6, 39, 9, 77, 71 };
        }

    }
}
