using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Farm_management.Data;
using Farm_management.Models;
using Farm_management.Services;

namespace Farm_management.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnimalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Animals
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Animals.Include(a => a.Barn);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Animals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animals = await _context.Animals
                .Include(a => a.Barn)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animals == null)
            {
                return NotFound();
            }

            return View(animals);
        }

        // GET: Animals/Create
        [HttpGet]
        public JsonResult GetBarnsByKind(string kind)
        {
            // جلب كل الحظائر ثم فلترتها باستخدام محرك الذكاء الخاص بنا
            var allBarns = _context.Barns.ToList();

            var filteredBarns = allBarns
                .Where(b => TextHelper.IsSimilar(kind, b.kindOfLife, maxErrors: 1)) // فحص ذكي
                .Select(b => new { id = b.Id, name = b.Name })
                .ToList();

            return Json(filteredBarns);
        }

        // GET: Animals/Create
        public IActionResult Create()
        {
            var availableKinds = _context.Barns.Select(b => b.kindOfLife).Distinct().ToList();
            ViewData["AnimalTypes"] = new SelectList(availableKinds);

            // في البداية، نرسل قائمة حظائر فارغة أو نطلب من المستخدم اختيار النوع أولاً
            ViewData["BarnId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            return View();
        }
        // POST: Animals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Species,Age,Gender,BarnId")] Animals animals)
        {
            // جلب كل الأنواع المتاحة في الحظائر حالياً (خروف، بقرة، إلخ)
            var existingKinds = await _context.Barns.Select(b => b.kindOfLife).Distinct().ToListAsync();

            // --- بداية التعامل الذكي مع النص ---
            foreach (var kind in existingKinds)
            {
                // إذا كانت الكلمة المدخلة قريبة جداً من نوع موجود (خطأ بحرف واحد مثلاً)
                if (TextHelper.IsSimilar(animals.Name, kind, maxErrors: 1))
                {
                    animals.Name = kind; // تصحيح تلقائي للنص
                    break;
                }
            }
            // --- نهاية التعامل الذكي ---

            // الكود السابق الخاص بفحص السعة والنوع يكمل عمله هنا...
            var selectedBarn = await _context.Barns
                .Include(b => b.Animals)
                .FirstOrDefaultAsync(b => b.Id == animals.BarnId);

            if (selectedBarn != null)
            {
                if (selectedBarn.kindOfLife != animals.Name)
                {
                    ModelState.AddModelError("Name", $"هذا الحيوان لا يناسب هذه الحظيرة. هل تقصد {selectedBarn.kindOfLife}؟");
                }

                int currentCount = selectedBarn.Animals?.Count ?? 0;
                if (currentCount >= selectedBarn.Capacity)
                {
                    ModelState.AddModelError("BarnId", "الحظيرة ممتلئة.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(animals);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // إعادة تعبئة القوائم في حال الخطأ
            ViewData["BarnId"] = new SelectList(_context.Barns, "Id", "Name", animals.BarnId);
            return View(animals);
        }

        // GET: Animals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animals = await _context.Animals.FindAsync(id);
            if (animals == null)
            {
                return NotFound();
            }
            ViewData["BarnId"] = new SelectList(_context.Barns, "Id", "Name", animals.BarnId);
            return View(animals);
        }

        // POST: Animals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Species,Age,Gender,BarnId")] Animals animals)
        {
            if (id != animals.Id) return NotFound();

            var selectedBarn = await _context.Barns.FindAsync(animals.BarnId);

            if (selectedBarn != null && selectedBarn.kindOfLife != animals.Name)
            {
                ModelState.AddModelError("Name", $"خطأ في التعديل: الحظيرة المختارة مخصصة لـ ({selectedBarn.kindOfLife}).");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(animals);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimalsExists(animals.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BarnId"] = new SelectList(_context.Barns, "Id", "Name", animals.BarnId);
            return View(animals);
        }

        // GET: Animals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animals = await _context.Animals
                .Include(a => a.Barn)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animals == null)
            {
                return NotFound();
            }

            return View(animals);
        }

        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animals = await _context.Animals.FindAsync(id);
            if (animals != null)
            {
                _context.Animals.Remove(animals);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnimalsExists(int id)
        {
            return _context.Animals.Any(e => e.Id == id);
        }
    }
}
