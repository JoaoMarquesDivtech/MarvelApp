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
using MarvelApp.Services;

namespace MarvelApp.Controllers
{
    public class HomeController : Controller
    {


        public IActionResult Index()
        {
            ViewData["Pagina"] = 0;
            personagemPageDto personagemPageDto = new();
            personagemPageDto = PersonagemService.ConsultarPersonagens();
            var IdFavoritos = PersonagemService.ConsultarIdPersonagensFavoritos().ToList();
            personagemPageDto.PersonagensFavoritos = PersonagemService.ConsultarPersonagens(Favoritos: IdFavoritos).PersonagensFavoritos;
            personagemPageDto.Personagens = personagemPageDto.Personagens.Except(personagemPageDto.PersonagensFavoritos).ToList();
            return View(personagemPageDto);
        }

        [HttpPost]
        public IActionResult Index(personagemPageDto personagemPageDto)
        {
            var paginaAntiga = Convert.ToInt32(Request.Form["paginaAntiga"]);
            if (paginaAntiga == personagemPageDto.Pagina)
            {
                personagemPageDto.Pagina = 0;
                personagemPageDto = PersonagemService.ConsultarPersonagens(personagem: personagemPageDto.FiltroDePesquisa, pagina: personagemPageDto.Pagina);
                personagemPageDto.PersonagensFavoritos = PersonagemService.ConsultarPersonagens(PersonagemService.ConsultarIdPersonagensFavoritos()).PersonagensFavoritos;
                personagemPageDto.Personagens = personagemPageDto.Personagens.Except(personagemPageDto.PersonagensFavoritos).ToList();
            }
            else
            {
                personagemPageDto = PersonagemService.ConsultarPersonagens(pagina: personagemPageDto.Pagina, ultimaConsulta: personagemPageDto.ultimaConsulta);
                personagemPageDto.Personagens.Except(personagemPageDto.PersonagensFavoritos);
                personagemPageDto.PersonagensFavoritos = PersonagemService.ConsultarPersonagens(PersonagemService.ConsultarIdPersonagensFavoritos()).PersonagensFavoritos;
                personagemPageDto.Personagens = personagemPageDto.Personagens.Except(personagemPageDto.PersonagensFavoritos).ToList();
            }


            return View(personagemPageDto);
        }




        public IActionResult Favorito(int id, bool retirar = false)
        {
            try {
                PersonagemService.Favoritar(id, retirar);
            }
            catch(Exception ex)
            {
                //Tratamento vazio provisorio
            }

            return RedirectToAction("Index");
        }


    }
}
