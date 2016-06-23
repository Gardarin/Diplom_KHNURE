using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Diplom.Models.Research;

namespace Diplom.Models.AllResults
{
    public class AprioriResult:Result   
    {
        public override List<string> ReturnData()
        {
            if (Data == null)
            {
                return base.ReturnData();
            }
            MemoryStream ms = new MemoryStream(Data);

            BinaryFormatter formatter = new BinaryFormatter();
            List<string> collection = (List<string>)formatter.Deserialize(ms);
            return collection;
        }
    }
}