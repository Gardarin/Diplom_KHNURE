using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Diplom.Models.Research
{
    public class InputData
    {
        public int Id { get; set; }

        public byte[] Input { get; set; }

        public virtual bool SetInputData(Stream stream)
        {
            if (stream == null)
            {
                return false;
            }
            return true;
        }

        public virtual object GetInputData()
        {
            MemoryStream stream = new MemoryStream(Input);
            return stream;
        }
    }
}