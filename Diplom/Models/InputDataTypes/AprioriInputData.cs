using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;

namespace Diplom.Models.InputDataTypes
{
    public class AprioriInputData : Research.InputData
    {
        public override bool SetInputData(Stream stream)
        {
            if (stream == null)
            {
                return false;
            }
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(stream);
            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;

            List<List<string>> tranactions = new List<List<string>>();
            // обход всех узлов в корневом элементе
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Name == "Transaction")
                {
                    var newTrans= new List<string>();
                    tranactions.Add(newTrans);

                    // обходим все дочерние узлы элемента user
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        // если узел - company
                        if (childnode.Name == "Product") 
                        {
                            newTrans.Add(childnode.InnerText);
                        }
                    }
                }
            }

            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms,tranactions);
            Input = ms.GetBuffer();

            return true;
        }

        public override object GetInputData()
        {
            MemoryStream memoryStream = new MemoryStream(Input);
            BinaryFormatter formatter = new BinaryFormatter();
            List<List<string>> collection = (List<List<string>>)formatter.Deserialize(memoryStream);
            return collection;
        }
    }
}