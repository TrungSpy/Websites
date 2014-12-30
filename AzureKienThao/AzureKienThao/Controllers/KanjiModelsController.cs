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
        public async Task<ActionResult> Index(string query, bool inname = true, bool insoundja = true,
            bool insoundvn = true, bool inmeaningen = true, bool inmeaningvi = false)
        {
 
            Random rand = new Random();
            query = query == null ? query : query.ToLower().Trim();
            var kanjis = db.Kanjis.Where(x=> 
                (string.IsNullOrEmpty(query)) ||
                (inname&&query.Contains(x.Name)) ||
                (insoundja && x.SoundJa.Contains(query) || x.SoundRo.Contains(query)) ||
                (insoundvn && x.SoundVn.Contains(query)) ||
                (inmeaningen && x.MeaningEn.Contains(query)) ||
                (inmeaningvi && x.MeaningVi.Contains(query))
                );
            ViewBag.query = query;
            ViewBag.inname = inname;
            ViewBag.insoundja = insoundja;
            ViewBag.insoundvn = insoundvn;
            ViewBag.inmeaningen = inmeaningen;
            ViewBag.inmeaningvi = inmeaningvi;

            IEnumerable<KanjiModel> rets = null;
            ViewBag.SoundVnSummary = "";
            if (string.IsNullOrEmpty(query))
            {
                rets = await kanjis.Include("Words").OrderBy(x=>Guid.NewGuid()).Take(5).ToListAsync();              
            }
            else
            {
                rets = await kanjis.ToListAsync();
                if (inname)
                {
                    ViewBag.SoundVnSummary = string.Join(" ", rets.Where(x=>query.Contains(x.Name)).Select(x => x.SoundVn));
                }
            }
            if (Request.Browser.IsMobileDevice)
            {
                return View("IndexMobile", rets);
            }
            else
            {
                return View(rets);
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
        [Authorize(Roles="Admin")]
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
        [Authorize(Roles = "Admin")]
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

       [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
