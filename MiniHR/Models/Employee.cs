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


        public int GetTotalAnnualLeave(DateTime criteriaDate)
        {
            // 입사일과 기준일 사이의 개월 수
            int totalMonths = ((criteriaDate.Year - HireDate.Year) * 12) + criteriaDate.Month - HireDate.Month;

            // 기준일의 '일'이 입사일의 '일'보다 작으면 아직 한 달이 안 찬 것이므로 -1
            if (criteriaDate.Day < HireDate.Day) totalMonths--;

            // 근로기준법에 따른 연차 산정
            if (totalMonths < 12)
            {
                // 1년 미만: 1개월 만근 시 1개 (최대 11개)
                return Math.Max(0, totalMonths);
            }
            else
            {
                // 1년 이상: 기본 15개 + 2년마다 1개 가산 (최대 25개)
                int years = totalMonths / 12;
                int extraDays = (years - 1) / 2;
                return Math.Min(15 + extraDays, 25);
            }
        }
    }
}
