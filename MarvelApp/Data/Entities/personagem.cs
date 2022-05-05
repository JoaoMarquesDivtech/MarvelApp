using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarvelApp.Data
{
    public class personagem
    {
        public string ID { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string URLIMAGEM { get; set; }
        public string URLWIKI { get; set; }
        public string Total { get; set; }
        public bool favorito { get; set; }


        public override int GetHashCode()
        {
            return Convert.ToInt32(ID);
        }
        public override bool Equals(object obj)
        {    
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                personagem personagem = (personagem)obj;
                return (personagem.ID == ID) ? true : false;
            }
        }



    }


}
