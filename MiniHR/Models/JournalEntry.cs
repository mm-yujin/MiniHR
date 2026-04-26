using System.ComponentModel.DataAnnotations;

namespace MiniHR.Models
{
    public class JournalEntry //하나의 전표안에
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } // 전표 일자

        [Required]
        public string Description { get; set; } = default!; // 적요

        public virtual List<JournalDetail> Details { get; set; } = new(); //한 전표는 여러 차대변을 갖는다
    }

    public class JournalDetail //여러개의 차대변
    {
        [Key]
        public int Id { get; set; }

        public int JournalEntryId { get; set; }
        public virtual JournalEntry JournalEntry { get; set; } = default!;

        public string AccountName { get; set; } = default!; // 계정 과목

        public decimal Debit { get; set; }  // 차변
        public decimal Credit { get; set; } // 대변
    }
}