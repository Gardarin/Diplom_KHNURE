using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Diplom.Models.Research
{
    public class Result
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }

        public virtual List<string> ReturnData()
        {
            //return new string[]{ this.ToString()};
            return null;
        }
    }
}