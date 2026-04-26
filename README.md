# MiniHR — ASP.NET Core 인사 관리 시스템

> C# / ASP.NET Core Razor Pages / Entity Framework Core / MS-SQL을 활용한 미니 HR 시스템 개인 프로젝트

---

## 🔗 라이브 데모

> **[Demo Link]** *(배포 후 링크 추가 예정)*

| 계정 | 사번 | 비밀번호 | 권한 |
|------|------|----------|------|
| 관리자 | `26040001` | `12345678` | Admin |
| 일반 사원 | `16040001` | `16040001` | User |

---

## 📌 프로젝트 개요

실무 환경과 유사한 스택(ASP.NET / Razor Pages / MS-SQL)을 직접 경험해보기 위해 제작한 인사 관리 웹 애플리케이션입니다.
사원 관리, 출퇴근 기록, 연차 계산, 급여 계산 및 회계 전표 생성까지 HR의 기본 라이프사이클을 구현했습니다.

---

## 🛠 기술 스택

| 구분 | 기술 |
|------|------|
| Framework | ASP.NET Core 8.0 (Razor Pages) |
| ORM | Entity Framework Core 8.x |
| DB | MS SQL Server (LocalDB → Azure SQL) |
| 인증 | Cookie Authentication |
| Frontend | Bootstrap 5, JavaScript |
| 배포 | Azure App Service + Azure SQL |

### 아키텍처 구조

```
[브라우저]
    ↕
[ASP.NET Core Razor Pages]  ← 화면(cshtml) + 핸들러(cs)
    ↕
[Service Layer]             ← 급여 계산 등 비즈니스 로직 분리
    ↕
[Entity Framework Core]     ← C# ↔ SQL 변환 (ORM)
    ↕
[MS SQL Server]             ← 데이터 저장
```

---

## ✅ 구현 기능

### 1. 사원 관리
- 사원 등록 / 수정 / 삭제 / 목록 조회
- 스캐폴딩(Scaffolding) 기반 CRUD 자동 생성 후 커스터마이징
- 사원 등록 시 입사년월 기반 사번 자동 채번 (예: `2604XXXX`)
- 이름 / 부서 검색 필터 + 페이징 (10건 단위)

### 2. 인증 및 권한 관리
- Cookie Authentication 기반 로그인 / 로그아웃
- 역할 구분: `Admin` (관리자) / `User` (일반 사원)
- 권한에 따른 UI 분기 처리
  - 연봉, 타인 출퇴근 버튼 등은 관리자 또는 본인만 열람 가능
  - 사원 수정/삭제는 관리자 전용
- 미인증 접근 시 로그인 페이지 리다이렉트
- 권한 없는 접근 시 Forbidden 페이지 처리

### 3. 출퇴근 관리
- 출근 / 퇴근 버튼으로 근태 기록
- 오늘 출근 여부에 따라 버튼 상태 자동 전환 (출근 전 → 퇴근 전 → 업무 종료)
- 중복 출근 방지 (프론트 + 서버 양쪽 방어 로직)
- 전날 퇴근 미처리 시 자동 마감 후 당일 출근 처리
- 메인 목록 상단에 오늘 출근자 / 미출근자 수 실시간 표시

### 4. 사원 상세 페이지
- 오늘의 출퇴근 현황 (출근 시간 / 퇴근 시간 / 업무 중 여부)
- 전체 출퇴근 이력 페이징 조회 (최신순, 10건 단위)
- 연차 현황 카드 (총 연차 / 사용 연차 / 잔여 연차 + 프로그레스바)
- 본인 또는 관리자만 접근 가능

### 5. 연차 관리
- 입사일 기준 자동 계산
  - 1년 미만: 매월 1개 발생
  - 1년 이상: 15개 이상 (근속 연수 반영)
- 연차 / 오전반차 / 오후반차 구분 (1.0 / 0.5일 단위)
- 사원별 사용 연차 합산 조회

### 6. 급여 계산
- 연봉 기반 월 기본급 산출
- 식대 비과세 한도 반영 (기본값 20만 원)
- 4대 보험 공제 계산 (근로자 / 사업주 부담분 구분)
  - 국민연금, 건강보험, 장기요양, 고용보험, 산재보험
- 소득세 계산
  - 근로소득공제 구간 테이블 기반
  - 과세표준 구간별 세율 테이블 기반
  - 지방소득세 포함
- 보험 요율 및 공제 기준은 DB 테이블(`SalarySetting`)로 관리 → 세법 변경 시 테이블에서 수정 가능
- 월 급여 생성 시 기존 데이터 삭제 후 재계산

### 7. 급여 명세서
- 사원별 지급 항목 / 공제 항목 / 실수령액 명세서 화면
- 브라우저 인쇄 기능으로 PDF 저장 지원

### 8. 회계 전표 자동 생성
- 급여 계산 실행 시 복식부기 전표 자동 생성
  - 차변: 급여, 복리후생비(식대), 세금과공과(회사 부담 4대 보험)
  - 대변: 예수금(각 보험 및 세금), 미지급금(실수령액)
