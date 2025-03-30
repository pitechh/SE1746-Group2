using Business.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Contexts
{
    public class AppDbContext : DbContext
    {
        #region Constructor
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        #endregion

        #region DbSet Properties
        public DbSet<Education> Educations { get; set; }
        public DbSet<WorkHistory> WorkHistories { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<CategoryPerson> CategoryPersons { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Technology> Technologies { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Pay> Pays { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }

        // ✅ Thêm dòng này để làm việc với công thức PIT / BHXH
        public DbSet<SalaryFormulaConfig> SalaryFormulaConfigs { get; set; }
        #endregion

        #region Fluent API Config
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Tự động tìm các cấu hình từ cùng Assembly
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        #endregion
    }
}
