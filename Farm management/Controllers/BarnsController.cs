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
    public class BarnsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BarnsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Barns
        public async Task<IActionResult> Index()
        {
            // جلب الحظائر مع تضمين قائمة الحيوانات التابعة لكل واحدة
            var barnsWithCount = await _context.Barns
                .Include(b => b.Animals)
                .ToListAsync();

            return View(barnsWithCount);
        }

        // GET: Barns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // إضافة Include(b => b.Animals) ضرورية جداً هنا
            var barn = await _context.Barns
                .Include(b => b.Animals)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (barn == null) return NotFound();

            return View(barn);
        }

        // GET: Barns/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Barns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,kindOfLife,Capacity")] Barns barns)
        {
            if (ModelState.IsValid)
            {
                _context.Add(barns);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(barns);
        }

        // GET: Barns/Edit/5
        // GET: Barns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // التعديل هنا: أضفنا Include(b => b.Animals)
            var barns = await _context.Barns
                .Include(b => b.Animals)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (barns == null)
            {
                return NotFound();
            }
            return View(barns);
        }

        // POST: Barns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,kindOfLife,Capacity")] Barns barns)
        {
            if (id != barns.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(barns);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BarnsExists(barns.Id))
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
            return View(barns);
        }

        // GET: Barns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var barns = await _context.Barns
                .FirstOrDefaultAsync(m => m.Id == id);
            if (barns == null)
            {
                return NotFound();
            }

            return View(barns);
        }

        // POST: Barns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var barns = await _context.Barns.FindAsync(id);
            if (barns != null)
            {
                _context.Barns.Remove(barns);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BarnsExists(int id)
        {
            return _context.Barns.Any(e => e.Id == id);
        }
    }
}
