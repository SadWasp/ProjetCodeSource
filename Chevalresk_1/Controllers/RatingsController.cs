using Chevalresk_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chevalresk_1.Controllers
{
    public class RatingsController : Controller
    {
        private DBEntities DB = new DBEntities();

        [UserAccess]
        public ActionResult AverageGraph(int itemId)
        {
            List<double> ratingViewList = new List<double> { 0, 0, 0, 0, 0 };
            ratingViewList = DB.FindAllRatingOf(itemId);
            if (ratingViewList != null && ratingViewList.Count > 0)
            {
                ViewBag.RatingsSpread = ratingViewList;
                return PartialView();
            }
            return null;
        }

        [UserAccess]
        public ActionResult RatingsAverage(int itemId)
        {
            Item item = DB.Items.Find(itemId);
            if (item != null)
            {
                ViewBag.Average = item.RatingsAverage;
                ViewBag.AverageText = item.RatingsAverage.ToString("0.0") + " (" + item.NbRatings + ")";
                return PartialView();
            }
            return null;
        }

        [UserAccess]
        public ActionResult Create()
        {
            RatingView ratingView = new RatingView();
            ViewBag.UserRating = DB.UserRating(ratingView.UserId, ratingView.ItemId);
            return View(ratingView);
        }

        [HttpPost]
        [UserAccess]
        public ActionResult Create(RatingView ratingView)
        {

            if (ModelState.IsValid)
            {
                ratingView.ItemId = (int)Session["Message"];
                ratingView.UserId = OnlineUsers.CurrentUser.Id;

                DB.AddRating(ratingView);
                return RedirectToAction("Index", "Items");
            }
            return View(ratingView);
        }

        [UserAccess]
        public ActionResult CommentsList(List<RatingView> ratings)
        {
            ViewBag.Ratings = DB.RatingsList();
            ViewBag.itemId = (int)Session["Message"];
            
            return PartialView(ratings);
        }

        [UserAccess]
        public ActionResult Edit()
        {
            int itemId = (int)Session["Message"];

            RatingView ratingView = DB.FindRating(OnlineUsers.CurrentUser.Id, itemId);
            ViewBag.UserRating = ratingView.Value;
            return View(ratingView);
        }

        [HttpPost]
        public ActionResult Edit(RatingView ratingView)
        {
            if (ModelState.IsValid)
            {
                ratingView.ItemId = (int)Session["Message"];
                ratingView.UserId = OnlineUsers.CurrentUser.Id;
                
                DB.AddRating(ratingView);
            }
            else
            {
                return View(ratingView);
            }
            return RedirectToAction("Index", "Items");
        }

        [AdminAccess]
        public ActionResult Delete(int id)
        {
            RatingView ratingView = DB.Ratings.Find(id).ToRatingView();
            ViewBag.UserRating = ratingView.Value;
            //DB.UpdateItemRating(ratingView.ItemId);

            return View(ratingView);
        }

        [AdminAccess]
        [HttpPost]
        public ActionResult Delete(RatingView ratingView)
        {
            DB.RemoveRating(ratingView);
            //DB.UpdateItemRating(ratingView.ItemId);
            return RedirectToAction("Index", "Items");
        }

    }
}
