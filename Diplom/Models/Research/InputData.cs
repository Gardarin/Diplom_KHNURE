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

        public bool SetInputData(MemoryStream stream)
        {
            if (stream == null)
            {
                return false;
            }
            Input = new byte[stream.Length];
            stream.Read(Input, 0, Input.Count());
            return true;
        }

        public MemoryStream GetInputData()
        {
            MemoryStream stream = new MemoryStream(Input);
            return stream;
        }
    }
}