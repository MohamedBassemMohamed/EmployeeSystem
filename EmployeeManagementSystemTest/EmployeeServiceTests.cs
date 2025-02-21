using EmployeeManagementAPI.Models;
using EmployeeManagementAPI.Repositories;
using EmployeeManagementAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementAPI.Tests
{
    public class EmployeeServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EmployeeManagement;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;
            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
            var repository = new EmployeeRepository(_context);
            _service = new EmployeeService(repository);
            ResetDatabase().GetAwaiter().GetResult();
        }

        private async Task SeedDatabase()
        {
            var employees = new List<Employee>
    {
        new Employee { Name = "John Doe", Position = "Developer", Department = "IT", Salary = 60000, JoiningDate = DateTime.Now.AddYears(-2) },
        new Employee { Name = "Jane Smith", Position = "Manager", Department = "HR", Salary = 80000, JoiningDate = DateTime.Now.AddYears(-1) }
    };
            _context.Employees.AddRange(employees);
            await _context.SaveChangesAsync();
        }

        

        [Fact]
        public async Task GetEmployeesAsync_ReturnsAllEmployees()
        {
            // Act
            var result = await _service.GetEmployeesAsync();

            // Assert
            var employees = result.ToList();
            Assert.Equal(2, employees.Count);
            Assert.Contains(employees, e => e.Name == "John Doe");
            Assert.Contains(employees, e => e.Name == "Jane Smith");
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ReturnsEmployee_WhenIdExists()
        {
            // Arrange
            await ResetDatabase(); 
            var seededEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.Name == "John Doe");
            Assert.NotNull(seededEmployee);

            // Act
            var result = await _service.GetEmployeeByIdAsync(seededEmployee.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John Doe", result.Name);
            Assert.Equal("Developer", result.Position);
        }
        private async Task ResetDatabase()
        {
            await _context.Employees.ExecuteDeleteAsync();
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Employees', RESEED, 0)");
            _context.ChangeTracker.Clear(); 
            await SeedDatabase();
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ReturnsNull_WhenIdDoesNotExist()
        {
            // Act
            var result = await _service.GetEmployeeByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddEmployeeAsync_AddsEmployeeToDatabase()
        {
            // Arrange
            var newEmployee = new Employee
            {
                
                Name = "Alice Brown",
                Position = "Analyst",
                Department = "Finance",
                Salary = 55000,
                JoiningDate = DateTime.Now
            };

            // Act
            await _service.AddEmployeeAsync(newEmployee);

            // Assert
            var addedEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.Name == "Alice Brown");
            Assert.NotNull(addedEmployee);
            Assert.Equal("Alice Brown", addedEmployee.Name);
            Assert.Equal("Analyst", addedEmployee.Position);
            Assert.Equal(3, _context.Employees.Count()); 
        }

        [Fact]
        public async Task UpdateEmployeeAsync_UpdatesEmployeeInDatabase()
        {
            // Arrange
            await ResetDatabase(); 
            var employeeToUpdate = await _context.Employees.FirstOrDefaultAsync(e => e.Name == "John Doe");
            Assert.NotNull(employeeToUpdate);

            employeeToUpdate.Name = "John Doe Updated";
            employeeToUpdate.Position = "Senior Developer";
            employeeToUpdate.Salary = 70000;

            // Act
            await _service.UpdateEmployeeAsync(employeeToUpdate);
            var result = await _context.Employees.FindAsync(employeeToUpdate.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John Doe Updated", result.Name);
            Assert.Equal("Senior Developer", result.Position);
            Assert.Equal(70000, result.Salary);
        }

            [Fact]
        public async Task DeleteEmployeeAsync_RemovesEmployeeFromDatabase()
        {
            // Arrange
            var initialCount = _context.Employees.Count();
            Assert.Equal(2, initialCount); 
            var employeeToDelete = await _context.Employees.FirstOrDefaultAsync(e => e.Name == "John Doe");
            Assert.NotNull(employeeToDelete); 

            // Act
            await _service.DeleteEmployeeAsync(employeeToDelete.Id);

            // Assert
            var deletedEmployee = await _context.Employees.FindAsync(employeeToDelete.Id);
            Assert.Null(deletedEmployee);
            Assert.Equal(1, _context.Employees.Count());
        }

        public void Dispose()
        {
            _context.Dispose(); 
        }
    }
}