using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Farm_management.Data;
using Farm_management.Models;

namespace Farm_management.Controllers
{
    public class HatcheriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HatcheriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. دالة عرض الصفحة الرئيسية (Index) - كما هي عندك
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Hatcheries
                .Include(h => h.FemaleAnimal)
                .Include(h => h.MaleAnimal)
                .Include(h => h.ProductionBarn);
            return View(await applicationDbContext.ToListAsync());
        }

        // 2. الدالة الجديدة التي يحتاجها السكريبت (GetAnimalsByBarn)
        // يجب أن تكون موجودة لكي يعمل السكريبت في صفحة Create
        [HttpGet]
        public async Task<JsonResult> GetAnimalsByBarn(int barnId)
        {
            var barn = await _context.Barns
                .Include(b => b.Animals)
                .FirstOrDefaultAsync(b => b.Id == barnId);

            if (barn == null) return Json(null);

            var result = new
            {
                specialization = barn.kindOfLife, // التخصص (نوع الحياة)
                animals = barn.Animals.Select(a => new {
                    a.Id,
                    a.Name,
                    a.Species,
                    a.Age,
                    a.Gender
                }).ToList()
            };

            return Json(result);
        }

        // GET: Hatcheries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hatchery = await _context.Hatcheries
                .Include(h => h.FemaleAnimal)
                .Include(h => h.MaleAnimal)
                .Include(h => h.ProductionBarn)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hatchery == null)
            {
                return NotFound();
            }

            return View(hatchery);
        }

        // GET: Hatcheries/Create
        // GET: Hatcheries/Create
        public IActionResult Create()
        {
            // جلب الحظائر التي سيتم اختيار الآباء منها
            ViewData["ParentBarns"] = new SelectList(_context.Barns, "Id", "Name");

            // جلب الحظائر التي ستكون مخصصة للإنتاج (نفس القائمة أو مختلفة)
            ViewData["ProductionBarnId"] = new SelectList(_context.Barns, "Id", "Name");

            return View();
        }

        // POST: Hatcheries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hatchery hatchery)
        {
            if (ModelState.IsValid)
            {
                // 1. حفظ سجل التفريخ أولاً
                _context.Add(hatchery);

                // 2. جلب الذكر والأنثى من قاعدة البيانات لتغيير مكانهم
                var male = await _context.Animals.FindAsync(hatchery.MaleAnimalId);
                var female = await _context.Animals.FindAsync(hatchery.FemaleAnimalId);

                if (male != null && female != null)
                {
                    // تغيير الحظيرة للحيوانات إلى حظيرة الإنتاج
                    male.BarnId = hatchery.ProductionBarnId;
                    female.BarnId = hatchery.ProductionBarnId;

                    _context.Update(male);
                    _context.Update(female);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hatchery);
        }

        // GET: Hatcheries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hatchery = await _context.Hatcheries.FindAsync(id);
            if (hatchery == null)
            {
                return NotFound();
            }
            ViewData["FemaleAnimalId"] = new SelectList(_context.Animals, "Id", "Name", hatchery.FemaleAnimalId);
            ViewData["MaleAnimalId"] = new SelectList(_context.Animals, "Id", "Name", hatchery.MaleAnimalId);
            ViewData["ProductionBarnId"] = new SelectList(_context.Barns, "Id", "Name", hatchery.ProductionBarnId);
            return View(hatchery);
        }

        // POST: Hatcheries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaleAnimalId,FemaleAnimalId,ProductionBarnId,StartDate,ExpectedEndDate,Status")] Hatchery hatchery)
        {
            if (id != hatchery.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hatchery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HatcheryExists(hatchery.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FemaleAnimalId"] = new SelectList(_context.Animals, "Id", "Name", hatchery.FemaleAnimalId);
            ViewData["MaleAnimalId"] = new SelectList(_context.Animals, "Id", "Name", hatchery.MaleAnimalId);
            ViewData["ProductionBarnId"] = new SelectList(_context.Barns, "Id", "Name", hatchery.ProductionBarnId);
            return View(hatchery);
        }

        // GET: Hatcheries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hatchery = await _context.Hatcheries
                .Include(h => h.FemaleAnimal)
                .Include(h => h.MaleAnimal)
                .Include(h => h.ProductionBarn)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hatchery == null)
            {
                return NotFound();
            }

            return View(hatchery);
        }

        // POST: Hatcheries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hatchery = await _context.Hatcheries.FindAsync(id);
            if (hatchery != null)
            {
                _context.Hatcheries.Remove(hatchery);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HatcheryExists(int id)
        {
            return _context.Hatcheries.Any(e => e.Id == id);
        }
    }
}
