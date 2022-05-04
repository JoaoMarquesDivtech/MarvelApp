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
        public string strConnection { get; set; } = "Server=(localdb)\\MSSQLLocalDB;Database=WebAppDb;Trusted_Connection=True;MultipleActiveResultSets=true";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(strConnection);
        }

        public DbSet<Favoritos> personagensFavoritos { get; set; }




    }
}