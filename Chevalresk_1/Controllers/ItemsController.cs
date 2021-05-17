using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Chevalresk_1.Models;

namespace Chevalresk_1.Controllers
{
    public class ItemsController : Controller
    {
        private DBEntities DB = new DBEntities();
        private int NombreEtoiles
        {
            get
            {
                if (Session["NombreEtoiles"] == null)
                {
                    Session["NombreEtoiles"] = 0;
                }
                return (int)Session["NombreEtoiles"];
            }
            set
            {
                Session["NombreEtoiles"] = value;
            }
        }
        private bool SortAscending
        {
            get
            {
                if (Session["SortAscending"] == null)
                {
                    Session["SortAscending"] = true;
                }
                return (bool)Session["SortAscending"];
            }
            set
            {
                Session["SortAscending"] = value;
            }
        }
        private string SearchName
        {
            get
            {
                if (Session["SearchName"] == null)
                {
                    Session["SearchName"] = "";
                }
                return (string)Session["SearchName"];
            }
            set
            {
                Session["SearchName"] = value;
            }
        }
        private bool TypeAll
        {
            get
            {
                if (Session["TypeAll"] == null)
                {
                    Session["TypeAll"] = true;
                }
                return (bool)Session["TypeAll"];
            }
            set
            {
                Session["TypeAll"] = value;
            }
        }
        private bool TypeArme
        {
            get
            {
                if (Session["TypeArme"] == null)
                {
                    Session["TypeArme"] = false;
                }
                return (bool)Session["TypeArme"];
            }
            set
            {
                Session["TypeArme"] = value;
            }
        }
        private bool TypeArmure
        {
            get
            {
                if (Session["TypeArmure"] == null)
                {
                    Session["TypeArmure"] = false;
                }
                return (bool)Session["TypeArmure"];
            }
            set
            {
                Session["TypeArmure"] = value;
            }
        }
        private bool TypePotion
        {
            get
            {
                if (Session["TypePotion"] == null)
                {
                    Session["TypePotion"] = false;
                }
                return (bool)Session["TypePotion"];
            }
            set
            {
                Session["TypePotion"] = value;
            }
        }
        private bool TypeRessource
        {
            get
            {
                if (Session["TypeRessources"] == null)
                {
                    Session["TypeRessources"] = false;
                }
                return (bool)Session["TypeRessources"];
            }
            set
            {
                Session["TypeRessources"] = value;
            }
        }
        public ActionResult Search(string tags, bool[] type, bool all)
        {
            SearchName = tags.Trim();
            TypeAll = all;

            TypeArme = type[0];
            TypeArmure = type[1];
            TypePotion = type[2];
            TypeRessource = type[3];

            return RedirectToAction("Index");
        }
        public ActionResult ToogleSort()
        {
            SortAscending = !SortAscending;
            return RedirectToAction("Index");
        }
        public ActionResult SearchEtoiles(int nb)
        {
            NombreEtoiles = nb;

            return RedirectToAction("Index");
        }
        public ActionResult Index()
        {
            ViewBag.SortAscending = SortAscending;
            ViewBag.SearchName = SearchName;
            ViewBag.TypeAll = TypeAll;
            ViewBag.TypeArme = TypeArme;
            ViewBag.TypeArmure = TypeArmure;
            ViewBag.TypePotion = TypePotion;
            ViewBag.TypeRessource = TypeRessource;
            return View(GetFilteredItems());
        }
        public ActionResult ItemsList(List<ItemView> items)
        {
            return PartialView(items);
        }
        [AdminAccess]
        public ActionResult Create()
        {
            return View(new ItemView());
        }
        [AdminAccess]
        [HttpPost]
        public ActionResult Create(ItemView itemView)
        {
            if (ModelState.IsValid)
            {
                //if (itemView.Type == typeItem.Ressource)
                //{
                //    if (itemView.description == null)
                //    {
                //        ModelState.AddModelError("description", "Obligatoire");
                //        return View(itemView);
                //    }
                //}
                ItemView item = DB.AddItem(itemView);
                itemView.Id = item.Id;
                DB.AddType(itemView);
                return RedirectToAction("Index", "Items");
            }
            return View(itemView);
        }
        private List<ItemView> GetFilteredItems()
        {
            List<Item> items = null;
            List<Item> result = null;

            if (!string.IsNullOrEmpty(SearchName))
                items = DB.Items.ToList().Where(p => p.Name.Contains(SearchName)).ToList();//Is not case sensitive.
            else
                items = DB.Items.ToList();

            List<Item> types = new List<Item>();
            if (TypeArme)
                types.AddRange(DB.Items.ToList().Where(p => p.Type.Contains("Arme")).ToList());
            if (TypeArmure)
                types.AddRange(DB.Items.ToList().Where(p => p.Type.Contains("Armure")).ToList());
            if (TypePotion)
                types.AddRange(DB.Items.ToList().Where(p => p.Type.Contains("Potion")).ToList());
            if (TypeRessource)
                types.AddRange(DB.Items.ToList().Where(p => p.Type.Contains("Ressource")).ToList());

            if (types.Count > 0)
                result = types.Where(a => items.Any(b => a == b)).ToList();
            else if (TypeAll)
                result = items;
            else
                return new List<ItemView>();

            var ordered = result.OrderBy(x => x.Prix).ToList();

            List<decimal> lstPrice = new List<decimal>();
            foreach (Item iv in ordered)
                lstPrice.Add(iv.Prix);
            ViewBag.Prices = lstPrice;

            if (SortAscending)
                ordered = ordered.OrderBy(p => p.Prix).ToList();
            else
                ordered = ordered.OrderByDescending(p => p.Prix).ToList();

            ordered = ordered.Where(p => p.RatingsAverage >= NombreEtoiles).ToList();

            return DBEntitiesExtensionsMethods.ItemViewList(ordered);
        }
        
