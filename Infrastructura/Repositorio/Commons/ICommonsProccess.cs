using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructuraLayer.Repositorio.Commons
{
    public interface ICommonsProccess<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);

        Task<(bool IsSuccess, string Message)> AddAsync(T entry);

        Task<(bool IsSuccess, string Message)> UpdateAsync(T entry);

        Task<(bool IsSuccess, string Message)> DeleteAsync(int id);
        Task<Tareas> GetIdAsync(int id);
    }
}
