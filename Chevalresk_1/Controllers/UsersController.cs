using Chevalresk_1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Chevalresk_1.Controllers
{
    public class UsersController : Controller
    {
        private DBEntities DB = new DBEntities();
        private DateTime OnLineUsersLastUpdate
        {
            get
            {
                if (Session["OnLineUsersUpdate"] == null)
                    Session["OnLineUsersUpdate"] = new DateTime(0);
                return (DateTime)Session["OnLineUsersUpdate"];
            }
            set
            {
                Session["OnLineUsersUpdate"] = value;
            }
        }
        protected override void Dispose(bool disposing)
        {
            DB.Dispose();
            base.Dispose(disposing);
        }
        [AdminAccess]
        public ActionResult Index()
        {
            ViewBag.CurrentUserId = OnlineUsers.CurrentUser.Id;
            return View(DB.UserList());
        }
        [AdminAccess]
        public ActionResult OnlineUsersList(List<UserView> users)
        {
            return PartialView(users);
        }
        public ActionResult Subscribe()
        {
            return View(new UserView());
        }
        [HttpPost]
        public ActionResult Subscribe(UserView userView)
        {
            if (ModelState.IsValid)
            {
                if (DB.UserNameExist(userView.Alias))
                {
                    ModelState.AddModelError("UserName", "Ce nom d'usager existe déjà.");
                    return View(userView);
                }
                userView.Password = userView.NewPassword;
                DB.AddUser(userView);
                return RedirectToAction("Login");
            }
            return View(userView);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginView loginView)
        {
            if (ModelState.IsValid)
            {
                User userFound =
                DB.Users.Where(u => u.Alias == loginView.Alias).FirstOrDefault();
                if (userFound == null)
                {
                    ModelState.AddModelError("Alias", "Ce nom d'utilisateur n'existe pas.");
                    return View();
                }
                else
                {
                    if (userFound.Password != loginView.Password)
                    {
                        ModelState.AddModelError("Password", "Mauvais mot de passe.");
                        return View();
                    }
                }
                OnlineUsers.AddSessionUser(userFound.ToUserView());
            }
            else
                return View();

            return RedirectToAction("Index", "Items");
        }
        public ActionResult Logout()
        {
            OnlineUsers.RemoveSessionUser();
            return RedirectToAction("Login");
        }
        [UserAccess]
        public ActionResult Profil()
        {
            UserView userView = OnlineUsers.GetSessionUser();
            ViewBag.oldPassword = userView.Password;
            return View(userView);
        }
        [HttpPost]
        public ActionResult Profil(UserView userView)
        {
            if (ModelState.IsValid)
            {
                string oldPassword = userView.Password;

                if (userView.NewPassword.Equals(oldPassword))
                {
                    User user = DB.Users.Find(userView.Id);
                    userView.Password = user.Password;
                }
                else
                {
                    userView.Password = userView.NewPassword;
                }
                DB.UpdateUser(userView);
                userView.CopyToUserView(OnlineUsers.GetSessionUser());
                OnlineUsers.LastUpdate = DateTime.Now;
            }
            else
            {
                return View(userView);
            }
            return RedirectToAction("Index", "Items");
        }
        [AdminAccess]
        public ActionResult Delete(int id)
        {
            return View(DB.Users.Find(id).ToUserView());
        }
        [AdminAccess]
        [HttpPost]
        public ActionResult Delete(UserView userView)
        {
            DB.RemoveUser(userView);
            return RedirectToAction("Index");
        }
        [AdminAccess]
        public ActionResult Details(int id)
        {
            return View(DB.Users.Find(id).ToUserView());
        }
        [AdminAccess]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = DB.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user.ToUserView());
        }


        [AdminAccess]
        public ActionResult Augmenter(int id)
        {
            User user = DB.Users.Find(id);
            //ViewBag.montant = user.Montant;

            ViewBag.Montant = user.Montant;

            return View(user.ToUserView());
        }

        [HttpPost]
        [AdminAccess]
        public ActionResult Augmenter(int id, decimal? nouveauMontant)
        {
            decimal capital = 0;
            if (nouveauMontant != null)
                capital = (decimal)nouveauMontant;

            User user = DB.Users.Find(id);
            ViewBag.Montant = user.Montant;
            if (user.Montant < nouveauMontant)
            {
                DB.AugmenterCapital(user.Id, capital);
                return RedirectToAction("Index");
            }
            return View(user.ToUserView());
        }
    }
}