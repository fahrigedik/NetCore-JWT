using System.Linq.Expressions;
using SharedLibrary.Dtos;

namespace AuthServer.Core.Services;

public interface IServiceGeneric<T, TDto> where T : class where TDto : class
{
    Task<Response<TDto>> GetByIdAsync(int id);
    Task<Response<IEnumerable<TDto>>> GetAllAsync();
    Task<Response<IEnumerable<TDto>>> Where(Expression<Func<T, bool>> predicate);
    Task<Response<TDto>> AddAsync(T entity);
    Task<Response<NoDataDto>> Remove(T entity);
    Task<Response<NoDataDto>> Update(T entity);
}

