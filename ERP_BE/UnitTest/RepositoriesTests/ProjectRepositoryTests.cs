using Business.Domain.Models;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using UnitTest.Tool;

namespace UnitTest.RepositoriesTests
{
    public class ProjectRepositoryTests : TestWithSqlite<AppDbContext>
    {
        private void SeedData(AppDbContext context)
        {
            var group = new Group { Id = 1, Name = "Test Group" };
            var person = new Person
            {
                Id = 1,
                FirstName = "Nguyen",
                LastName = "Van A"
            };


            context.Groups.Add(group);
            context.People.Add(person);

            context.Projects.Add(new Project
            {
                Id = 1,
                Position = "Leader",
                Responsibilities = "Manage ERP System",
                StartDate = DateTime.UtcNow,
                OrderIndex = 1,
                IsDeleted = false,
                GroupId = 1,
                PersonId = 1
            });

            context.SaveChanges();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProjectWithGroupAndPerson()
        {
            // Arrange
            using var context = new AppDbContext(Options);
            context.Database.EnsureCreated();
            SeedData(context);

            var repo = new ProjectRepository(context);

            // Act
            var project = await repo.GetByIdAsync(1);

            // Assert
            Assert.NotNull(project);
            Assert.Equal("Leader", project.Position);

            // Kiểm tra Group
            Assert.NotNull(project.Group);
            Assert.Equal("Test Group", project.Group.Name);

            // Kiểm tra Person
            Assert.NotNull(project.Person);
            Assert.Equal("Nguyen", project.Person.FirstName);
            Assert.Equal("Van A", project.Person.LastName);
        }


        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            using var context = new AppDbContext(Options);
            context.Database.EnsureCreated();
            SeedData(context);

            var repo = new ProjectRepository(context);

            // Act
            var project = await repo.GetByIdAsync(999); // ID không tồn tại

            // Assert
            Assert.Null(project);
        }
    }
}
