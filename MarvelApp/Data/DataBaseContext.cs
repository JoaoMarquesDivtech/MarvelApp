using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarvelApp.Data;

namespace MarvelApp.Data
{
    public class DataBaseContext : DbContext
    {
        public string strConnection { get; set; } = "";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(strConnection);
        }

        public DbSet<Favoritos> personagensFavoritos { get; set; }




    }
}