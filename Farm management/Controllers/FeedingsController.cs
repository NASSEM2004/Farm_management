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
    public class FeedingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Feedings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Feeding.Include(f => f.Barn);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Feedings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feeding = await _context.Feeding
                .Include(f => f.Barn)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feeding == null)
            {
                return NotFound();
            }

            return View(feeding);
        }

        // GET: Feedings/Create
        public IActionResult Create()
        {
            ViewData["BarnId"] = new SelectList(_context.Barns, "Id", "Name");
            return View();
        }

        // POST: Feedings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Feeding feeding, string[] mealTimes)
        {
            // دمج الأوقات أولاً
            if (mealTimes != null && mealTimes.Any())
            {
                feeding.MealTimesJson = string.Join(", ", mealTimes);
            }

            // إزالة التحقق من الحقول التي قد تسبب تعليق الحفظ
            ModelState.Remove("Barn"); // إزالة التحقق من كائن الحظيرة نفسه
            ModelState.Remove("MealTimesJson"); // لأننا ملأناه يدوياً فوق

            if (ModelState.IsValid)
            {
                _context.Add(feeding);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // إذا وصلنا هنا فهناك خطأ، سنعيد تعبئة القائمة ونعرض الصفحة
            ViewData["BarnId"] = new SelectList(_context.Barns, "Id", "Name", feeding.BarnId);
            return View(feeding);
        }
        // GET: Feedings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feeding = await _context.Feeding.FindAsync(id);
            if (feeding == null)
            {
                return NotFound();
            }
            ViewData["BarnId"] = new SelectList(_context.Barns, "Id", "Name", feeding.BarnId);
            return View(feeding);
        }

        // POST: Feedings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Feeding feeding, string[] mealTimes)
        {
            if (id != feeding.Id) return NotFound();

            if (mealTimes != null && mealTimes.Any())
            {
                feeding.MealTimesJson = string.Join(", ", mealTimes);
            }

            ModelState.Remove("Barn");
            ModelState.Remove("MealTimesJson");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feeding);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedingExists(feeding.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BarnId"] = new SelectList(_context.Barns, "Id", "Name", feeding.BarnId);
            return View(feeding);
        }
        // GET: Feedings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feeding = await _context.Feeding
                .Include(f => f.Barn)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feeding == null)
            {
                return NotFound();
            }

            return View(feeding);
        }

        // POST: Feedings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feeding = await _context.Feeding.FindAsync(id);
            if (feeding != null)
            {
                _context.Feeding.Remove(feeding);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedingExists(int id)
        {
            return _context.Feeding.Any(e => e.Id == id);
        }
    }
}
