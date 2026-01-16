using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Farm_management.Data; // تأكد من اسم الـ Namespace لمشروعك

[Route("api/[controller]")]
[ApiController]
public class FarmDataController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FarmDataController(ApplicationDbContext context)
    {
        _context = context;
    }

    // جلب بيانات الحظائر
    [HttpGet("barns")]
    public async Task<IActionResult> GetBarns()
    {
        var barns = await _context.Barns.ToListAsync();
        return Ok(barns);
    }

    // جلب بيانات الحيوانات (مع اسم الحظيرة)
    [HttpGet("animals")]
    public async Task<IActionResult> GetAnimals()
    {
        var animals = await _context.Animals
            .Include(a => a.Barn)
            .Select(a => new {
                id = a.Id,
                name = a.Name,
                species = a.Species,
                age = a.Age,
                gender = a.Gender,
                barnName = a.Barn.Name
            })
            .ToListAsync();
        return Ok(animals);
    }

    [HttpGet("hatchery")]
    public async Task<IActionResult> GetHatchery()
    {
        var hatchery = await _context.Hatcheries
            .Include(h => h.MaleAnimal)     // جلب بيانات الذكر
            .Include(h => h.FemaleAnimal)   // جلب بيانات الأنثى
            .Include(h => h.ProductionBarn) // جلب بيانات الحظيرة
            .Select(h => new {
                Id = h.Id,
                // نأخذ الفصيلة من حيوان الذكر كمثال لنوع العملية
                Species = h.MaleAnimal != null ? h.MaleAnimal.Species : "غير محدد",
                // عرض أسماء الأبوين في عمود الحيوان
                ParentInfo = (h.MaleAnimal != null ? h.MaleAnimal.Name : "?") + " x " + (h.FemaleAnimal != null ? h.FemaleAnimal.Name : "?"),
                Quantity = 1, // أو أي قيمة تعبر عن العملية
                ProductionDate = h.ExpectedEndDate,
                Status = h.Status,
                BarnName = h.ProductionBarn != null ? h.ProductionBarn.Name : "غير محدد"
            })
            .ToListAsync();

        return Ok(hatchery);
    }
    // جلب بيانات العيادة
    [HttpGet("clinic")]
    public async Task<IActionResult> GetClinic()
    {
        var clinic = await _context.Clinics
            .Include(c => c.Animal)
            .Select(c => new {
                id = c.Id,
                animalId = c.AnimalId,
                species = c.Animal.Species,
                diagnosis = c.Diagnosis,
                entryDate = c.EntryDate.ToShortDateString(),
                endDate = c.EndDate.ToShortDateString()
            })
            .ToListAsync();
        return Ok(clinic);
    }
}