using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHR.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("EmployeeId")]
        public string EmployeeId { get; set; }         //fk

        public Employee? Employee { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }     //아직 퇴근 안했을 수도 있으니까
        public DateOnly WorkDate { get; set; }
    }
}
