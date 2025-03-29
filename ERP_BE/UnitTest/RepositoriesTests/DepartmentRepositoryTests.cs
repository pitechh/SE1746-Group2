using Business.Domain.Models;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using UnitTest.Tool;

namespace UnitTest.RepositoriesTests
{
    public class DepartmentRepositoryTests : TestWithSqlite<AppDbContext>
    {
        private void SeedDepartments(AppDbContext context)
        {
            context.Departments.AddRange(
                new Department { Id = 1, Name = "IT", IsDeleted = false },
                new Department { Id = 2, Name = "HR", IsDeleted = false },
                new Department { Id = 3, Name = "Finance", IsDeleted = false },
                new Department { Id = 4, Name = "R&D", IsDeleted = false },
                new Department { Id = 5, Name = "Marketing", IsDeleted = false },
                new Department { Id = 6, Name = "Accounting", IsDeleted = false },
                new Department { Id = 7, Name = "Deleted Dep", IsDeleted = true } // sẽ bị lọc
            );
            context.SaveChanges();
        }

        [Fact]
        public async Task FindByNameAsync_Contains_ShouldReturnMatches()
        {
            using var context = new AppDbContext(Options);
            context.Database.EnsureCreated();
            SeedDepartments(context);

            var repo = new DepartmentRepository(context);
            var result = await repo.FindByNameAsync("a"); // Finance, Marketing, Accounting...

            Assert.True(result.Count > 0);
            Assert.All(result, d => Assert.Contains("a", d.Name, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task FindByNameAsync_Equals_ShouldReturnExact()
        {
            using var context = new AppDbContext(Options);
            context.Database.EnsureCreated();
            SeedDepartments(context);

            var repo = new DepartmentRepository(context);
            var result = await repo.FindByNameAsync("HR", absolute: true);

            Assert.Single(result);
            Assert.Equal("HR", result[0].Name);
        }

        [Fact]
        public async Task FindByNameAsync_EmptyFilter_ShouldReturnTopFive()
        {
            using var context = new AppDbContext(Options);
            context.Database.EnsureCreated();
            SeedDepartments(context);

            var repo = new DepartmentRepository(context);
            var result = await repo.FindByNameAsync("");

            Assert.Equal(5, result.Count); // Limit by .Take(5)
        }

        [Fact]
        public async Task FindByNameAsync_NoMatch_ShouldReturnEmpty()
        {
            using var context = new AppDbContext(Options);
            context.Database.EnsureCreated();
            SeedDepartments(context);

            var repo = new DepartmentRepository(context);
            var result = await repo.FindByNameAsync("NonExistentDept");

            Assert.Empty(result);
        }
    }
}
