using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Chevalresk_1.Models
{
    public class RatingView
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage = "Requis")]
        public int Value { get; set; }

        [Required(ErrorMessage = "Requis")]
        public string Comment { get; set; }

        public RatingView()
        {
            //Value = -1;
            Value = 1;
            Comment = "";
        }

        public void ToRating(Rating rating)
        {
            rating.Id = Id;
            rating.ItemId = ItemId;
            rating.UserId = UserId;
            rating.Value = Value;
            rating.Comment = Comment;
        }
    }
}