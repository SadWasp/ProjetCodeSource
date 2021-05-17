using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Chevalresk_1.Models
{
    public static class DBEntitiesExtensionsMethods
    {
        //TRANSACTION
        private static DbContextTransaction Transaction
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return (DbContextTransaction)HttpContext.Current.Session["Transaction"];
                }
                return null;
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Session["Transaction"] = value;
                }
            }
        }

        private static void BeginTransaction(DBEntities DB)
        {
            if (Transaction != null)
                throw new Exception("Transaction en cours! Impossible d'en démarrer une nouvelle!");
            Transaction = DB.Database.BeginTransaction();
        }
        private static void Commit()
        {
            if (Transaction != null)
            {
                Transaction.Commit();
                Transaction.Dispose();
                Transaction = null;
            }
            else
                throw new Exception("Aucune transaction en cours! Impossible de mettre à jour la base de données!");
        }

        //ITEMS
        public static List<ItemView> ItemList(this DBEntities DB)
        {
            List<ItemView> itemsLst = new List<ItemView>();
            if (DB.Items != null)
            {
                foreach (Item item in DB.Items)
                    itemsLst.Add(item.ToItemView());
            }
            return itemsLst.OrderBy(i => i.Prix).ToList();
        }
        public static ItemView ToItemView(this Item item)
        {
            typeItem newType = typeItem.Arme;
            if (item.Type == typeItem.Armure.ToString())
                newType = typeItem.Armure;
            if (item.Type == typeItem.Potion.ToString())
                newType = typeItem.Potion;
            if (item.Type == typeItem.Ressource.ToString())
                newType = typeItem.Ressource;
            ItemView itemView = new ItemView();
            itemView.Id = item.Id;
            itemView.Name = item.Name;
            itemView.Prix = (int)item.Prix;
            itemView.Type = newType;
            itemView.QuantiteStock = item.QuantiteStock;
            itemView.ImageId = item.ImageId;
            itemView.RatingsAverage = item.RatingsAverage;
            itemView.NbRatings = item.NbRatings;
            itemView.flag = item.flag;

            itemView.description = "";
            itemView.duree = 0;
            itemView.matiere = "";
            itemView.genreArme = genreArme.Arc;
            itemView.efficaciteArme = "";
            itemView.matiere = "";
            itemView.poids = 0;
            itemView.taille = "";

            return itemView;
        }
        public static Item ToItem(this ItemView itemView)
        {
            return new Item()
            {
                Id = itemView.Id,
                Name = itemView.Name,
                Prix = itemView.Prix,
                Type = itemView.Type.ToString(),
                QuantiteStock = itemView.QuantiteStock,
                ImageId = itemView.ImageId,
                RatingsAverage = itemView.RatingsAverage,
                NbRatings = itemView.NbRatings,
                flag = itemView.flag
            };
        }
        public static List<ItemView> ItemViewList(List<Item> items)
        {
            List<ItemView> itemLst = new List<ItemView>();
            foreach (Item item in items)
                itemLst.Add(item.ToItemView());
            return itemLst;
        }
        public static ItemView AddItem(this DBEntities DB, ItemView itemView)
        {
            itemView.SaveImage();
            Item item = itemView.ToItem();
            //BeginTransaction(DB);
            item = DB.Items.Add(item);
            DB.SaveChanges();
            //Commit();
            return item.ToItemView();
        }
        public static void AddType(this DBEntities DB, ItemView itemView)
        {
            Arme arme = new Arme();
            Armure armure = new Armure();
            Potion potion = new Potion();
            Ressource ressource = new Ressource();
            if (itemView.Type == typeItem.Arme)
            {
                BeginTransaction(DB);
                arme.genre = itemView.genreArme.ToString();
                arme.efficacite = itemView.efficaciteArme;
                arme.idItems = itemView.Id;
                DB.Armes.Add(arme);
            }
            else if (itemView.Type == typeItem.Armure)
            {
                BeginTransaction(DB);
                armure.matiere = itemView.matiere;
                armure.Poids = itemView.poids;
                armure.taille = itemView.taille;
                armure.idItems = itemView.Id;
                DB.Armures.Add(armure);
            }
            else if (itemView.Type == typeItem.Potion)
            {
                BeginTransaction(DB);
                potion.duree = itemView.duree;
                potion.effet = itemView.effet;
                potion.idItems = itemView.Id;
                DB.Potions.Add(potion);
            }
            else if (itemView.Type == typeItem.Ressource)
            {
                BeginTransaction(DB);
                ressource.description = itemView.description;
                ressource.idItems = itemView.Id;
                DB.Ressources.Add(ressource);
            }
            DB.SaveChanges();
            Commit();
        }
        public static bool RemoveItem(this DBEntities DB, ItemView itemView)
        {
            BeginTransaction(DB);
            itemView.flag = 0;
            Item item = DB.Items.Find(itemView.Id);
            DB.Entry(item).State = EntityState.Modified;

            item.flag = 0;

            DB.SaveChanges();
            Commit();
            return true;
        }
        public static ItemView EditItem(this DBEntities DB, ItemView itemView)
        {
            BeginTransaction(DB);
            itemView.SaveImage();
            Item item = itemView.ToItem();
            item.Type = itemView.Type.ToString();
            DB.Entry(item).State = EntityState.Modified;
            if (item.Type == typeItem.Arme.ToString())
            {
                Arme arme = DB.Armes.Where(i => i.idItems.Equals(item.Id)).FirstOrDefault();
                arme.efficacite = itemView.efficaciteArme;
                arme.genre = itemView.genreArme.ToString();
                DB.Entry(arme).State = EntityState.Modified;
            }
            else if (item.Type == typeItem.Armure.ToString())
            {
                Armure armure = DB.Armures.Where(i => i.idItems.Equals(item.Id)).FirstOrDefault();
                armure.matiere = itemView.matiere;
                armure.Poids = itemView.poids;
                armure.taille = itemView.taille;
                DB.Entry(armure).State = EntityState.Modified;
            }
            else if (item.Type == typeItem.Potion.ToString())
            {
                Potion potion = DB.Potions.Where(i => i.idItems.Equals(item.Id)).FirstOrDefault();
                potion.duree = itemView.duree;
                potion.effet = itemView.effet;
                DB.Entry(potion).State = EntityState.Modified;
            }
            else if (item.Type == typeItem.Ressource.ToString())
            {
                Ressource ressource = DB.Ressources.Where(i => i.idItems.Equals(item.Id)).FirstOrDefault();
                ressource.description = itemView.description;
                DB.Entry(ressource).State = EntityState.Modified;
            }
            DB.SaveChanges();
            Commit();
            return item.ToItemView();
        }
        public static bool ModifierQuantiteItems(this DBEntities DB, List<PanierView> lst)
        {
            BeginTransaction(DB);
            foreach (PanierView pv in lst)
            {
                Item item = DB.Items.Find(pv.ItemId);
                item.QuantiteStock -= pv.QuantiteAchat;
            }
            DB.SaveChanges();
            Commit();
            return true;
        }
        public static bool UpdateItem(this DBEntities DB, ItemView itemView)
        {
            itemView.SaveImage();
            Item item = DB.Items.Find(itemView.Id);
            BeginTransaction(DB);
            DB.Entry(item).State = EntityState.Modified;
            DB.SaveChanges();
            Commit();
            return true;
        }
        public static bool EnleverDoublons(this DBEntities DB, ItemView itemView, List<Item> checkList)
        {
            List<Item> itemsList = DB.Items.ToList();
            foreach (var item in itemsList)
            {
                if (item.Name == itemView.Name)
                {
                    checkList.Add(item);
                    if (checkList.Count() > 1)
                        return true;
                }
            }
            return false;
        }

        //USERS
        public static UserView ToUserView(this User user)
        {
            return new UserView()
            {
                Id = user.Id,
                AvatarId = user.AvatarId,
                Alias = user.Alias,
                FullName = user.FullName,
                Password = user.Password,
                Montant = user.Montant,
                Admin = user.Admin
            };
        }
        public static bool UserNameExist(this DBEntities DB, string Alias)
        {
            User user = DB.Users.Where(u => u.Alias == Alias).FirstOrDefault();
            return (user != null);
        }
        public static User FindByUserName(this DBEntities DB, string Alias)
        {
            User user = DB.Users.Where(u => u.Alias == Alias).FirstOrDefault();
            return user;
        }
        public static UserView AddUser(this DBEntities DB, UserView userView)
        {
            userView.SaveImage();
            User user = userView.ToUser();
            user = DB.Users.Add(user);
            DB.SaveChanges();
            return user.ToUserView();
        }
        public static bool UpdateUser(this DBEntities DB, UserView userView)
        {
            userView.SaveImage();
            User userToUpdate = DB.Users.Find(userView.Id);
            userView.CopyToUser(userToUpdate);
            DB.Entry(userToUpdate).State = EntityState.Modified;
            DB.SaveChanges();
            return true;
        }
        public static bool RemoveUser(this DBEntities DB, UserView userView)
        {
            User user = userView.ToUser();
            List<InventaireView> inventaires = DB.FetchInventaire(user.Id);
            foreach (InventaireView i in inventaires)
            {
                Inventaire inventaire = DB.Inventaires.Find(i.Id);
                DB.Inventaires.Remove(inventaire);
            }

            List<Rating> rat = DB.Ratings.ToList().Where(i => i.UserId.Equals(user.Id)).ToList();
            if (rat.Count > 0)
                foreach (Rating r in rat)
                    DB.RemoveRating(r.ToRatingView());

            DB.SaveChanges();
            userView.RemoveImage();
            User userToDelete = DB.Users.Find(userView.Id);
            DB.Users.Remove(userToDelete);
            DB.SaveChanges();
            return true;
        }
        public static List<UserView> UserList(this DBEntities DB)
        {
            List<UserView> users = new List<UserView>();
            if (DB.Users != null)
            {
                foreach (User user in DB.Users)
                {
                    users.Add(user.ToUserView());
                }
            }
            return users.OrderBy(f => f.Alias).ToList();
        }
        public static bool ModifierMontant(this DBEntities DB, int userID, decimal aPayer)
        {
            User user = DB.Users.Find(userID);

            BeginTransaction(DB);

            user.Montant -= aPayer;

            DB.Entry(user).State = EntityState.Modified;
            DB.SaveChanges();
            Commit();
            return true;
        }
        public static bool AugmenterCapital(this DBEntities DB, int userID, decimal nouveau)
        {
            User user = DB.Users.Find(userID);
            BeginTransaction(DB);
            user.Montant = nouveau;
            DB.Entry(user).State = EntityState.Modified;
            DB.SaveChanges();
            Commit();
            return true;
        }

        //PANIERS
        public static Panier ToPanier(this PanierView panierView)
        {
            return new Panier()
            {
                Id = panierView.Id,
                ItemId = panierView.ItemId,
                UserId = panierView.UserId,
                QuantiteAchat = panierView.QuantiteAchat,
            };
        }
        public static PanierView ToPanierView(this Panier panier)
        {
            return new PanierView()
            {
                Id = panier.Id,
                ItemId = panier.ItemId,
                UserId = panier.UserId,
                QuantiteAchat = panier.QuantiteAchat,
            };
        }
        public static PanierView CreatePanier(this DBEntities DB, PanierView panierView)
        {
            Panier panier = panierView.ToPanier();
            panier = DB.Paniers.Add(panier);
            DB.SaveChanges();
            return panier.ToPanierView();
        }
        public static List<PanierView> FetchPanier(this DBEntities DB, int userId)
        {
            List<Panier> lst = new List<Panier>();
            List<PanierView> lstPanierView = new List<PanierView>();

            lst = DB.Paniers.ToList().Where(p => p.UserId.Equals(userId)).ToList();

            foreach (Panier p in lst)
            {
                lstPanierView.Add(p.ToPanierView());
            }

            return lstPanierView;
        }
        public static bool UpdatePanier(this DBEntities DB, int itemId, int NouvelleQuant, int userId)
        {
            Panier panier = DB.Paniers.Where(p => p.ItemId.Equals(itemId) && p.UserId.Equals(userId)).FirstOrDefault();
            BeginTransaction(DB);

            panier.QuantiteAchat = NouvelleQuant;

            DB.Entry(panier).State = EntityState.Modified;
            DB.SaveChanges();
            Commit();
            return true;
        }
        public static bool RemovePanier(this DBEntities DB, int itemId, int userId)
        {
            Panier panier = DB.Paniers.Where(p => p.ItemId.Equals(itemId) && p.UserId.Equals(userId)).FirstOrDefault();
            BeginTransaction(DB);

            DB.Paniers.Remove(panier);


            DB.SaveChanges();
            Commit();
            return true;
        }
        public static bool ViderPanier(this DBEntities DB, int userId)
        {
            List<Panier> lstPanier = DB.Paniers.ToList().Where(p => p.UserId.Equals(userId)).ToList();
            BeginTransaction(DB);

            foreach (Panier p in lstPanier)
            {
                DB.Paniers.Remove(p);
            }

            DB.SaveChanges();
            Commit();
            return true;


        }
        public static decimal CalculerMontantPanier(this DBEntities DB, int userId)
        {
            List<Panier> lstPanier = new List<Panier>();
            decimal montant = 0;
            lstPanier = DB.Paniers.ToList().Where(p => p.UserId.Equals(userId)).ToList();
            foreach (Panier i in lstPanier)
            {
                int quantite = i.QuantiteAchat;
                decimal prix = DB.Items.Where(p => p.Id.Equals(i.ItemId)).FirstOrDefault().Prix;
                montant += quantite * prix;
            }
            return montant;
        }
        public static bool DejaDansPanier(this DBEntities DB, int userId, int itemId)
        {
            Panier pan = DB.Paniers.Where(x => x.UserId.Equals(userId) && x.ItemId.Equals(itemId)).FirstOrDefault();
            if (pan != null)
            {
                return true;
            }
            return false;
        }

        //INVENTAIRE
        public static Inventaire ToInventaire(this InventaireView InventaireView)
        {
            return new Inventaire()
            {
                Id = InventaireView.Id,
                ItemId = InventaireView.ItemId,
                UserId = InventaireView.UserId,
                QuantiteInventaire = InventaireView.QuantiteInventaire,
            };
        }
        public static InventaireView ToInventaireView(this Inventaire inventaire)
        {
            return new InventaireView()
            {
                Id = inventaire.Id,
                ItemId = inventaire.ItemId,
                UserId = inventaire.UserId,
                QuantiteInventaire = inventaire.QuantiteInventaire,
            };
        }
        public static InventaireView CreateInventaire(this DBEntities DB, InventaireView InventaireView)
        {
            Inventaire inventaire = InventaireView.ToInventaire();
            inventaire = DB.Inventaires.Add(inventaire);
            DB.SaveChanges();
            return inventaire.ToInventaireView();
        }
        public static bool RemoveInventaire(this DBEntities DB, InventaireView inventaireView)
        {
            Inventaire inventaire = DB.Inventaires.Find(inventaireView.Id);
            BeginTransaction(DB);

            DB.Inventaires.Remove(inventaire);
            DB.SaveChanges();

            Commit();
            return true;
        }
        public static List<InventaireView> FetchInventaire(this DBEntities DB, int userId)
        {
            List<Inventaire> lst = new List<Inventaire>();
            List<InventaireView> lstInventaireView = new List<InventaireView>();

            lst = DB.Inventaires.ToList().Where(p => p.UserId.Equals(userId)).ToList();

            foreach (Inventaire i in lst)
            {
                lstInventaireView.Add(i.ToInventaireView());
            }

            return lstInventaireView;
        }
        public static bool PanierToInventaire(this DBEntities DB, int userId)
        {
            List<PanierView> panier = DB.FetchPanier(userId);
            BeginTransaction(DB);
            foreach (PanierView p in panier)
            {
                Inventaire dbl = DB.Inventaires.Where(x => x.UserId.Equals(p.UserId) && x.ItemId.Equals(p.ItemId)).FirstOrDefault();

                if (dbl != null)
                {
                    dbl.QuantiteInventaire += p.QuantiteAchat;
                    DB.Entry(dbl).State = EntityState.Modified;
                }
                else
                {
                    DB.Inventaires.Add(new Inventaire()
                    {
                        ItemId = p.ItemId,
                        UserId = p.UserId,
                        QuantiteInventaire = p.QuantiteAchat,
                    });
                }
            }
            Commit();
            DB.SaveChanges();
            return true;
        }
        public static bool IsInInventaire(this DBEntities DB, int userId, int itemId)
        {
            return DB.Inventaires.ToList().Where(p => p.UserId.Equals(userId) && p.ItemId.Equals(itemId)).FirstOrDefault() != null;
        }

        //RATINGS
        public static int UserRating(this DBEntities DB, int userId, int itemId)
        {
            Rating rating = DB.Ratings.Where(r => r.ItemId == itemId && r.UserId == userId).FirstOrDefault();
            if (rating != null)
            {
                return rating.Value;
            }
            return 0;
        }
        public static void UpdateItemRating(this DBEntities DB, int itemId)
        {
            Item itemToUpdate = DB.Items.Find(itemId);
            if (itemToUpdate != null)
            {
                List<Rating> itemRatings = DB.Ratings.Where(f => f.ItemId == itemId).ToList();
                float sum = 0;
                foreach (Rating itemRating in itemRatings)
                {
                    sum += itemRating.Value;
                }
                itemToUpdate.NbRatings = itemRatings.Count;
                if (itemToUpdate.NbRatings > 0)
                    itemToUpdate.RatingsAverage = (float)sum / itemToUpdate.NbRatings;
                else
                    itemToUpdate.RatingsAverage = 0;
                DB.UpdateItem(itemToUpdate.ToItemView());
            }
        }
        public static int AddRating(this DBEntities DB, RatingView ratingView)
        {
            int result = 0;
            Item itemToUpdate = DB.Items.Find(ratingView.ItemId);
            if (itemToUpdate != null)
            {
                if (DB.UserRating(ratingView.UserId, ratingView.ItemId) == 0)
                {
                    DB.NewRating(ratingView);
                }
                else
                {
                    Rating ratingToUpdate = DB.Ratings.Where(r => r.ItemId == ratingView.ItemId && r.UserId == ratingView.UserId).FirstOrDefault();
                    if (ratingToUpdate != null)
                    {
                        ratingView.Id = ratingToUpdate.Id;
                        DB.UpdateRating(ratingView);
                    }
                }
                DB.UpdateItemRating(ratingView.ItemId);
            }
            return result;
        }
        public static bool UpdateRating(this DBEntities DB, RatingView ratingView)
        {

            Rating rating = DB.Ratings.Find(ratingView.Id);
            ratingView.ToRating(rating);
            BeginTransaction(DB);
            DB.Entry(rating).State = EntityState.Modified;
            DB.SaveChanges();
            Commit();
            return true;

        }
        public static RatingView NewRating(this DBEntities DB, RatingView ratingView)
        {
            Rating rating = new Rating();

            ratingView.ToRating(rating);
            BeginTransaction(DB);
            rating = DB.Ratings.Add(rating);
            DB.SaveChanges();
            Commit();
            return rating.ToRatingView();
        }
        public static bool RemoveRating(this DBEntities DB, RatingView ratingView)
        {
            Rating rating = DB.Ratings.Find(ratingView.Id);
            BeginTransaction(DB);
            DB.Ratings.Remove(rating);
            //DB.UpdateItemRating(ratingView.ItemId);
            DB.SaveChanges();
            Commit();


            DB.UpdateItemRating(rating.ItemId);


            return true;
        }
        public static bool UserAlreadyRated(this DBEntities DB, int userId, int itemId)
        {
            Rating rating = DB.Ratings.Where(c => c.UserId == userId && c.ItemId == itemId).FirstOrDefault();
            if (rating != null)
            {
                return true;
            }
            return false;
        }
        public static List<RatingView> RatingsList(this DBEntities DB)
        {
            List<RatingView> ratings = new List<RatingView>();
            if (DB.Ratings != null)
            {
                foreach (Rating rating in DB.Ratings)
                {
                    ratings.Add(rating.ToRatingView());
                }
            }
            return ratings.OrderBy(f => f.Value).ToList();
        }
        public static RatingView FindRating(this DBEntities DB, int userId, int itemId)
        {
            Rating rating = DB.Ratings.Where(c => c.UserId == userId && c.ItemId == itemId).FirstOrDefault();
            return (rating.ToRatingView());
        }
        public static RatingView ToRatingView(this Rating rating)
        {
            return new RatingView()
            {
                Id = rating.Id,
                Value = rating.Value,
                UserId = rating.UserId,
                ItemId = rating.ItemId,
                Comment = rating.Comment,

            };
        }
        public static List<double> FindAllRatingOf(this DBEntities DB, int id)
        {
            List<int> ratingSpread = new List<int> { 0, 0, 0, 0, 0, 0 };
            List<Rating> ratingLst = DB.Ratings.Where(c => c.ItemId == id).ToList();
            List<double> percent = new List<double> { 0, 0, 0, 0, 0, 0};
            int total = 0;
            foreach (Rating r in ratingLst)
            {
                switch (r.Value)
                {
                    case 0:
                        ratingSpread[5]++;
                        break;
                    case 1:
                        ratingSpread[4]++;
                        break;
                    case 2:
                        ratingSpread[3]++;
                        break;
                    case 3:
                        ratingSpread[2]++;
                        break;
                    case 4:
                        ratingSpread[1]++;
                        break;
                    case 5:
                        ratingSpread[0]++;
                        break;
                    default:
                        break;
                }
            }
            foreach (int i in ratingSpread)
                total += i;
            if(total != 0)
                for (int i = 0; i < percent.Count; ++i)
                {
                    percent[i] = ratingSpread[i] * 100 / total;
                }
            return percent;
        }
    }
}