using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventosACG.DAL;
using EventosACG.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EventosACG.Controllers
{
    [Authorize]
    public class EventoController : Controller
    {
        private EventoContext db = new EventoContext();

        // GET: Evento
        public ActionResult Index(int? eventoSeleccionado)
        {
            //return View(db.Eventos.ToList());
 
            var participantes = db.Participantes.OrderBy(q => q.Persona.Apellido).ToList();
            ViewBag.SelectedEvento = new SelectList(participantes, "ParticipanteID", "Name", eventoSeleccionado);
            int eventoID = eventoSeleccionado.GetValueOrDefault();

            IQueryable<Evento> eventos = db.Eventos
                .Where(c => !eventoSeleccionado.HasValue || c.ID == eventoID)
                .OrderBy(d => d.ID)
                .Include(d => d.Participantes);

            var sql = eventos.ToString();
            return View(eventos.ToList());
            
        }

        [HttpPost]
        public JsonResult Index(string Prefix)
        {
            string parroquiaUsuario = "";
            if (User.Identity.IsAuthenticated)
            {
                var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var userManager = new UserManager<ApplicationUser>(store);
                ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;
                parroquiaUsuario = user.Parroquia;

                
                if (string.IsNullOrEmpty(parroquiaUsuario))
                {
                    //Si parroquiaUsuario = nulo o vacio entonces es un administrador
                    var allPersonas = from Persona in db.Personas
                                      //join parroq in db.Parroquias on Persona.ParroquiaId equals parroq.ParroquiaID
                                      //where parroq.Nombre == parroquiaUsuario
                                      select new
                                      {
                                          NombreCompleto = Persona.Apellido + ", " + Persona.Nombre,
                                          Persona.ID,
                                          Persona.Apellido,
                                          Persona.Nombre,
                                          Persona.ParroquiaID
                                      };



                    //Searching records from list using LINQ query  
                    var PersonaList = (from N in allPersonas
                                       where N.NombreCompleto.StartsWith(Prefix)
                                       select new {N.ID, N.NombreCompleto });
                    return Json(PersonaList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //Si parroquiaUsuario tiene valor entoces filtro las personas por esa parroquia
                    var allPersonas = from Persona in db.Personas
                                    //.Where(c => !(string.IsNullOrEmpty(parroquiaUsuario) || c.Parroquia.Nombre == parroquiaUsuario))
                                    join parroq in db.Parroquias on Persona.ParroquiaID equals parroq.ParroquiaID
                                    where parroq.Nombre == parroquiaUsuario
                                    select new
                                    {
                                        NombreCompleto = Persona.Apellido + ", " + Persona.Nombre,
                                        Persona.ID,
                                        Persona.Apellido,
                                        Persona.Nombre,
                                        Persona.ParroquiaID
                                    };



                    //Searching records from list using LINQ query  
                    var PersonaList = (from N in allPersonas
                                       where N.NombreCompleto.StartsWith(Prefix)
                                       select new { N.ID, N.NombreCompleto });
                    return Json(PersonaList, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                //NO está logueado -> devolvemos vacio
                return null;

            }
        }

        [HttpPost]
        public JsonResult ObtenerDatosPersona(string idPersona, string nombreCompletoPersona)
        {
            int personaId = -1;
            var persona = db.Personas.Find(-1);
            try
            {
                personaId = Convert.ToInt32(idPersona);

                persona = db.Personas.Find(personaId);
            }
            catch
            {
                //No se ha podido realizar la conversión
                //Intento buscar por el nombre completo

                string apellidos = nombreCompletoPersona.Split(',')[0].Trim();
                string nombre = nombreCompletoPersona.Split(',')[1].Trim();

                IQueryable<Persona> personas = db.Personas.Where(c => c.Apellido.ToUpper() == apellidos.ToUpper() &&
                                                c.Nombre.ToUpper() == nombre.ToUpper());
                if (personas.Count() > 0)
                {
                    persona = personas.First();
                        
                }
                else
                {
                    return null;
                }
            }

            //persona = db.Personas.Find(personaId);
            
            var obj = new ObjetoPersonaDevuelta();
            obj.Nombre = persona.Nombre;
            obj.Apellido = persona.Apellido;
            obj.Dni = persona.Dni;
            obj.Fechanacimiento = persona.Fechanacimiento;
            obj.Telefono = persona.Telefono;
            obj.Enfermedad = persona.Enfermedad;
            obj.Alergia = persona.Alergia;
            obj.ParroquiaID = persona.ParroquiaID;
            obj.Parroquia = persona.Parroquia.Nombre;
            obj.SexoPersona = persona.Sexo;
            obj.PersonaId = persona.ID;

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult InsertarPersonaEventoById(string idPersona, string idEvento, string puesto, string observacion
                                            , string bAutobus, string bPagado, string bDocumentacion)
        {
            int personaId = -1;
            int eventoId = -1;
            bool autobus = false;
            bool pagado = false;
            bool documentacion = false;
            
            try
            {
                personaId = Convert.ToInt32(idPersona);
                eventoId = Convert.ToInt32(idEvento);

                if (bAutobus == "true")
                {
                    autobus = true;
                }

                if (bPagado == "true")
                {
                    pagado = true;
                }

                if(bDocumentacion == "true")
                {
                    documentacion = true;
                }
            }
            catch
            {
                //No se ha podido realizar la conversión
            }

            if (personaId != -1 && eventoId != -1)
            {
                //Busco si existe ya esa relación

                IQueryable<Participante> participante = db.Participantes.Where(c => c.PersonaID == personaId && c.EventoID == eventoId);

                if (participante.Count() > 0 )
                {
                    //Ya existe esa persona en el evento
                    return Json(new { success = false, responseText = "Ya existe esta persona en el Evento" }, JsonRequestBehavior.AllowGet);
                    
                }

                //Si no existe entonces hago la inserción
                Participante participanteNuevo = new Participante();

                participanteNuevo.PersonaID = personaId;
                participanteNuevo.EventoID = eventoId;
                Puesto puestoAux = (Puesto)Enum.Parse(typeof(Puesto), puesto);
                participanteNuevo.Puesto = puestoAux;
                participanteNuevo.Observacion = observacion;


                participanteNuevo.Autobus = autobus;
                participanteNuevo.Pagado = pagado;
                participanteNuevo.Documentacion = documentacion;

                db.Participantes.Add(participanteNuevo);
                db.SaveChanges();

            }
            //var resultado = true;
            return Json(new { success = true, responseText = "Persona Insertada" }, JsonRequestBehavior.AllowGet);
            //return Json(resultado);

        }

        [HttpPost]
        public JsonResult AñadirPersonaEventoByDatos(string nombre, string apellido, string fechanacimiento, string dni, string sexo, string telefono, 
                                        string enfermedad, string alergia, string parroquiaID, string idEvento, string puesto, string observacion,
                                        string bAutobus, string bPagado, string bDocumentacion)
        {
            bool insercionCorrecta = true;
            string mensajeInsercion = "Inserción correcta";


            //Si no existe entonces hago la inserción
            Persona personaNueva = new Persona();
            personaNueva.Nombre = nombre;
            personaNueva.Apellido = apellido;
            personaNueva.Fechanacimiento = Convert.ToDateTime(fechanacimiento);
            personaNueva.Dni = dni;
            personaNueva.Sexo = sexo;
            personaNueva.Telefono = telefono;
            personaNueva.Enfermedad = enfermedad;
            personaNueva.Alergia = alergia;
            personaNueva.ParroquiaID = Convert.ToInt32(parroquiaID);

            db.Personas.Add(personaNueva);
            db.SaveChanges();

            int personaId = personaNueva.ID;


            int eventoId = -1;
            bool autobus = false;
            bool pagado = false;
            bool documentacion = false;

            try
            {
                //personaId = Convert.ToInt32(idPersona);
                eventoId = Convert.ToInt32(idEvento);

                if (bAutobus == "true")
                {
                    autobus = true;
                }

                if (bPagado == "true")
                {
                    pagado = true;
                }

                if (bDocumentacion == "true")
                {
                    documentacion = true;
                }
            }
            catch
            {
                //No se ha podido realizar la conversión 
                insercionCorrecta = false;
                mensajeInsercion = "Error en la conversión";
            }

            if (personaId != -1 && eventoId != -1)
            {
                //Busco si existe ya esa relación

                IQueryable<Participante> participante = db.Participantes.Where(c => c.PersonaID == personaId && c.EventoID == eventoId);

                if (participante.Count() > 0)
                {
                    //Ya existe esa persona en el evento
                    //return Json(new { success = false, responseText = "Ya existe esta persona en el Evento" }, JsonRequestBehavior.AllowGet);
                    insercionCorrecta = false;
                    mensajeInsercion = "Ya existe esta persona en el Evento";
                }
                else
                {

                    //Si no existe entonces hago la inserción
                    Participante participanteNuevo = new Participante();

                    participanteNuevo.PersonaID = personaId;
                    participanteNuevo.EventoID = eventoId;
                    Puesto puestoAux = (Puesto)Enum.Parse(typeof(Puesto), puesto);
                    participanteNuevo.Puesto = puestoAux;
                    participanteNuevo.Observacion = observacion;


                    participanteNuevo.Autobus = autobus;
                    participanteNuevo.Pagado = pagado;
                    participanteNuevo.Documentacion = documentacion;

                    db.Participantes.Add(participanteNuevo);
                    db.SaveChanges();
                }
            }

            return Json(new { success = insercionCorrecta, responseText = mensajeInsercion}, JsonRequestBehavior.AllowGet);
            //return Json(new { success = true, responseText = "Persona Insertada" }, JsonRequestBehavior.AllowGet);

        }



        // GET: Evento/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evento evento = db.Eventos.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdEvento = id;
              
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;
            if (user.Parroquia != null)
            {
                ViewBag.parroquiaUsuario = user.Parroquia.ToLower();
            }
            else
            {
                ViewBag.parroquiaUsuario = string.Empty;
            }

            RellenarParroquiaDropDownList();

            List<SelectListItem> sex_items = new List<SelectListItem>();
            sex_items.Add(new SelectListItem() { Text = "MUJER", Value = "M" });
            sex_items.Add(new SelectListItem() { Text = "VARON", Value = "V" });

            ViewBag.Sex = sex_items;

            return View(evento);
        }

        // GET: Evento/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Evento/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nombre,Lugar,Fecha")] Evento evento)
        {
            if (ModelState.IsValid)
            {
                db.Eventos.Add(evento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(evento);
        }

        // GET: Evento/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evento evento = db.Eventos.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return View(evento);
        }

        // POST: Evento/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nombre,Lugar,Fecha")] Evento evento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(evento);
        }

        // GET: Evento/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evento evento = db.Eventos.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return View(evento);
        }

        // POST: Evento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Evento evento = db.Eventos.Find(id);
            db.Eventos.Remove(evento);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public JsonResult PuestosParticipantes() 
        {
            List<SelectListItem> puestos = new List<SelectListItem>();
            
            puestos.Add(new SelectListItem { Value = "RESPONSABLE", Text = "RESPONSABLE" });
            puestos.Add(new SelectListItem { Value = "MÉDICO", Text = "MÉDICO" });
            puestos.Add(new SelectListItem { Value = "CATEQUISTA", Text = "CATEQUISTA" });
            puestos.Add(new SelectListItem { Value = "COCINA", Text = "COCINA" });
            puestos.Add(new SelectListItem { Value = "CURA", Text = "CURA" });
            puestos.Add(new SelectListItem { Value = "NIÑO", Text = "NIÑO" });
            puestos.Add(new SelectListItem { Value = "ANIMACIÓN", Text = "ANIMACIÓN" });
            puestos.Add(new SelectListItem { Value = "MANTENIENTO", Text = "MANTENIENTO" });
            puestos.Add(new SelectListItem { Value = "OTRO", Text = "OTRO" });
           
            return Json(puestos, JsonRequestBehavior.AllowGet);
        }

        private void RellenarParroquiaDropDownList(object parroquiaSeleccionada = null)
        {
            //ViewBag.ParroquiasNombres = new SelectList(db.Parroquias, "ParroquiaID", "Nombre");
            var parroquiasQuery = from d in db.Parroquias
                                  orderby d.Nombre
                                  select d;
            ViewBag.ParroquiaId = new SelectList(parroquiasQuery, "ParroquiaID", "Nombre", parroquiaSeleccionada);
        }

        public class ObjetoPersonaDevuelta
        {
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Dni { get; set; }
            public DateTime Fechanacimiento{ get; set; }
            public string Telefono { get; set; }
            public string Enfermedad { get; set; }
            public string Alergia { get; set; }
            public string SexoPersona { get; set; }
            public int ParroquiaID { get; set; }
            public string Parroquia { get; set; }
            public int PersonaId { get; set; }



        }
    }
}
