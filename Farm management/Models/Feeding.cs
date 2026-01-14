using System.ComponentModel.DataAnnotations;

namespace Farm_management.Models
{
    public class Feeding
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "الحظيرة")]
        public int BarnId { get; set; }
        public virtual Barns? Barn { get; set; }

        [Display(Name = "عدد الوجبات اليومية")]
        public int MealsCount { get; set; }

        [Display(Name = "وزن الوجبة الواحدة (كجم)")]
        public double SingleMealWeight { get; set; }

        [Display(Name = "نوع الطعام")]
        public string FoodType { get; set; }

        // سنخزن أوقات الوجبات كـ سلسلة نصية مفصولة بفاصلة أو نستخدم حقولاً ديناميكية
        [Display(Name = "أوقات الوجبات")]
        public string MealTimesJson { get; set; }
    }
}
