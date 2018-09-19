using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventosACG.Codigo;
using EventosACG.DAL;
using EventosACG.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EventosACG.Controllers
{
    [Authorize]
    public class ParticipanteController : Controller
    {
        private EventoContext db = new EventoContext();

        // GET: Participante
        public ActionResult Index(int? eventoID, string option, string busqueda)
        {
            if (eventoID.HasValue)
            {
                var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var userManager = new UserManager<ApplicationUser>(store);
                ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;
                string parroquiaUsuario = user.Parroquia;

                var participantes = db.Participantes.Include(p => p.Evento).Include(p => p.Persona).Where(e => e.EventoID == -1).OrderBy(p => p.Persona.ParroquiaID);
                if (string.IsNullOrEmpty(option))
                {
                    if (string.IsNullOrEmpty(parroquiaUsuario))
                    {
                        participantes = db.Participantes.Include(p => p.Evento).Include(p => p.Persona).Where(e => e.EventoID == eventoID).OrderBy(p => p.Persona.ParroquiaID);
                    }
                    else
                    {
                        participantes = db.Participantes.Include(p => p.Evento).Include(p => p.Persona).Where(e => e.EventoID == eventoID && e.Persona.Parroquia.Nombre == parroquiaUsuario).OrderBy(p => p.Persona.ParroquiaID);
                    }
                    //ViewBag.eventoID = eventoID;
                    //return View(participantes.ToList());

                }
                else
                {
                    if (string.IsNullOrEmpty(busqueda))
                    {
                        if (string.IsNullOrEmpty(parroquiaUsuario))
                        {
                            participantes = db.Participantes.Include(p => p.Evento).Include(p => p.Persona).Where(e => e.EventoID == eventoID).OrderBy(p => p.Persona.ParroquiaID);
                        }
                        else
                        {
                            participantes = db.Participantes.Include(p => p.Evento).Include(p => p.Persona).Where(e => e.EventoID == eventoID && e.Persona.Parroquia.Nombre == parroquiaUsuario).OrderBy(p => p.Persona.ParroquiaID);
                        }

                        //ViewBag.eventoID = eventoID;
                        //return View(participantes.ToList());

                    }
                    else //existe eventoID, option y busqueda -> hago el filtro
                    {
                        if (string.IsNullOrEmpty(parroquiaUsuario))
                        {
                            if (option == "Puesto")
                            {
                                participantes = db.Participantes.Include(p => p.Evento).Include(p => p.Persona)
                                            .Where(e => e.EventoID == eventoID  && e.Puesto.ToString().StartsWith(busqueda)).OrderBy(p => p.Persona.ParroquiaID);
                            }
                            else if (option == "Nombre")
                            {
                                participantes = db.Participantes.Include(p => p.Evento).Include(p => p.Persona)
                                            .Where(e => e.EventoID == eventoID && e.Persona.Nombre.StartsWith(busqueda)).OrderBy(p => p.Persona.ParroquiaID);
                            }
                            else if (option == "Apellidos")
                            {
                                participantes = db.Participantes.Include(p => p.Evento).Include(p => p.Persona)
                                            .Where(e => e.EventoID == eventoID && e.Persona.Apellido.StartsWith(busqueda)).OrderBy(p => p.Persona.ParroquiaID);
                            }
                        }
                        else
                        {
                            if (option == "Puesto")
                            {
                                participantes = db.Participantes.Include(p => p.Evento).Include(p => p.Persona)
                                            .Where(e => e.EventoID == eventoID && e.Persona.Parroquia.Nombre == parroquiaUsuario && e.Puesto.ToString().StartsWith(busqueda)).OrderBy(p => p.Persona.ParroquiaID);
                            }
                            else if (option == "Nombre")
                            {
                                participantes = db.Participantes.Include(p => p.Evento).Include(p => p.Persona)
                                            .Where(e => e.EventoID == eventoID && e.Persona.Parroquia.Nombre == parroquiaUsuario && e.Persona.Nombre.StartsWith(busqueda)).OrderBy(p => p.Persona.ParroquiaID);
                            }
                            else if (option == "Apellidos")
                            {
                                participantes = db.Participantes.Include(p => p.Evento).Include(p => p.Persona)
                                            .Where(e => e.EventoID == eventoID && e.Persona.Parroquia.Nombre == parroquiaUsuario && e.Persona.Apellido.StartsWith(busqueda)).OrderBy(p => p.Persona.ParroquiaID);
                            }

                        }

                        //participantes = db.Participantes.Include(p => p.Evento).Include(p => p.Persona).Where(e => e.EventoID == eventoID);
                        //ViewBag.eventoID = eventoID;
                        //return View(participantes.ToList());
                    }

                }

                //participantes = db.Participantes.Include(p => p.Evento).Include(p => p.Persona).Where(e => e.EventoID == eventoID);
                ViewBag.eventoID = eventoID;
                ViewBag.ParticipantesCount = participantes.Count();
                return View(participantes.ToList());
            }
            else
            {
                return null;
            }
            
        }

       

        // GET: Participante/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participante participante = db.Participantes.Find(id);
            if (participante == null)
            {
                return HttpNotFound();
            }
            return View(participante);
        }

        // GET: Participante/Create
        public ActionResult Create()
        {
            ViewBag.EventoID = new SelectList(db.Eventos, "ID", "Nombre");
            ViewBag.PersonaID = new SelectList(db.Personas, "ID", "Nombre");
            return View();
        }

        // POST: Participante/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ParticipanteID,EventoID,PersonaID,Puesto,Observacion,Autobus,Pagado,Documentacion")] Participante participante)
        {
            if (ModelState.IsValid)
            {
                db.Participantes.Add(participante);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EventoID = new SelectList(db.Eventos, "ID", "Nombre", participante.EventoID);
            ViewBag.PersonaID = new SelectList(db.Personas, "ID", "Nombre", participante.PersonaID);
            return View(participante);
        }

        // GET: Participante/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participante participante = db.Participantes.Find(id);
            if (participante == null)
            {
                return HttpNotFound();
            }
            //ViewBag.EventoID = new SelectList(db.Eventos, "ID", "Nombre", participante.EventoID);
            ViewBag.EventoID = participante.EventoID;
            //ViewBag.PersonaID = new SelectList(db.Personas, "ID", "Nombre", participante.PersonaID);
            ViewBag.PersonaID = participante.PersonaID;
            return View(participante);
        }

        // POST: Participante/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ParticipanteID,EventoID,PersonaID,Puesto,Observacion,Autobus,Pagado,Documentacion")] Participante participante)
        {
            if (ModelState.IsValid)
            {
                db.Entry(participante).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { eventoID = participante.EventoID } );
            }
            ViewBag.EventoID = new SelectList(db.Eventos, "ID", "Nombre", participante.EventoID);
            ViewBag.PersonaID = new SelectList(db.Personas, "ID", "Nombre", participante.PersonaID);
            return View(participante);
        }

        // GET: Participante/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participante participante = db.Participantes.Find(id);
            if (participante == null)
            {
                return HttpNotFound();
            }
            return View(participante);
        }

        // POST: Participante/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Participante participante = db.Participantes.Find(id);
            db.Participantes.Remove(participante);
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

        [HttpGet]
        public FileContentResult ExportToExcel(string idEvento)
        {
            int EventoID = Convert.ToInt32(idEvento);

            //var participantes = db.Participantes.Include(p => p.Evento).Include(p => p.Persona).Where(e => e.EventoID == EventoID);
            var participantes = (from p in db.Participantes
                            where p.EventoID == EventoID
                            select new { Evento = p.Evento.Nombre, NombrePersona = p.Persona.Nombre , p.Persona.Apellido, Parroquia = p.Persona.Parroquia.Nombre, p.Puesto, p.Observacion, p.Persona.Enfermedad, p.Persona.Alergia }).ToList();
            //List < Participante > participantesList = participantes.ToList();

            string[] columns = { "Evento", "NombrePersona", "Apellido", "Parroquia", "Puesto", "NombrePuesto", "Observaciones", "Enfermedad", "Alergia" };
            byte[] filecontent = ExcelExportHelper.ExportExcel(participantes, "Participantes", true, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, "Participantes.xlsx");
        }
    }
}
