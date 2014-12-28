using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AzureKienThao.Domain.Models;

namespace AzureKienThao.Controllers
{
    public class KanjiModelsController : Controller
    {
        private DictionaryContext db = new DictionaryContext();

        // GET: KanjiModels
        //Name 	SoundJa 	SoundVn 	MeaningEn 	MeaningVi 
        
        
        public async Task<ActionResult> Index(string name, string soundja, string soundvn, string meaningen, string meaningvi)
        {
            var kanjis = db.Kanjis.Where(x=>true);
            bool isFiltered = false;
            Random rand = new Random();

            if (!string.IsNullOrEmpty(name))
            {
                kanjis = kanjis.Where(x => x.Name == name);
                isFiltered = true;
            }

            if (!string.IsNullOrEmpty(soundja))
            {
                kanjis = kanjis.Where(x => x.SoundJa.Contains(soundja) || x.SoundRo.Contains(soundja));
                isFiltered = true;
            }

            if (!string.IsNullOrEmpty(soundvn))
            {
                kanjis = kanjis.Where(x => x.SoundVn.Contains(soundvn));
                isFiltered = true;
            }

            if (isFiltered)
            {
                return View(await kanjis.Include("Words").ToListAsync());
            }
            else
            {
                var temp = await kanjis.Include("Words").Take(40).ToListAsync();
                var list = temp.OrderBy(x => rand.Next());
                return View(list);
            }
        }

        // GET: KanjiModels/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KanjiModel kanjiModel = await db.Kanjis.FindAsync(id);
            if (kanjiModel == null)
            {
                return HttpNotFound();
            }
            return View(kanjiModel);
        }

        // GET: KanjiModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KanjiModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,SoundJa,SoundVn,MeaningEn,MeaningVi")] KanjiModel kanjiModel)
        {
            if (ModelState.IsValid)
            {
                db.Kanjis.Add(kanjiModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(kanjiModel);
        }

        // GET: KanjiModels/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KanjiModel kanjiModel = await db.Kanjis.FindAsync(id);
            if (kanjiModel == null)
            {
                return HttpNotFound();
            }
            return View(kanjiModel);
        }

        // POST: KanjiModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,SoundJa,SoundVn,MeaningEn,MeaningVi")] KanjiModel kanjiModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kanjiModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(kanjiModel);
        }

        // GET: KanjiModels/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KanjiModel kanjiModel = await db.Kanjis.FindAsync(id);
            if (kanjiModel == null)
            {
                return HttpNotFound();
            }
            return View(kanjiModel);
        }

        // POST: KanjiModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            KanjiModel kanjiModel = await db.Kanjis.FindAsync(id);
            db.Kanjis.Remove(kanjiModel);
            await db.SaveChangesAsync();
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
