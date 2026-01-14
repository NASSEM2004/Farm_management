using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm_management.Models
{
    public class Animals
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Species { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public int BarnId { get; set; }
        [ForeignKey("BarnId")]
        public Barns? Barn { get; set; }
    }
}
