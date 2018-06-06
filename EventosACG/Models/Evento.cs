using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventosACG.Models
{
    public class Evento
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Lugar { get; set; }
        
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }

        public virtual ICollection<Participante> Participantes { get; set; }


    }
}