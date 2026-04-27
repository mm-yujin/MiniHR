using MiniHR.Models;
using Microsoft.EntityFrameworkCore;

namespace MiniHR.Services
{
    public class SalaryService
    {
        private readonly AppDbContext _context;

        public SalaryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateMonthlySalaryProcessAsync(int yearMonth, string description)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 같은 월에 대해서 계산된 데이터가 이미 있으면 덮어씌우기 위해 기존 데이터를 지움
                var existingLogs = _context.SalaryLogs.Where(s => s.YearMonth == yearMonth);
                _context.SalaryLogs.RemoveRange(existingLogs);

                var existingEntries = _context.JournalEntries
                    .Include(j => j.Details)
                    .Where(j => j.Description.Contains(description));
                _context.JournalEntries.RemoveRange(existingEntries);

                await _context.SaveChangesAsync();

                var targets = await _context.Employees.ToListAsync();
                var setting = await _context.SalarySettings.OrderByDescending(s => s.Year).FirstOrDefaultAsync()
                              ?? new SalarySetting();

                var newLogs = new List<SalaryLog>();

                // 급여는 인당 계산을 원칙으로 해야 하므로 반복문 돌린다.
                foreach (var emp in targets)
                {
                    var log = await CalculateMonthlySalaryAsync(emp, yearMonth, setting);
                    newLogs.Add(log);
                }

