using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHR.Models
{
    public class SalaryLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string EmployeeNumber { get; set; } = default!;

        public virtual Employee? Employee { get; set; }

        // 202604 같은 형태로 저장하여 인덱싱 및 조회용
        [Required]
        public int YearMonth { get; set; }

        public DateTime PaymentDate { get; set; } // 실지급일

        // 급여 항목
        public decimal BaseSalary { get; set; }  // 기본급
        public decimal MealAllowance { get; set; } // 식대 (비과세)

        // 공제 항목
        public decimal NationalPension { get; set; } // 연금
        public decimal HealthInsurance { get; set; } // 건보
        public decimal LongTermCare {  get; set; }  //요양
        public decimal EmploymentInsurance { get; set; }  //고용보험
        public decimal IncomeTax { get; set; }      // 소득세

        //4대보험 회사부담금
        public decimal CompanyNationalPension { get; set; } // 연금
        public decimal CompanyHealthInsurance { get; set; } // 건보
        public decimal CompanyLongTermCare { get; set; }    // 요양
        public decimal CompanyEmploymentInsurance { get; set; } // 고용보험
        public decimal IndustrialAccidentInsurance { get; set; } // 산재보험

        public decimal NetPay => (BaseSalary + MealAllowance) - (NationalPension + HealthInsurance + LongTermCare + EmploymentInsurance + IncomeTax); // 실수령액
    }
}