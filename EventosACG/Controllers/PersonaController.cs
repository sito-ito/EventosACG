using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
    public class PersonaController : Controller
    {
        private EventoContext db = new EventoContext();

        // GET: Persona
        public ActionResult Index()
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
                    //Puede ver todas las personas
                    return View(db.Personas.ToList());
                }
                else
                {
                    var personas = db.Personas.Where(c => c.Parroquia.Nombre.ToLower() == parroquiaUsuario.ToLower());
                    return View(personas.ToList());

                }
                //return View(db.Personas.ToList());
            }
            else
            {
                var personas = db.Personas.Where(c => c.ID == -1);
                return View(personas.ToList());
            }
        }

        // GET: Persona/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = db.Personas.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }

            int idParroquia = persona.ParroquiaID;

            Parroquia parroquia = db.Parroquias.Find(idParroquia);

            ViewBag.ParroquiaNombre = parroquia.Nombre;

            return View(persona);
        }

        // GET: Persona/Create
        public ActionResult Create()
        {
            RellenarParroquiaDropDownList();
            //ViewBag.ParroquiasNombres = new SelectList(db.Parroquias, "ParroquiaID", "Nombre");

            List<SelectListItem> sex_items = new List<SelectListItem>();
            sex_items.Add(new SelectListItem() { Text = "MUJER", Value = "M" });
            sex_items.Add(new SelectListItem() { Text = "VARON", Value = "V" });

            ViewBag.Sex = sex_items;

            return View();
        }

        // POST: Persona/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nombre,Apellido,Dni,Fechanacimiento,Telefono,Enfermedad,Alergia,Sexo, ParroquiaID")] Persona persona)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Personas.Add(persona);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }


            RellenarParroquiaDropDownList(persona.ParroquiaID);
            return View(persona);
        }

        // GET: Persona/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = db.Personas.Find(id);
            //ViewBag.ParroquiaId= new SelectList(db.Parroquias, "ParroquiaID", "Nombre", persona.ParroquiaId);
            
            if (persona == null)
            {
                return HttpNotFound();
            }
            RellenarParroquiaDropDownList(persona.ParroquiaID);
            return View(persona);
        }

        // POST: Persona/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nombre,Apellido,Dni,Fechanacimiento,Telefono,Enfermedad,Alergia,Sexo,ParroquiaID")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                db.Entry(persona).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(persona);
        }

        // GET: Persona/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = db.Personas.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // POST: Persona/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Persona persona = db.Personas.Find(id);
            db.Personas.Remove(persona);
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

        private void RellenarParroquiaDropDownList(object parroquiaSeleccionada = null)
        {
            //ViewBag.ParroquiasNombres = new SelectList(db.Parroquias, "ParroquiaID", "Nombre");
            var parroquiasQuery = from d in db.Parroquias
                                   orderby d.Nombre
                                   select d;
            ViewBag.ParroquiaId = new SelectList(parroquiasQuery, "ParroquiaID", "Nombre", parroquiaSeleccionada);
        }
    }
}
