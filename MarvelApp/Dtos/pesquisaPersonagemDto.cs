using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarvelApp.Data
{
    public class pesquisaPersonagemDto
    {
        public string Nome { get; set; }
        public string NomeInicial { get; set; }
        public string dataModificada { get; set; }
        public string Quadrinhos { get; set; }
        public string Series { get; set; }
        public string Eventos { get; set; }
        public string historias { get; set; }
        public string orderBy { get; set; }
        public int Limite { get; set; }
    }
}