- 트랜잭션 처리로 급여 로그 + 전표 생성의 원자성 보장

---

## 💡 구현 시 주요 고려사항

### 비동기 처리
웹 서버의 스레드 효율을 위해 DB I/O가 발생하는 모든 핸들러에 `async / await` 패턴 적용.

### 서비스 레이어 분리
급여 계산 및 전표 생성 로직은 `SalaryService`로 분리하여 Razor Page 핸들러에서 비즈니스 로직을 제거. 추후 API 엔드포인트나 배치 작업으로 재사용 가능한 구조로 설계.

### 데이터 무결성
급여 생성 시 `BeginTransactionAsync`로 기존 데이터 삭제 + 신규 생성을 하나의 트랜잭션으로 묶어 중간 실패 시 롤백 처리.

### 과세 로직의 데이터 기반 관리
근로소득공제 구간(`DeductionBracket`)과 소득세 세율 구간(`TaxBracket`)을 테이블로 관리하여 세법 개정 시 코드 수정 없이 DB 데이터만 변경하면 반영되도록 설계.

### 읽기 전용 쿼리 최적화
수정이 필요 없는 조회 쿼리에 `.AsNoTracking()` 적용하여 EF Core의 Change Tracker 부하 제거.

### IQueryable 활용
검색 필터와 페이징을 `.Skip()` / `.Take()`로 DB 단에서 처리하여 불필요한 전체 데이터 메모리 로딩 방지.

---

## 📂 프로젝트 구조

```
MiniHR/
├── Models/
│   ├── Employee.cs          # 사원 (사번, 이름, 부서, 입사일, 연봉, 역할, 비밀번호)
│   ├── Attendance.cs        # 출퇴근 기록
│   ├── LeaveLog.cs          # 연차 사용 기록
│   ├── SalaryLog.cs         # 급여 지급 기록
│   ├── SalarySetting.cs     # 보험 요율 설정
│   ├── DeductionBracket.cs  # 근로소득공제 구간 테이블
│   ├── TaxBracket.cs        # 소득세 세율 구간 테이블
│   ├── JournalEntry.cs      # 회계 전표
│   └── AppDbContext.cs      # EF Core DbContext
├── Pages/
│   ├── Account/             # 로그인, 로그아웃, 권한없음
│   ├── Employees/           # 사원 목록, 상세, 등록, 수정, 삭제
│   └── Salary/              # 급여 계산, 명세서, 요율 설정
├── Services/
│   └── SalaryService.cs     # 급여 계산 및 전표 생성 비즈니스 로직
├── Migrations/              # EF Core 마이그레이션 이력
└── Program.cs               # 미들웨어 및 서비스 등록
```

---

## 🗄 DB 설계 (주요 테이블)

```
Employees ──┬── Attendances   (출퇴근 기록, FK: EmployeeNumber)
            ├── LeaveLogs     (연차 사용 기록, FK: EmployeeNumber)
            └── SalaryLogs    (급여 지급 기록, FK: EmployeeNumber)

SalarySettings               (보험 요율 설정)
DeductionBrackets            (근로소득공제 구간)
TaxBrackets                  (소득세 세율 구간)
JournalEntries ── JournalDetails  (회계 전표 헤더/라인)
```

> FK는 DB의 자동증가 PK(Id)가 아닌 비즈니스 키(EmployeeNumber)를 참조하도록 `HasPrincipalKey`로 명시 설정

---

## 📅 개발 기간

| 일차 | 주요 작업 |
|------|-----------|
| 1일차 | 환경 설정, 모델 설계, EF Core 마이그레이션, 스캐폴딩, 출퇴근 기능 |
| 2일차 | Cookie 인증/권한, 페이징/검색, 연차 계산, 급여 계산, 전표 자동 생성 |

---

## 🤖 AI 활용 내역

본 프로젝트는 아래 영역에서 Gemini 및 Claude를 활용했습니다.

- **초기 구조 설계**: 기술 스택 선정 및 레이어 구조(MVC vs Razor Pages, Service 분리 등) 설계 시 방향 검토
- **UI 마크업**: cshtml 화면 코드 작성 및 Bootstrap 레이아웃 정리
- **트러블슈팅**: 오류 원인 파악 및 해결 방향 논의

설계 판단, 비즈니스 로직(급여 계산, 전표 생성, 인증/권한, 출퇴근 방어 로직 등) 구현 및
DB 설계와 마이그레이션, 리팩토링 등의 작업은 직접 수행했습니다.

---

## 🔧 향후 개선 예정

- [ ] Azure App Service 배포 + Azure SQL 연결
- [ ] 전표 목록 조회 화면
- [ ] 연차 사용 신청 기능
- [ ] MS-SQL 저장 프로시저를 활용한 급여 확정 로직 이관
- [ ] Non-Clustered Index 적용 (SalaryLog.YearMonth + EmployeeNumber)
- [ ] Window Function 기반 부서별 인건비 통계 쿼리
