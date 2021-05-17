using Chevalresk_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chevalresk_1.Controllers
{
    public class InventairesController : Controller
    {
        private DBEntities DB = new DBEntities();
        [UserAccess]
        public ActionResult Index()
        {
            int userID = OnlineUsers.CurrentUser.Id;
            User user = DB.Users.Find(userID);
            ViewBag.Solde = user.Montant;
            return View(GetInventaireItems(userID));
        }
        [AdminAccess]
        public ActionResult InventaireJoueur(int id)
        {
            User user = DB.Users.Find(id);
            ViewBag.userId = id;
            ViewBag.Solde = user.Montant;
            return View(GetInventaireItems(id));
        }
        [UserAccess]
        public ActionResult ItemsList(List<ItemView> items)
        {
            return PartialView(items);
        }
        private List<ItemView> GetInventaireItems(int id)
        {
            List<InventaireView> inventaire = DB.FetchInventaire(id);
            List<ItemView> itemList = new List<ItemView>();
            List<int> nb = new List<int>();

            foreach (InventaireView i in inventaire)
            {
                Item item = DB.Items.Find(i.ItemId);
                itemList.Add(item.ToItemView());
                nb.Add(i.QuantiteInventaire);
            }

            ViewBag.Nb = nb;

            return itemList;
        }
    }
}