                if (newLogs.Any())
                {
                    _context.SalaryLogs.AddRange(newLogs);

                    var entry = GenerateJournalEntryFromLogs(newLogs, description); //전표 생성
                    _context.JournalEntries.Add(entry);

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<SalaryLog> CalculateMonthlySalaryAsync(Employee employee, int yearMonth, SalarySetting setting)
        {
            int year = yearMonth / 100;
            int month = yearMonth % 100;
            var monthStart = new DateOnly(year, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            int totalDaysInMonth = monthEnd.Day;
            var hireDate = employee.HireDate;

            decimal ratio = 1.0m;

            // 해당 월에 입사한 경우- 일할 계산
            if (hireDate > monthStart && hireDate <= monthEnd)
            {
                // 근무일수 = 종료일 - 입사일 + 1
                int workDays = monthEnd.Day - hireDate.Day + 1;
                ratio = (decimal)workDays / totalDaysInMonth;
            }
            else if (hireDate > monthEnd)
            {
                ratio = 0;
            }

            decimal fullMonthlyTotal = Math.Floor(employee.AnnualSalary / 12);
            decimal monthlyTotal = Math.Floor(fullMonthlyTotal * ratio); 

            decimal fullMealAllowance = fullMonthlyTotal >= setting.MealAllowanceLimit ? setting.MealAllowanceLimit : fullMonthlyTotal;
            decimal mealAllowance = Math.Floor(fullMealAllowance * ratio);

            decimal taxableSalary = monthlyTotal - mealAllowance;

            decimal nationalPension = Math.Floor((taxableSalary * setting.NationalPensionRate) / 10) * 10;
            decimal healthInsurance = Math.Floor((taxableSalary * setting.HealthInsuranceRate) / 10) * 10;
            decimal longTermCare = Math.Floor((healthInsurance * setting.LongTermCareRate) / 10) * 10;
            decimal employmentInsurance = Math.Floor((taxableSalary * setting.EmploymentInsuranceRate) / 10) * 10;
            decimal incomeTax = await CalculateIncomeTaxAsync(taxableSalary, setting);

            decimal companyPension = Math.Floor((taxableSalary * setting.NationalPensionRate) / 10) * 10;
            decimal companyHealth = Math.Floor((taxableSalary * setting.HealthInsuranceRate) / 10) * 10;
            decimal companyLongTerm = Math.Floor((companyHealth * setting.LongTermCareRate) / 10) * 10;
            decimal companyEmployment = Math.Floor((taxableSalary * setting.CompanyEmploymentInsuranceRate) / 10) * 10;
            decimal industrialAccident = Math.Floor((taxableSalary * setting.IndustrialAccidentInsuranceRate) / 10) * 10;

            return new SalaryLog
            {
                EmployeeNumber = employee.EmployeeNumber,
                YearMonth = yearMonth,
                PaymentDate = DateTime.Today,
                BaseSalary = taxableSalary,
                MealAllowance = mealAllowance,
                NationalPension = nationalPension,
                HealthInsurance = healthInsurance,
                LongTermCare = longTermCare,
                EmploymentInsurance = employmentInsurance,
                IncomeTax = incomeTax,
                CompanyNationalPension = companyPension,
                CompanyHealthInsurance = companyHealth,
                CompanyLongTermCare = companyLongTerm,
                CompanyEmploymentInsurance = companyEmployment,
                IndustrialAccidentInsurance = industrialAccident
            };
        }

        private JournalEntry GenerateJournalEntryFromLogs(List<SalaryLog> logs, string description)
        {
            var entry = new JournalEntry { TransactionDate = DateTime.Today, Description = description };

            entry.Details.Add(new JournalDetail { AccountName = "급여", Debit = logs.Sum(s => s.BaseSalary) });
            entry.Details.Add(new JournalDetail { AccountName = "복리후생비(식대)", Debit = logs.Sum(s => s.MealAllowance) });

            decimal totalCompanyCost = logs.Sum(s =>
                s.CompanyNationalPension + s.CompanyHealthInsurance + s.CompanyLongTermCare +
                s.CompanyEmploymentInsurance + s.IndustrialAccidentInsurance);
            entry.Details.Add(new JournalDetail { AccountName = "세금과공과(회사부담금)", Debit = totalCompanyCost });

            entry.Details.Add(new JournalDetail { AccountName = "예수금(국민연금)", Credit = logs.Sum(s => s.NationalPension + s.CompanyNationalPension) });
            entry.Details.Add(new JournalDetail { AccountName = "예수금(건강보험)", Credit = logs.Sum(s => s.HealthInsurance + s.LongTermCare + s.CompanyHealthInsurance + s.CompanyLongTermCare) });
            entry.Details.Add(new JournalDetail { AccountName = "예수금(고용보험)", Credit = logs.Sum(s => s.EmploymentInsurance + s.CompanyEmploymentInsurance) });
            entry.Details.Add(new JournalDetail { AccountName = "예수금(산재보험)", Credit = logs.Sum(s => s.IndustrialAccidentInsurance) });
            entry.Details.Add(new JournalDetail { AccountName = "예수금(소득세)", Credit = logs.Sum(s => s.IncomeTax) });
            entry.Details.Add(new JournalDetail { AccountName = "미지급금(급여)", Credit = logs.Sum(s => s.NetPay) });

            return entry;
        }

        private async Task<decimal> CalculateIncomeTaxAsync(decimal monthlyTaxableIncome, SalarySetting setting)
        {
            //연간 총급여 추정 > 근로소득공제 > 과표 계산 > 산출세액 계산 > 결정세액&소득세 산출
            decimal annualTotal = monthlyTaxableIncome * 12;
                        
            decimal incomeDeduction = await GetLaborIncomeDeduction(annualTotal); //근로소득공제 (DeductionBrackets 테이블 사용)

            decimal taxStandard = Math.Max(0, annualTotal - incomeDeduction - setting.BasicDeduction - setting.AdditionalDeduction);

            decimal annualTax = await GetCalculatedTax(taxStandard); //과세표준 계산 (TaxBrackets 테이블)

            decimal monthlyTax = (Math.Max(0, annualTax - setting.StandardTaxCredit) / 12);
            decimal totalMonthlyTax = monthlyTax * 1.1m; //지방소득세는 일단 한번에 더해서 해둔다.

            return Math.Floor(totalMonthlyTax / 10) * 10; //10원 이하 절사
        }

        private async Task<decimal> GetCalculatedTax(decimal taxStandard) //소득세 세율 구간 계산
        {
            var taxBrackets = await _context.TaxBrackets
                                            .OrderByDescending(b => b.Threshold)
                                            .ToListAsync();

            foreach (var bracket in taxBrackets)
            {
                if (taxStandard > bracket.Threshold)
                {
                    // 공식: (과세표준 * 세율) - 누진공제액
                    return (taxStandard * bracket.TaxRate) - bracket.ProgressiveDeduction;
                }
            }

            return 0;
        }

        private async Task<decimal> GetLaborIncomeDeduction(decimal annualTotal) //근로소득공제 계산
        {
            var deductionBrackets = await _context.DeductionBrackets
                                                  .OrderByDescending(b => b.Threshold)
                                                  .ToListAsync();

            foreach (var bracket in deductionBrackets)
            {
                if (annualTotal > bracket.Threshold)
                {
                    return bracket.BaseDeduction + (annualTotal - bracket.Threshold) * bracket.TaxRate;
                }
            }

            return annualTotal * 0.7m; // 최저 구간 기본값
        }
    }
}