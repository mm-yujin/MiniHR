using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHR.Models
{
    public class Employee
    {
        
        public int Id { get; set; } // DB 관리용 인덱스

        [Display(Name = "사원번호")]
        [Required(ErrorMessage = "사원번호는 반드시 입력해야 합니다.")]
        public string EmployeeNumber { get; set; } = string.Empty;

        [Display(Name = "이름")]
        [Required(ErrorMessage = "이름은 반드시 입력해야 합니다.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "부서")]
        public string Department {  get; set; } = string.Empty;

        [Display(Name = "입사일자")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required(ErrorMessage = "입사일자는 반드시 입력해야 합니다.")]
        public DateOnly HireDate { get; set; }

        [Display(Name = "연봉")]
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Required(ErrorMessage = "연봉은 반드시 입력해야 합니다.")]
        public decimal AnnualSalary { get; set; } = 0;


        public enum RoleType {
            [Display(Name = "관리자")] Admin,
            [Display(Name = "일반 사용자")] User 
        }

        [Display(Name = "권한")]
        public RoleType Role { get; set; } = RoleType.User;

        [Display(Name = "비밀번호")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "비밀번호는 8자 이상이어야 합니다.")]
        public string Password { get; set; } = string.Empty;
    }
}
