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
    public class ClinicsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClinicsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clinics
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Clinics
                .Include(c => c.Animal)
                .Include(c => c.Barn);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Clinics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clinic = await _context.Clinics
                .Include(c => c.Animal)
                .Include(c => c.Barn)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clinic == null)
            {
                return NotFound();
            }

            return View(clinic);
        }

        // 1. الدالة التي تفتح صفحة الإدخال (GET)
        public IActionResult Create()
        {
            // إرسال قائمة الحظائر ليختار المستخدم منها حظيرة العزل
            ViewData["BarnId"] = new SelectList(_context.Barns, "Id", "Name");
            return View();
        }

        // 2. الدالة التي تستقبل البيانات وتحفظها (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Clinic clinic)
        {
            // البحث عن الحيوان بالرقم الذي كتبه المستخدم
            var animal = await _context.Animals.FindAsync(clinic.AnimalId);

            if (animal == null)
            {
                // إضافة رسالة خطأ تظهر بجانب مربع نص رقم الحيوان
                ModelState.AddModelError("AnimalId", "عذراً، هذا الحيوان غير موجود في النظام. يرجى التأكد من الرقم.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(clinic);

                // إذا وجدنا الحيوان، نقوم بتغيير حظيرته فوراً
                if (animal != null)
                {
                    animal.BarnId = clinic.BarnId;
                    _context.Update(animal);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // إذا فشل الإدخال أو الرقم غير موجود، نعيد تحميل قائمة الحظائر لكي لا تظهر فارغة
            ViewData["BarnId"] = new SelectList(_context.Barns, "Id", "Name", clinic.BarnId);
            return View(clinic);
        }

        // GET: Clinics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
            {
                return NotFound();
            }
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Name", clinic.AnimalId);
            ViewData["BarnId"] = new SelectList(_context.Barns, "Id", "Name", clinic.BarnId);
            return View(clinic);
        }

        // POST: Clinics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AnimalId,Diagnosis,Details,Medications,DoseFrequency,EntryDate,EndDate,BarnId,Status")] Clinic clinic)
        {
            if (id != clinic.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clinic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClinicExists(clinic.Id))
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
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Name", clinic.AnimalId);
            ViewData["BarnId"] = new SelectList(_context.Barns, "Id", "Name", clinic.BarnId);
            return View(clinic);
        }

        // GET: Clinics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clinic = await _context.Clinics
                .Include(c => c.Animal)
                .Include(c => c.Barn)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clinic == null)
            {
                return NotFound();
            }

            return View(clinic);
        }

        // POST: Clinics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic != null)
            {
                _context.Clinics.Remove(clinic);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClinicExists(int id)
        {
            return _context.Clinics.Any(e => e.Id == id);
        }
    }
}
