using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Skin
    {
        public int Id { get; set; }


        public string Name { get; set; }

        public decimal BasePriceUsd {  get; set; }

        public bool IsAvailable { get; set; }
        private Skin() { }

        public Skin(int Id,string Name,decimal BasePriceUsd,bool IsAvailable)
        {
            this.Id = Id;
            this.Name = Name;
            this.BasePriceUsd = BasePriceUsd;
            this.IsAvailable = IsAvailable;
        }
    }
}
