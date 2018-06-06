using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventosACG.Models
{
    public enum Puesto
    {
        RESPONSABLE, MÉDICO, CATEQUISTA, COCINA, CURA, NIÑO, ANIMACIÓN, MANTENIENTO, OTRO 
    }


    public class Participante
    {
        public int ParticipanteID { get; set; }
        public int EventoID { get; set; }
        public int PersonaID { get; set; }
        public Puesto? Puesto { get; set; }
        public string Observacion { get; set; }
        public bool Autobus { get; set; }
        public bool Pagado { get; set; }
        public bool Documentacion { get; set; }

        public virtual Evento Evento { get; set; }
        public virtual Persona Persona { get; set; }
    }
}