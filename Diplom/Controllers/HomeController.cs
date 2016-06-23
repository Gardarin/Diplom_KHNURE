using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Diplom.Models;
using Diplom.Models.Research;

namespace Diplom.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            Researcher reseacher = null;
            string userId = "";
            userId = User.Identity.GetUserId();
            if (userId == "")
            {
                return View();
            }
            using (ResearchDbContext rdbc = new ResearchDbContext())
            {
                reseacher = rdbc.Researchers.Include("Researches.CurentAlgorithm").FirstOrDefault(x => x.UserId == userId);
            }
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Researcher = reseacher;
            }
            return View();
        }

        [HttpGet]
        public ActionResult CreateResearch()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateResearch(Research research)
        {
            research.CurentAlgorithm = null;
            research.CurentResult = null;
            research.State = "Created";
            using (ResearchDbContext rdbc = new ResearchDbContext())
            {
                string userId = User.Identity.GetUserId();
                var res = rdbc.Researchers.Include("Researches.CurentAlgorithm").FirstOrDefault(x => x.UserId == userId).Researches;
                if (res != null)
                {
                    res.Add(research);
                }
                rdbc.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ApplicationDbContext adc = new ApplicationDbContext();
            int a= adc.Users.Count();

            ViewBag.Message = string.Format("Count {0}; isIdenty {1}",a,User.Identity.IsAuthenticated);

            return View();
        }

        public ActionResult DeleteResearch(int id)
        {
            Researcher reseacher = null;
            string userId = "";
            userId = User.Identity.GetUserId();
            if (userId == "")
            {
                return RedirectToAction("Index");
            }
            using (ResearchDbContext rdbc = new ResearchDbContext())
            {
                reseacher = rdbc.Researchers.Include("Researches").FirstOrDefault(x => x.UserId == userId);
                rdbc.Researchs.Remove(rdbc.Researchs.FirstOrDefault(x => x.Id == id));
                reseacher.DeleteResearch(id);
                rdbc.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult SetAlgorithm(int id)
        {
            Researcher reseacher = null;
            string userId = "";
            userId = User.Identity.GetUserId();
            if (userId == "")
            {
                return RedirectToAction("Index");
            }
            using (ResearchDbContext rdbc = new ResearchDbContext())
            {
                reseacher = rdbc.Researchers.Include("Researches.CurentAlgorithm").FirstOrDefault(x => x.UserId == userId);
                ViewBag.CurentAlgorithm = reseacher.Researches.FirstOrDefault(x => x.Id == id).CurentAlgorithm;
                ViewBag.Algorithms = rdbc.Algoritms.ToList();
                ViewBag.ResearchId = id;
                rdbc.SaveChanges();
            }
            return View();
        }

        public ActionResult SetlectAlgorithm(int id,int researchId)
        {
            Researcher reseacher = null;
            string userId = "";
            userId = User.Identity.GetUserId();
            if (userId == "")
            {
                return RedirectToAction("Index");
            }
            using (ResearchDbContext rdbc = new ResearchDbContext())
            {
                reseacher = rdbc.Researchers.Include("Researches.CurentAlgorithm").FirstOrDefault(x => x.UserId == userId);
                reseacher.Researches.FirstOrDefault(x => x.Id == researchId).CurentAlgorithm = 
                    rdbc.Algoritms.FirstOrDefault(x => x.Id == id);
                rdbc.SaveChanges();
            }
            return RedirectToAction("SetAlgorithm/" + researchId);
        }

        public ActionResult Result(int id)
        {
            string userId = "";
            userId = User.Identity.GetUserId();
            if (userId == "")
            {
                return RedirectToAction("Index");
            }
            using (ResearchDbContext rdbc = new ResearchDbContext())
            {
                ViewData.Add("Result", rdbc.Researchs.Include("CurentResult").FirstOrDefault(x => x.Id == id).CurentResult);
            }
            return View();
        }

        public ActionResult StartResearch(int id)
        {
            string userId = "";
            userId = User.Identity.GetUserId();
            if (userId == "")
            {
                return RedirectToAction("Index");
            }
            using (ResearchDbContext rdbc = new ResearchDbContext())
            {
                var research= rdbc.Researchs.Include("CurentAlgorithm").Include("InputData").Include("CurentResult").FirstOrDefault(x => x.Id == id);
                if (research.CurentResult != null)
                {
                    rdbc.Results.Remove(research.CurentResult);
                }
                research.CurentResult = research.CurentAlgorithm.Perform(research.InputData);
                rdbc.SaveChanges();
            }
            return RedirectToAction("TheResearch/" + id);
        }

        public ActionResult TheResearch(int id)
        {
            string userId = "";
            userId = User.Identity.GetUserId();
            if (userId == "")
            {
                return RedirectToAction("Index");
            }
            Research research = null;
            using (ResearchDbContext rdbc = new ResearchDbContext())
            {
                research = rdbc.Researchs.Include("CurentAlgorithm").Include("CurentResult").Include("InputData").FirstOrDefault(x => x.Id == id);
            }
            return View(research);
        }

        [HttpPost]
        public ActionResult SetInputData(int id, HttpPostedFileBase file = null)
        {
            string userId = "";
            userId = User.Identity.GetUserId();
            if (userId == "")
            {
                return RedirectToAction("Index");
            }
            using (ResearchDbContext rdbc = new ResearchDbContext())
            {
                if (file != null)
                {
                    InputData inputData = rdbc.Researchs.Include("CurentAlgorithm").FirstOrDefault(x => x.Id == id).CurentAlgorithm.CreateInputData();
                    inputData.SetInputData(file.InputStream);
                    rdbc.InputDatas.Remove(rdbc.Researchs.Include("InputData").FirstOrDefault(x => x.Id == id).InputData);
                    rdbc.Researchs.Include("InputData").FirstOrDefault(x => x.Id == id).InputData = inputData;
                    rdbc.SaveChanges();
                }
            }
            return RedirectToAction("TheResearch/" + id); ;
        }

        #region ResultPartial

        public ActionResult AprioriResultPartial()
        {
            return PartialView();
        }

        #endregion

    }
}