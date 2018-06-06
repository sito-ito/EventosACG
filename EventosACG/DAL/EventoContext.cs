using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using EventosACG.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace EventosACG.DAL
{
    public class EventoContext : DbContext
    {
        public EventoContext() : base("EventoContext")
        {
        }

        public DbSet<Persona> Personas { get; set; }
        public DbSet<Participante> Participantes { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Parroquia> Parroquias { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}