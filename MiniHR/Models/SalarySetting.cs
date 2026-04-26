using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHR.Models
{
    public class SalarySetting
    {
        [Key]
        public int Id { get; set; }

        public string Year { get; set; } = "2026"; // 적용 년도

        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal NationalPensionRate { get; set; } = 0.045m;  // 국민연금 (4.5%)

        [DisplayFormat(DataFormatString = "{0:N5}")]
        public decimal HealthInsuranceRate { get; set; } = 0.03545m; // 건강보험 (3.545%)

        [DisplayFormat(DataFormatString = "{0:N4}")]
        public decimal LongTermCareRate { get; set; } = 0.1295m;    // 장기요양 (건보료의 12.95%)

        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal EmploymentInsuranceRate { get; set; } = 0.009m; // 고용보험 (0.9%)

        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal CompanyEmploymentInsuranceRate { get; set; } = 0.015m; // 회사측고용보험 (1.15%)

        [DisplayFormat(DataFormatString = "{0:N4}")]
        public decimal IndustrialAccidentInsuranceRate { get; set; } = 0.0096m; // 산재보험 (0.96%)



        // 비과세 한도
        public decimal MealAllowanceLimit { get; set; } = 200000m;

        [Display(Name = "기본 소득세율")]
        public decimal IncomeTaxRate { get; set; } = 0.03m; // 사업소득자나 단순 계산용

        public decimal BasicDeduction { get; set; } = 1500000m;      // 기본 인적공제 (1인당)
        public decimal StandardTaxCredit { get; set; } = 130000m;    // 표준세액공제 (월 추정치)
        public decimal AdditionalDeduction { get; set; } = 12000000m; // 기타 일괄 공제액 (보험료 등 추정)
    }
}