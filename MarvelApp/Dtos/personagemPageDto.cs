using MarvelApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarvelApp.Dto
{
    public class personagemPageDto
    {
        public personagem FiltroDePesquisa { get; set; }
        public List<personagem> PersonagensFavoritos { get; set; }
        public List<personagem> Personagens { get; set; }
        public int Pagina { get; set; }
    }
}
