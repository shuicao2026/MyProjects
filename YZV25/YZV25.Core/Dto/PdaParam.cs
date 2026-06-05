
using System;
using System.Threading.Tasks;

namespace YZV25.Dto
{
    public class PdaParam
    {
        public int id { get; set; }
        public string code { get; set; }

        public string face { get; set; }

     
    }


    public class PdaForceParam
    {
        public int id { get; set; }
        public string code { get; set; }
        public string pwd { get; set; }
        public string face { get; set; }


    }
}