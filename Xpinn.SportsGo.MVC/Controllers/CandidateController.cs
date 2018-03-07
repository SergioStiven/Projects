using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.MVC.Models;
using Xpinn.SportsGo.Services;

namespace Xpinn.SportsGo.MVC.Controllers
{
    public class CandidateController : BaseController
    {
        #region Views
        // GET: Personal information of group
        public ActionResult InfoCandidate()
        {
            return View();
        }

        //-- PARTIALS VIEWS --//

        // GET: Filters of candidate
        public ActionResult FiltersCandidate()
        {
            return View();
        }

        // GET: Soccer court
        public ActionResult SoccerCourtCandidate()
        {
            return View();
        }

        // GET: Basketball court
        public ActionResult BasketballCourtCandidate()
        {
            return View();
        }

        // GET: Baseball court
        public ActionResult BaseballCourtCandidate()
        {
            return View();
        }

        // GET: Volleyball court
        public ActionResult VolleyballCourtCandidate()
        {
            return View();
        }
        #endregion

        #region Methods

        #endregion
    }
}