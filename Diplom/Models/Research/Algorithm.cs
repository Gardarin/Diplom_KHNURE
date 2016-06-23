using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diplom.Models.Research
{
    public class Algorithm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual Result Perform(InputData inputData)
        {
            return null;
        }

        public virtual InputData CreateInputData()
        {
            return null;
        }
    }
}