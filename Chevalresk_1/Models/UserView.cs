using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Chevalresk_1.Models
{
    public class UserView
    {
        public int Id { get; set; }
        public string Alias { get; set; }
        public string Password { get; set; }

        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Nouveau mot de passe")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Confirmation")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "La confirmation ne correspond pas.")]
        public string Confirmation { get; set; }
        [Display(Name = "Nom au complet")]
        public string FullName { get; set; }
        public decimal Montant { get; set; }
        public bool Admin { get; set; }
        public string AvatarId { get; set; }
        private ImageGUIDReference ImageReference { get; set; }

        [Display(Name = "Avatar")]
        [Required(ErrorMessage = "Requis")]
        public string AvatarData { get; set; }

        public void InitImageManagement()
        {
            ImageReference = new ImageGUIDReference(@"/ImagesData/Avatars/", @"no_avatar.png");
            ImageReference.MaxSize = 512;
            ImageReference.HasThumbnail = false;
        }

        public UserView()
        {
            InitImageManagement();
            Montant = 150;
        }

        public String GetImageURL()
        {
            return ImageReference.GetURL(AvatarId, false);
        }
        public void SaveImage()
        {
            AvatarId = ImageReference.SaveImage(AvatarData, AvatarId);
        }
        public void RemoveImage()
        {
            ImageReference.Remove(AvatarId);
        }
        public User ToUser()
        {   
            return new User()
            {
                Id = this.Id,
                AvatarId = this.AvatarId,
                Alias = this.Alias,
                FullName = this.FullName,
                Montant = this.Montant,
                Password = this.Password,
                Admin = this.Admin
            };
        }
        public void CopyToUser(User user)
        { 
            user.Id = Id;
            user.AvatarId = AvatarId;
            user.Alias = Alias;
            user.FullName = FullName;
            user.Montant = Montant;
            user.Password = Password;
            user.Admin = Admin;
        }
        public void CopyToUserView(UserView user)
        { 
            user.Id = Id;
            user.AvatarId = AvatarId;
            user.Alias = Alias;
            user.FullName = FullName;
            user.Montant = Montant;
            user.Password = Password;
            user.Admin = Admin;
        }
    }
    public class LoginView
    {
        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Nom d'usager")]
        public string Alias { get; set; }

        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Mot de passe")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}