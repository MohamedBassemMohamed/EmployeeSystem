using EmployeeManagementAPI.Models;
using EmployeeManagementAPI.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementAPI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Employee>> GetEmployeesAsync() => _repository.GetEmployeesAsync();
        public Task<Employee> GetEmployeeByIdAsync(int id) => _repository.GetEmployeeByIdAsync(id);
        public Task AddEmployeeAsync(Employee employee) => _repository.AddEmployeeAsync(employee);
        public Task UpdateEmployeeAsync(Employee employee) => _repository.UpdateEmployeeAsync(employee);
        public Task DeleteEmployeeAsync(int id) => _repository.DeleteEmployeeAsync(id);
    }
}
