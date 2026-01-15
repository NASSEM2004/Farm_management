using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm_management.Models
{
    public class Clinic
    {
        [Key]
        public int Id { get; set; } // رقم الحالة

        [Required(ErrorMessage = "يرجى إدخال رقم الحيوان")]
        [Display(Name = "رقم الحيوان")]
        public int AnimalId { get; set; } // المستخدم يدخله يدوياً

        // ربط اختياري مع كلاس الحيوانات للعرض فقط
        [ForeignKey("AnimalId")]
        public virtual Animals? Animal { get; set; }

        [Required(ErrorMessage = "يرجى كتابة التشخيص")]
        [Display(Name = "التشخيص الطبي")]
        public string Diagnosis { get; set; }

        [Display(Name = "تفاصيل الحالة")]
        public string Details { get; set; }

        [Required(ErrorMessage = "يرجى تحديد الأدوية")]
        [Display(Name = "الأدوية الموصوفة")]
        public string Medications { get; set; }

        [Display(Name = "عدد مرات الجرعة")]
        public int DoseFrequency { get; set; }

        [Required]
        [Display(Name = "تاريخ الدخول")]
        [DataType(DataType.DateTime)]
        public DateTime EntryDate { get; set; }

        [Required]
        [Display(Name = "تاريخ نهاية العلاج")]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        // الحظيرة (سيتم نقل الحيوان إليها لتلقي العلاج أو بعده)
        [Display(Name = "حظيرة العزل/العلاج")]
        public int BarnId { get; set; }

        [ForeignKey("BarnId")]
        public virtual Barns? Barn { get; set; }

        [Display(Name = "حالة الحالة")]
        public string Status { get; set; } = "تحت العلاج"; // تحت العلاج، تماثل للشفاء، منتهية
    }
}