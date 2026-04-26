using Microsoft.EntityFrameworkCore;

namespace MiniHR.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Employee)                    // Attendance는 하나의 Employee를 가짐
                .WithMany()                                 // Employee는 여러 Attendance를 가질 수 있음
                .HasForeignKey(a => a.EmployeeId)           // Attendance의 EmployeeId 속성을 FK로 사용
                .HasPrincipalKey(e => e.EmployeeNumber);    // 참조 대상을 Id가 아닌 EmployeeNumber로 지정
        }
    }
}
