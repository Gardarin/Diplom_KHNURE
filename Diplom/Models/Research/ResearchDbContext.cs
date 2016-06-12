using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Diplom.Models.Research
{
    public class ResearchDbContext:DbContext
    {
        public DbSet<Researcher> Researchers { get; set; }
        public DbSet<Research> Researchs { get; set; }
        public DbSet<Algorithm> Algoritms { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<InputData> InputDatas { get; set; }
    }
}