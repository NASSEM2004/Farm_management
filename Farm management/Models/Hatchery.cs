using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm_management.Models
{
    public class Hatchery
    {
        [Key]
        public int Id { get; set; }

        // الحيوان الأول (الذكر)
        [Display(Name = "الذكر")]
        public int MaleAnimalId { get; set; }
        [ForeignKey("MaleAnimalId")]
        public virtual Animals? MaleAnimal { get; set; }

        // الحيوان الثاني (الأنثى)
        [Display(Name = "الأنثى")]
        public int FemaleAnimalId { get; set; }
        [ForeignKey("FemaleAnimalId")]
        public virtual Animals? FemaleAnimal { get; set; }

        // حظيرة الإنتاج (المكان الذي ستتم فيه العملية)
        [Display(Name = "حظيرة الإنتاج")]
        public int ProductionBarnId { get; set; }
        [ForeignKey("ProductionBarnId")]
        public virtual Barns? ProductionBarn { get; set; }

        [Required]
        [Display(Name = "وقت الإدخال")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "موعد التفقيس/الإنتاج المتوقع")]
        [DataType(DataType.DateTime)]
        public DateTime ExpectedEndDate { get; set; }

        [Display(Name = "حالة العملية")]
        public string Status { get; set; } = "قيد الانتظار"; // قيد الانتظار، جارية، مكتملة
    }
}