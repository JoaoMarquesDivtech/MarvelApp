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
            ViewData["Pagina"] = 0;
            personagemPageDto personagemPageDto = new();
            personagemPageDto = ConsultarPersonagens();

            return View(personagemPageDto);
        }

        [HttpPost]
        public IActionResult Index(personagemPageDto personagemPageDto)
        {
            var paginaAntiga = Convert.ToInt32(Request.Form["paginaAntiga"]);
            if(paginaAntiga == personagemPageDto.Pagina) {
                personagemPageDto.Pagina = 0;
                personagemPageDto = ConsultarPersonagens(personagem: personagemPageDto.FiltroDePesquisa, pagina: personagemPageDto.Pagina);
            }
            else
            {
                personagemPageDto = ConsultarPersonagens(pagina: personagemPageDto.Pagina, ultimaConsulta : personagemPageDto.ultimaConsulta);
            }

            
            return View(personagemPageDto);
        }



        public personagemPageDto ConsultarPersonagens([Optional] List<int> Favoritos, [Optional] int pagina, [Optional] pesquisaPersonagemDto personagem, [Optional] string ultimaConsulta)
        {
            personagemPageDto personagemPageDto = new();
            personagem Personagem = new();
            using (var client = new HttpClient())
            {
                Favoritos = new();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                string ts = DateTime.Now.Ticks.ToString();
                string hash = GerarHash(ts, publicKey, privateKey);
                var consulta = baseUrl + $"?ts={ts}&apikey={publicKey}&hash={hash}";
                consulta += "&limit=90";

                if (pagina != 0)
                {
                    if (ultimaConsulta != null)
                        consulta = ultimaConsulta;

                    personagemPageDto.Pagina = pagina;
                    int contagemIgnorada = pagina * 30;
                    consulta += $"&offset={contagemIgnorada}";
                }

                

                if (Favoritos.Count != 0)
                {
                    
                    foreach (var favorito in Favoritos)
                    {
                        consulta += $"&id={favorito}";
                        HttpResponseMessage responseFavorito = client.GetAsync(consulta).Result;



                        responseFavorito.EnsureSuccessStatusCode();

                        dynamic resultadoFavoritos = JsonConvert.DeserializeObject(responseFavorito.Content.ReadAsStringAsync().Result);

                        if (resultadoFavoritos.data.total != null)
                        {
                            personagemPageDto.ContagemDePaginas = Convert.ToInt32(resultadoFavoritos.data.total.Value);
                            personagemPageDto.ContagemDePaginas = (personagemPageDto.ContagemDePaginas - (personagemPageDto.ContagemDePaginas % 30)) / 30;
                        }

                        Personagem.ID = resultadoFavoritos.data.results[0].id;
                        Personagem.Nome = resultadoFavoritos.data.results[0].name;
                        Personagem.Descricao = resultadoFavoritos.data.results[0].description;
                        Personagem.URLIMAGEM = resultadoFavoritos.data.results[0].thumbnail.path + "." + resultadoFavoritos.data.results[0].thumbnail.extension;
                        Personagem.URLWIKI = resultadoFavoritos.data.results[0].url;
                        personagemPageDto.Personagens.Add(Personagem);

                    }

                    return personagemPageDto;
                }
                else
                {

                    if (personagem != null)
                    {

                        if (personagem.NomeInicial != null)
                            consulta += "&nameStartsWith="+personagem.NomeInicial;
                        if (personagem.Nome != null)
                            consulta += "&name="+personagem.Nome;
                        if (personagem.orderBy != "0")
                            consulta += "&orderBy="+personagem.orderBy;

                    }
                }
            //name=teste&nameStartsWith=a&modifiedSince=12-03&comics=teste&series=teste&events=teste&stories=teste&orderBy=name

                HttpResponseMessage response = client.GetAsync(consulta).Result;



                response.EnsureSuccessStatusCode();
                string conteudo =
                    response.Content.ReadAsStringAsync().Result;

                dynamic resultado = JsonConvert.DeserializeObject(conteudo);

                if (resultado.data.total != null)
                {
                    personagemPageDto.ContagemDePaginas = Convert.ToInt32(resultado.data.total.Value);
                    personagemPageDto.ContagemDePaginas = (personagemPageDto.ContagemDePaginas - (personagemPageDto.ContagemDePaginas % 30)) / 30;
                }

                foreach (var obj in resultado.data.results)
                {
                    Personagem = new();
                    Personagem.ID = obj.id;
                    Personagem.Nome = obj.name;
                    Personagem.Descricao = obj.description;
                    Personagem.URLIMAGEM = obj.thumbnail.path + "." + obj.thumbnail.extension;
                    Personagem.URLWIKI = obj.urls[1].url;
                    personagemPageDto.Personagens.Add(Personagem);
                }

                personagemPageDto.ultimaConsulta = consulta;
                return personagemPageDto;
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