        public ActionResult Details(int id)
        {
            ViewBag.ListUsers = DB.UserList();
            Session["Message"] = id;
            if (OnlineUsers.CurrentUser != null)
            {
                ViewBag.CanCreate = DB.IsInInventaire(OnlineUsers.CurrentUser.Id, id);
                ViewBag.AlreadyRated = DB.UserAlreadyRated(OnlineUsers.CurrentUser.Id, id);
            }
            else
                ViewBag.AlreadyRated = false;


            
            ViewBag.RatingsSpread = DB.FindAllRatingOf(id);
            ViewBag.Ratings = DB.RatingsList();

            Item item = DB.Items.Find(id);

            ViewBag.NbRating = item.NbRatings;
            ViewBag.Average = item.RatingsAverage;



            ItemView itemView = item.ToItemView();
            if (item.Type == typeItem.Arme.ToString())
            {
                Arme arme = DB.Armes.Where(p => p.idItems.Equals(id)).FirstOrDefault();
                ViewBag.genreArme = arme.genre;
                ViewBag.efficacite = arme.efficacite;
            }
            else if (item.Type == typeItem.Armure.ToString())
            {
                Armure armure = DB.Armures.Where(p => p.idItems.Equals(id)).FirstOrDefault();
                ViewBag.matiere = armure.matiere;
                ViewBag.poids = armure.Poids;
                ViewBag.taille = armure.taille;
            }
            else if (item.Type == typeItem.Potion.ToString())
            {
                Potion potion = DB.Potions.Where(p => p.idItems.Equals(id)).FirstOrDefault();
                ViewBag.duree = potion.duree;
                ViewBag.effet = potion.effet;
            }
            else if (item.Type == typeItem.Ressource.ToString())
            {
                Ressource ressource = DB.Ressources.Where(p => p.idItems.Equals(id)).FirstOrDefault();
                ViewBag.description = ressource.description;
            }
            List<int> N = new List<int>();
            if (itemView.QuantiteStock > 0)
                for (int i = 1; i <= itemView.QuantiteStock; ++i)
                    N.Add(i);
            else
                N.Add(0);
            ViewBag.Nbs = N;
            return View(itemView);
        }
        [AdminAccess]
        public ActionResult Delete(int id)
        {
            Item item = DB.Items.Find(id);
            if (item.Type == typeItem.Arme.ToString())
            {
                Arme arme = DB.Armes.Where(p => p.idItems.Equals(id)).FirstOrDefault();
                ViewBag.genreArme = arme.genre;
                ViewBag.efficacite = arme.efficacite;
            }
            else if (item.Type == typeItem.Armure.ToString())
            {
                Armure armure = DB.Armures.Where(p => p.idItems.Equals(id)).FirstOrDefault();
                ViewBag.matiere = armure.matiere;
                ViewBag.poids = armure.Poids;
                ViewBag.taille = armure.taille;
            }
            else if (item.Type == typeItem.Potion.ToString())
            {
                Potion potion = DB.Potions.Where(p => p.idItems.Equals(id)).FirstOrDefault();
                ViewBag.duree = potion.duree;
                ViewBag.effet = potion.effet;
            }
            else if (item.Type == typeItem.Ressource.ToString())
            {
                Ressource ressource = DB.Ressources.Where(p => p.idItems.Equals(id)).FirstOrDefault();
                ViewBag.description = ressource.description;
            }
            return View(DB.Items.Find(id).ToItemView());
        }
        [HttpPost]
        public ActionResult Delete(ItemView itemView)
        {
            DB.RemoveItem(itemView);
            return RedirectToAction("Index");
        }
        [AdminAccess]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = DB.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            if (item.Type == typeItem.Arme.ToString())
            {
                Arme arme = DB.Armes.Where(p => p.idItems.Equals(item.Id)).FirstOrDefault();
                ViewBag.genreArme = arme.genre;
                ViewBag.efficacite = arme.efficacite;

                //TempData["matiere"] = "";

                ViewBag.matiere = "";
                ViewBag.poids = 0;
                ViewBag.taille = "";
                ViewBag.duree = 0;
                ViewBag.effet = "";
                ViewBag.description = "";
            }
            else if (item.Type == typeItem.Armure.ToString())
            {
                Armure armure = DB.Armures.Where(p => p.idItems.Equals(item.Id)).FirstOrDefault();
                ViewBag.matiere = armure.matiere;
                ViewBag.poids = armure.Poids;
                ViewBag.taille = armure.taille;

                ViewBag.genreArme = "";
                ViewBag.efficacite = "";
                ViewBag.duree = 0;
                ViewBag.effet = "";
                ViewBag.description = "";
            }
            else if (item.Type == typeItem.Potion.ToString())
            {
                Potion potion = DB.Potions.Where(p => p.idItems.Equals(item.Id)).FirstOrDefault();
                ViewBag.duree = potion.duree;
                ViewBag.effet = potion.effet;

                ViewBag.genreArme = "";
                ViewBag.efficacite = "";
                ViewBag.matiere = "";
                ViewBag.poids = 0;
                ViewBag.taille = "";
                ViewBag.description = "";
            }
            else if (item.Type == typeItem.Ressource.ToString())
            {
                Ressource ressource = DB.Ressources.Where(p => p.idItems.Equals(item.Id)).FirstOrDefault();
                ViewBag.description = ressource.description;

                ViewBag.duree = 0;
                ViewBag.matiere = "";
                ViewBag.genreArme = "";
                ViewBag.efficacite = "";
                ViewBag.matiere = "";
                ViewBag.poids = 0;
                ViewBag.taille = "";
            }
            return View(item.ToItemView());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ItemView itemView)
        {
            if (ModelState.IsValid)
            {
                ItemView item = DB.EditItem(itemView);
                itemView.Id = item.Id;
                return RedirectToAction("Index");
            }
            return View(itemView);
        }
        public ActionResult About()
        {
            return View();
        }
    }
}