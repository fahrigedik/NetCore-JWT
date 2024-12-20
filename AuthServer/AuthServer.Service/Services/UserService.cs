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
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(UserManager<UserApp> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
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

    public async Task<Response<NoDataDto>> CreateUserRoles(string email)
    {
        if ((await _roleManager.RoleExistsAsync("admin")))
        {
            await _roleManager.CreateAsync(new IdentityRole("admin"));
            await _roleManager.CreateAsync(new IdentityRole("manager"));
        }
        var user = await _userManager.FindByEmailAsync(email);
        await _userManager.AddToRoleAsync(user, "admin");
        await _userManager.AddToRoleAsync(user, "manager");

        return Response<NoDataDto>.Success(200);
    }
}
