using MarvelApp.Data;
using MarvelApp.Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace MarvelApp.Controllers
{
    public class HomeController : Controller
    {
        public string privateKey { get; set; } = "df9432e811987a8760d717ac84773a297d87972a";
        public string publicKey { get; set; } = "83ee85394fcda89db0b2841c767ee57f";
        public string baseUrl { get; set; } = "http://gateway.marvel.com//v1/public/characters";

        public IActionResult Index()
        {
            personagemPageDto personagemPageDto = new();
            personagemPageDto.Personagens = ConsultarPersonagens();
            return View(personagemPageDto);
        }

        [HttpPost]
        public IActionResult Index(personagemPageDto personagemPageDto)
        {
            var Personagem = ConsultarPersonagens(personagem: personagemPageDto.FiltroDePesquisa);
            return View(Personagem);
        }



        public List<personagem> ConsultarPersonagens([Optional] List<int> Favoritos, [Optional] int pagina, [Optional] personagem personagem)
        {
            List<personagem> Personagens = new();
            personagem Personagem = new();
            using (var client = new HttpClient())
            {
                Favoritos = new();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                string ts = DateTime.Now.Ticks.ToString();
                string hash = GerarHash(ts, publicKey, privateKey);
                var consulta = baseUrl + $"?ts={ts}&apikey={publicKey}&hash={hash}&orderBy=name";


                if(personagem != null) { 
                if (personagem.Descricao != null)
                    consulta += "&description=" + personagem.Descricao;

                if (personagem.Nome != null)
                    consulta += "&name=" + personagem.Nome;
                }

                consulta += "&limit=50";

                if (Favoritos.Count != 0)
                {
                    foreach (var favorito in Favoritos)
                    {
                        consulta += $"&id={favorito}";
                        HttpResponseMessage responseFavorito = client.GetAsync(consulta).Result;



                        responseFavorito.EnsureSuccessStatusCode();

                        dynamic resultadoFavoritos = JsonConvert.DeserializeObject(responseFavorito.Content.ReadAsStringAsync().Result);

                        Personagem.ID = resultadoFavoritos.data.results[0].id;
                        Personagem.Nome = resultadoFavoritos.data.results[0].name;
                        Personagem.Descricao = resultadoFavoritos.data.results[0].description;
                        Personagem.URLIMAGEM = resultadoFavoritos.data.results[0].thumbnail.path + "." + resultadoFavoritos.data.results[0].thumbnail.extension;
                        Personagem.URLWIKI = resultadoFavoritos.data.results[0].url;
                        Personagens.Add(Personagem);

                    }
                    return Personagens;
                }


                HttpResponseMessage response = client.GetAsync(consulta).Result;



                response.EnsureSuccessStatusCode();
                string conteudo =
                    response.Content.ReadAsStringAsync().Result;

                dynamic resultado = JsonConvert.DeserializeObject(conteudo);



                foreach (var obj in resultado.data.results)
                {
                    Personagem = new();
                    Personagem.ID = obj.id;
                    Personagem.Nome = obj.name;
                    Personagem.Descricao = obj.description;
                    Personagem.URLIMAGEM = obj.thumbnail.path + "." + obj.thumbnail.extension;
                    Personagem.URLWIKI = obj.urls[1].url;
                    Personagens.Add(Personagem);
                }


                return Personagens;
            }
        }

        private string GerarHash(string ts, string publicKey, string privateKey)
        {
            byte[] bytes =
                Encoding.UTF8.GetBytes(ts + privateKey + publicKey);
            var gerador = MD5.Create();
            byte[] bytesHash = gerador.ComputeHash(bytes);
            return BitConverter.ToString(bytesHash)
                .ToLower().Replace("-", String.Empty);
        }

        private List<int> ConsultarFavoritos()
        {
            using (var db = new DataBaseContext())
            {
                var context = db.personagensFavoritos.Select(tb => tb.IdFavorito).ToList();
                return context;
            }
        }

        private void AdicionarFavorito(int id)
        {
            using (var db = new DataBaseContext())
            {
                var context = db.personagensFavoritos.Add(new Favoritos { IdFavorito = id });
                db.SaveChanges();
            }
        }

    }
}
