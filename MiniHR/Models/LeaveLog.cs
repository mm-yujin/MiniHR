using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHR.Models
{
    public class LeaveLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string EmployeeNumber { get; set; } = default!;

        public Employee? Employee { get; set; }

        [Required]
        [Display(Name = "사용 일자")]
        public DateOnly LeaveDate { get; set; }

        [Required]
        [Display(Name = "구분")]
        public LeaveType Type { get; set; } // 연차, 오전반차, 오후반차

        [Required]
        [Column(TypeName = "decimal(3,1)")]
        [Display(Name = "차감 일수")]
        public decimal UsedDays { get; set; } // 1.0 또는 0.5

        [Display(Name = "사유")]
        public string? Reason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public enum LeaveType
        {
            Annual,         // 연차 (1.0)
            MorningHalf,    // 오전반차 (0.5)
            AfternoonHalf   // 오후반차 (0.5)
        }
    }
}