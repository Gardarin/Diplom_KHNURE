using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Diplom.Models.Research
{
    public class Researcher
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public List<Research> Researches { get; set; }

        public void AddResearch(string name, string description, Algorithm algorithm)
        {
            Research research = new Research() {Name=name, State="created", Description=description, 
                CurentAlgorithm=algorithm, CurentResult= new Result() };
            Researches.Add(research);
        }

        public bool DeleteResearch(int id)
        {
            return Researches.Remove(Researches.FirstOrDefault(x => x.Id == id));
        }
        
        public Result StartResearch(int id)
        {
            var res = Researches.FirstOrDefault(x => x.Id == id);
            if (res == null)
            {
                return null;
            }
            if (res.InputData == null || res.CurentAlgorithm == null)
            {
                return null;
            }
            Result result = res.CurentAlgorithm.Perform(res.InputData);
            return result;
        }

        public bool SetAlgorithm(int id, Algorithm algorithm)
        {
            if (algorithm == null)
            {
                return false;
            }
            var res = Researches.FirstOrDefault(x => x.Id == id);
            if (res == null)
            {
                return false;
            }
            res.CurentAlgorithm = algorithm;
            return true;
        }

        public bool SetInputData(int id, InputData inputData)
        {
            if (inputData == null)
            {
                return false;
            }
            var res = Researches.FirstOrDefault(x => x.Id == id);
            if (res == null)
            {
                return false;
            }
            res.InputData = inputData;
            return true;
        }

        public InputData GetInputData(int researcherId, int researchId)
        {
            throw new NotImplementedException();
        }
    }
}