using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diplom.Models.Research
{
    public class Research
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public InputData InputData { get; set; }
        public Algorithm CurentAlgorithm { get; set; }
        public Result CurentResult { get; set; }
    }
}