using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventosACG.Models
{
    public class Persona
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }

        [Display(Name = "Fecha Nacimiento")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fechanacimiento { get; set; }
        
        
        public string Telefono { get; set; }
        public string Enfermedad { get; set; }
        public string Alergia { get; set; }
        public string Sexo { get; set; }

        public int ParroquiaID { get; set; }

        public virtual Parroquia Parroquia { get; set; }
        public virtual ICollection<Participante> Participantes { get; set; }

    }

}