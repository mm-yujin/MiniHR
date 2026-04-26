using Microsoft.EntityFrameworkCore;

namespace MiniHR.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<LeaveLog>  LeaveLogs { get; set; }
        public DbSet<SalaryLog> SalaryLogs { get; set; }

        //이하 DbSet들은 Employee와의 연관관계가 없는 Table이라 OnModelCreating서 설정 안함
        public DbSet<SalarySetting> SalarySettings { get; set; }
        public DbSet<DeductionBracket> DeductionBrackets { get; set; }
        public DbSet<TaxBracket> TaxBrackets { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; } = default!;
        public DbSet<JournalDetail> JournalDetails { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Employee)                    // Attendance는 하나의 Employee를 가짐
                .WithMany()                                 // Employee는 여러 Attendance를 가질 수 있음
                .HasForeignKey(a => a.EmployeeNumber)           // Attendance의 EmployeeId 속성을 FK로 사용
                .HasPrincipalKey(e => e.EmployeeNumber);    // 참조 대상을 Id가 아닌 EmployeeNumber로 지정

            modelBuilder.Entity<LeaveLog>()
                .HasOne(l => l.Employee)
                .WithMany() // 한 직원은 여러 연차 기록을 가질 수 있음
                .HasForeignKey(l => l.EmployeeNumber)
                .HasPrincipalKey(e => e.EmployeeNumber); // PK(Id) 대신 사번(EmployeeNumber)을 참조

            modelBuilder.Entity<SalaryLog>()
                .HasOne(s => s.Employee)        
                .WithMany()                     
                .HasForeignKey(s => s.EmployeeNumber)
                .HasPrincipalKey(e => e.EmployeeNumber);
        }
    }
}
