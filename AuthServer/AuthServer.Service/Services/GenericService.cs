using System.Linq.Expressions;
using System.Net;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;

namespace AuthServer.Service.Services;

public class GenericService<T, TDto> : IServiceGeneric<T,TDto> where TDto : class where T : class
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<T> _genericRepository;

    public GenericService(IUnitOfWork unitOfWork, IGenericRepository<T> genericRepository )
    {
        _unitOfWork = unitOfWork;
        _genericRepository = genericRepository;
    }

    public async Task<Response<TDto>> GetByIdAsync(int id)
    {
        var entity = await _genericRepository.GetByIdAsync(id);
        var entityDto = ObjectMapper.Mapper.Map<TDto>(entity);


        return Response<TDto>.Success(entityDto,200);
    }

    public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
    {
        var entities = await _genericRepository.GetAllAsync();

        if (entities is null)
        {
            return Response<IEnumerable<TDto>>.Fail("Entities not found", 404, true);
        }

        var entityDtos = ObjectMapper.Mapper.Map<List<TDto>>(entities);
        return Response<IEnumerable<TDto>>.Success(entityDtos,200);
    }

    public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<T, bool>> predicate)
    {
        var entities = _genericRepository.Where(predicate).AsEnumerable();

        if (entities is null)
        {
            return Response<IEnumerable<TDto>>.Fail("entities not found",404,true);
        }

        var entityDtos = ObjectMapper.Mapper.Map<List<TDto>>(entities);
        return Response<IEnumerable<TDto>>.Success(entityDtos, 200);
    }

    public async Task<Response<TDto>> AddAsync(TDto entity)
    {
        var newEntity = ObjectMapper.Mapper.Map<T>(entity);
        await _genericRepository.AddAsync(newEntity);
        await _unitOfWork.SaveChangesAsync();

        var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);
        
        return Response<TDto>.Success(newDto, 200);
    }

    public async Task<Response<NoDataDto>> Remove(int id)
    {
        var isExistEntity = await _genericRepository.GetByIdAsync(id);

        if (isExistEntity is null)
        {
            return Response<NoDataDto>.Fail("Entity is not found", 404, true);
        }

        _genericRepository.Remove(isExistEntity);
        await _unitOfWork.SaveChangesAsync();

        return Response<NoDataDto>.Success(200);
    }

    public async Task<Response<NoDataDto>> Update(TDto entity, int id)
    {
        var isExistEntity = await _genericRepository.GetByIdAsync(id);

        if (isExistEntity is null)
        {
            return Response<NoDataDto>.Fail("entity not found", 404, true);
        }

        var updateEntity = ObjectMapper.Mapper.Map<T>(entity);
        _genericRepository.Update(updateEntity);
        await _unitOfWork.SaveChangesAsync();

        return Response<NoDataDto>.Success(200);

    }
}

