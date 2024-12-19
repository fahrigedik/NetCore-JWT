using System.Security.Cryptography.X509Certificates;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dtos;

namespace AuthServer.Service.Services;

public class UserService : IUserService
{

    private readonly UserManager<UserApp> _userManager;

    public UserService(UserManager<UserApp> userManager)
    {
        _userManager = userManager;
    }
    public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = new UserApp()
        {
            Id = Guid.NewGuid().ToString(),
            Email = createUserDto.Email,
            UserName = createUserDto.UserName
        };

        var result = await _userManager.CreateAsync(user, password: createUserDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => x.Description).ToList();
            return Response<UserAppDto>.Fail(new ErrorDto(errors, true), 404);
        }

        var userDto = ObjectMapper.Mapper.Map<UserAppDto>(user);

        return Response<UserAppDto>.Success(userDto, 200);
    }

    public async Task<Response<UserAppDto>> GetUserByNameAsync(string name)
    {
        var user = await _userManager.FindByNameAsync(name);
        if (user is null)
        {
            return Response<UserAppDto>.Fail("user not found", 404, true);
        }

        var userDto = ObjectMapper.Mapper.Map<UserAppDto>(user);
        return Response<UserAppDto>.Success(userDto, 200);
    }
}
