using System.ComponentModel.DataAnnotations;

namespace MiniHR.Models
{
    public class Employee
    {
        [Display(Name = "인덱스 번호")]
        public int Id { get; set; }
        
        [Display(Name = "이름")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "부서")]
        public string Department {  get; set; } = string.Empty;

        [Display(Name = "입사일자")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateOnly HireDate { get; set; }

        [Display(Name = "연봉")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal AnnualSalary { get; set; } = 0;
    }
}
