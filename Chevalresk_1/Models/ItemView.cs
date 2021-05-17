using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Chevalresk_1.Models
{
    public enum typeItem { Arme, Armure, Potion, Ressource }
    public enum genreArme { Épée, Espadon, ÉpéeBouclier, BatonMagique, Arc }
    public class ItemView
    {
        /*Item*/
        public int Id { get; set; }
        [Display(Name = "Nom de l'item")]
        [Required(ErrorMessage = "Obligatoire")]
        public string Name { get; set; }
        [Display(Name = "Prix de l'item")]
        [Range(1, 10000000, ErrorMessage = "Invalide, ex: 1 à 10_000_000")]
        [Required(ErrorMessage = "Obligatoire")]
        public int Prix { get; set; }
        [Display(Name = "Quantité en stock")]
        [Range(1, 10000, ErrorMessage = "Invalide, ex: 1 à 10000")]
        [Required(ErrorMessage = "Obligatoire")]
        public int QuantiteStock { get; set; }
        [Display(Name = "Type de l'item")]
        public typeItem Type { get; set; }
        public string ImageId { get; set; }
        private ImageGUIDReference ImageReference { get; set; }

        [Display(Name = "Image")]
        [Required(ErrorMessage = "Requis")]
        public string ImageData { get; set; }
        /*Arme*/
        [Display(Name = "Genre de l'arme")]
        public genreArme genreArme { get; set; }

        [Display(Name = "Efficacité de l'arme")]
        //[Required(ErrorMessage = "Obligatoire")]
        public string efficaciteArme { get; set; }
        /*Armure*/
        [Display(Name = "Poids de l'armure")]
        [Range(1, 500, ErrorMessage = "Invalide, ex: 1,500")]
        //[Required(ErrorMessage = "Obligatoire")]
        public int poids { get; set; }

        [Display(Name = "Matière de l'armure")]
        //[Required(ErrorMessage = "Obligatoire")]
        public string matiere { get; set; }

        [Display(Name = "taille de l'armure")]
        [Range(100, 200, ErrorMessage = "Invalide, ex: 100 cm,200 cm")]
        //[Required(ErrorMessage = "Obligatoire")]
        public string taille { get; set; }
        /*Potion*/
        [Display(Name = "Effet de la potion")]
        //[Required(ErrorMessage = "Obligatoire")]
        public string effet { get; set; }

        [Display(Name = "Durée de l'effet de la potion")]
        [Range(1, 300, ErrorMessage = "Invalide, ex: 1 sec,300 secs")]
        //[Required(ErrorMessage = "Obligatoire")]
        public int duree { get; set; }
        /*Ressource*/
        [Display(Name = "Description")]
        //[Required(ErrorMessage = "Obligatoire")]
        public string description { get; set; }
        public double RatingsAverage { get; set; }
        public int NbRatings { get; set; }
        public int flag { get; set; }
        public void InitImageManagement()
        {
            ImageReference = new ImageGUIDReference(@"/ImagesData/Items/", @"no_Item.png");
            ImageReference.MaxSize = 512;
            ImageReference.HasThumbnail = false;
        }
        public ItemView()
        {
            duree = 1;
            poids = 1;
            taille = "100";
            Prix = 1;
            QuantiteStock = 1;
            InitImageManagement();
            flag = 1;
            RatingsAverage = 0;
            NbRatings = 0;
        }

        public String GetImageURL()
        {
            return ImageReference.GetURL(ImageId, false);
        }
        public void SaveImage()
        {
            ImageId = ImageReference.SaveImage(ImageData, ImageId);
        }
        public void RemoveImage()
        {
            ImageReference.Remove(ImageId);
        }


    }
}