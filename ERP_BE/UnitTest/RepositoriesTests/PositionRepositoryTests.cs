using Business.Domain.Models;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using UnitTest.Tool;

namespace UnitTest.RepositoriesTests
{
    public class PositionRepositoryTests : TestWithSqlite<AppDbContext>
    {
        private void SeedPositions(AppDbContext context)
        {
            context.Positions.AddRange(
                new Position { Id = 1, Name = "Developer", IsDeleted = false },
                new Position { Id = 2, Name = "DevOps", IsDeleted = false },
                new Position { Id = 3, Name = "Tester", IsDeleted = false },
                new Position { Id = 4, Name = "PM", IsDeleted = false },
                new Position { Id = 5, Name = "Business Analyst", IsDeleted = false },
                new Position { Id = 6, Name = "Scrum Master", IsDeleted = false }
            );
            context.SaveChanges();
        }

        [Fact]
        public async Task FindByNameAsync_Contains_ShouldReturnMatches()
        {
            using var context = new AppDbContext(Options);
            context.Database.EnsureCreated();
            SeedPositions(context);

            var repo = new PositionRepository(context);
            var result = await repo.FindByNameAsync("Dev");

            Assert.Equal(2, result.Count); // "Developer", "DevOps"
            Assert.All(result, p => Assert.Contains("Dev", p.Name));
        }

        [Fact]
        public async Task FindByNameAsync_Equals_ShouldReturnExactMatch()
        {
            using var context = new AppDbContext(Options);
            context.Database.EnsureCreated();
            SeedPositions(context);

            var repo = new PositionRepository(context);
            var result = await repo.FindByNameAsync("PM", absolute: true);

            Assert.Single(result);
            Assert.Equal("PM", result[0].Name);
        }

        [Fact]
        public async Task FindByNameAsync_EmptyFilter_ShouldReturnTopFive()
        {
            using var context = new AppDbContext(Options);
            context.Database.EnsureCreated();
            SeedPositions(context);

            var repo = new PositionRepository(context);
            var result = await repo.FindByNameAsync("");

            Assert.Equal(5, result.Count); // Lấy tối đa 5
        }

        [Fact]
        public async Task FindByNameAsync_NoMatch_ShouldReturnEmpty()
        {
            using var context = new AppDbContext(Options);
            context.Database.EnsureCreated();
            SeedPositions(context);

            var repo = new PositionRepository(context);
            var result = await repo.FindByNameAsync("NonExistent");

            Assert.Empty(result);
        }
    }
}
