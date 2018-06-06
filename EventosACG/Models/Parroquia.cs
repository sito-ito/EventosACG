using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventosACG.Models
{
    public class Parroquia
    {
        public int ParroquiaID { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<Persona> Personas { get; set; }

    }
}