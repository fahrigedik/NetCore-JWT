using System.Linq.Expressions;
using SharedLibrary.Dtos;

namespace AuthServer.Core.Services;

public interface IServiceGeneric<T, TDto> where T : class where TDto : class
{
    Task<Response<TDto>> GetByIdAsync(int id);
    Task<Response<IEnumerable<TDto>>> GetAllAsync();
    Task<Response<IEnumerable<TDto>>> Where(Expression<Func<T, bool>> predicate);
    Task<Response<TDto>> AddAsync(TDto entity);
    Task<Response<NoDataDto>> Remove(int id);
    Task<Response<NoDataDto>> Update(TDto entity, int id);
}

