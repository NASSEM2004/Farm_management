using System.ComponentModel.DataAnnotations;

namespace Farm_management.Models
{
    public class Barns
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string kindOfLife { get; set; }
        public int Capacity { get; set; }
        public virtual ICollection<Animals>? Animals { get; set; }

    }
}
