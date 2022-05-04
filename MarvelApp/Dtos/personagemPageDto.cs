using MarvelApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarvelApp.Dto
{
    public class personagemPageDto
    {
        public pesquisaPersonagemDto FiltroDePesquisa { get; set; } 
        public List<personagem> PersonagensFavoritos { get; set; } = new();
        public List<personagem> Personagens { get; set; } = new();
        public int Pagina { get; set; }
        public int ContagemDePaginas { get; set; }
        public string ultimaConsulta { get; set; }
    }
}
