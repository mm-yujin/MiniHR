using System.ComponentModel.DataAnnotations;

namespace MiniHR.Models
{
    //근로소득공제 테이블
    public class DeductionBracket
    {
        public int Id { get; set; }
        public decimal Threshold { get; set; }     // 해당 구간 시작 금액 (예: 500만, 1500만...)
        public decimal BaseDeduction { get; set; } // 해당 구간 기본 공제액
        public decimal TaxRate { get; set; }       // 초과 금액에 대한 공제율
    }

    //소득세 세율 테이블
    public class TaxBracket
    {
        public int Id { get; set; }
        public decimal Threshold { get; set; }     // 과표 시작액
        public decimal ProgressiveDeduction { get; set; } // 누진공제액
        public decimal TaxRate { get; set; }       // 세율
    }
}
