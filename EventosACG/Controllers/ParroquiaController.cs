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
    public class ParroquiaController : Controller
    {
        private EventoContext db = new EventoContext();

        // GET: Parroquia
        public ActionResult Index()
        {
            string parroquiaUsuario = string.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var userManager = new UserManager<ApplicationUser>(store);
                ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;
                parroquiaUsuario = user.Parroquia;

                if (string.IsNullOrEmpty(parroquiaUsuario))
                {
                    return View(db.Parroquias.ToList());

                }
                else
                {
                    //Solo puede introducir arroquia el usuario maestro
                    var parroquias = db.Parroquias.Where(c => c.ParroquiaID == -1);
                    return View(parroquias.ToList());

                }
            }
            else
            {
                return null;

            }
                //return View(db.Parroquias.ToList());
        }

        // GET: Parroquia/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parroquia parroquia = db.Parroquias.Find(id);
            if (parroquia == null)
            {
                return HttpNotFound();
            }
            return View(parroquia);
        }

        // GET: Parroquia/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Parroquia/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ParroquiaID,Nombre")] Parroquia parroquia)
        {
            if (ModelState.IsValid)
            {
                db.Parroquias.Add(parroquia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(parroquia);
        }

        // GET: Parroquia/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parroquia parroquia = db.Parroquias.Find(id);
            if (parroquia == null)
            {
                return HttpNotFound();
            }
            return View(parroquia);
        }

        // POST: Parroquia/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ParroquiaID,Nombre")] Parroquia parroquia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(parroquia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(parroquia);
        }

        // GET: Parroquia/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parroquia parroquia = db.Parroquias.Find(id);
            if (parroquia == null)
            {
                return HttpNotFound();
            }
            return View(parroquia);
        }

        // POST: Parroquia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Parroquia parroquia = db.Parroquias.Find(id);
            db.Parroquias.Remove(parroquia);
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
    }
}
