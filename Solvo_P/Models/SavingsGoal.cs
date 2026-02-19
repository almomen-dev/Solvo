using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Solvo.Models
{
    public class SavingsGoal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TargetAmount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime TargetDate { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal CurrentAmount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}