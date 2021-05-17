using Chevalresk_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chevalresk_1.Controllers
{
    public class PaniersController : Controller
    {
        private DBEntities DB = new DBEntities();

        //not sure if i even want that.
        public ActionResult Index()
        {
            return View();
        }
        [UserAccess]
        public ActionResult ItemsList(List<ItemView> items)
        {
            return PartialView(items);
        }
        [UserAccess]
        public ActionResult Facture()
        {
            return PartialView();
        }
        [UserAccess]
        [HttpPost]
        public ActionResult Add(ItemView model, int quantite)
        {
            PanierView panierView = new PanierView();
            panierView.ItemId = model.Id;
            int userID = OnlineUsers.CurrentUser.Id;

            if (!DB.DejaDansPanier(userID, model.Id))
            {
                panierView.UserId = userID;//CurrentUser.
                panierView.QuantiteAchat = quantite;
                DB.CreatePanier(panierView);
                return RedirectToAction("Index", "Items");
            }
            return RedirectToAction("Details", "Items", new { id = model.Id });
        }
        [UserAccess]
        public ActionResult Purchase()//prob current user.
        {
            int userID = OnlineUsers.CurrentUser.Id;
            TempData["Total"] = DB.CalculerMontantPanier(userID);

            User user = DB.Users.Find(userID);
            ViewBag.Solde = user.Montant;

            List<PanierView> panier = DB.FetchPanier(OnlineUsers.CurrentUser.Id);
            List<int> quant = new List<int>();
            foreach (PanierView p in panier)
            {
                quant.Add(p.QuantiteAchat);
            }

            ViewBag.Quantites = quant;
            return View(GetPanierItems());
        }
        [UserAccess]
        [HttpPost]
        public ActionResult Remove(int itemID)//itemviewId
        {
            DB.RemovePanier(itemID, OnlineUsers.CurrentUser.Id);
            return RedirectToAction("Purchase");
        }
        
        private List<ItemView> GetPanierItems()
        {
            List<PanierView> panier = DB.FetchPanier(OnlineUsers.CurrentUser.Id);//currentuser id
            List<ItemView> itemList = new List<ItemView>();

            foreach (PanierView p in panier)
            {
                Item item = DB.Items.Find(p.ItemId);
                itemList.Add(item.ToItemView());
            }
            return itemList;
        }
        [UserAccess]
        [HttpPost]
        public ActionResult Modifier(int itemId, int quantity)
        {
            DB.UpdatePanier(itemId, quantity, OnlineUsers.CurrentUser.Id);//current user.
            return RedirectToAction("Purchase");
        }
        [UserAccess]
        public ActionResult Acheter()
        {
            int userID = OnlineUsers.CurrentUser.Id;
            decimal montantPanier = DB.CalculerMontantPanier(userID);

            User user = DB.Users.Find(userID);
            if (user.Montant >= montantPanier)
            {
                DB.PanierToInventaire(userID);
                DB.ModifierQuantiteItems(DB.FetchPanier(userID));
                DB.ViderPanier(userID);
                DB.ModifierMontant(userID, montantPanier);
            }
            else
            {
                return RedirectToAction("Purchase");
            }

            return RedirectToAction("Index", "Inventaires");
        }

    }
}