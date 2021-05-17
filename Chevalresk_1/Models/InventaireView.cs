using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chevalresk_1.Models
{
    public class InventaireView
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int UserId { get; set; }
        public int QuantiteInventaire { get; set; }
    }
}