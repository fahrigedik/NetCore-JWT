using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos;

namespace AuthServer.Service.Services;

public class AuthenticationService : IAuthenticationService
{

    private readonly List<Client> _clients;
    private readonly ITokenService _tokenService;
    private readonly UserManager<UserApp> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<UserRefreshToken> _userRefreshRepository;


    public AuthenticationService(IOptions<List<Client>> optionsClient,
        ITokenService tokenService,
        UserManager<UserApp> userManager,
        IUnitOfWork unitOfWork,
        IGenericRepository<UserRefreshToken> userRefreshRepository)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _userRefreshRepository = userRefreshRepository;
        _unitOfWork = unitOfWork;
        _clients = optionsClient.Value;
    }

    public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
    {
        if (loginDto is null)
        {
            throw new ArgumentNullException(nameof(loginDto));
        }
        
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user is null)
        {
            return Response<TokenDto>.Fail("Email or Password is wrong!", 404, true);
        }

        var isPasswordCheck = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!isPasswordCheck)
        {
            return Response<TokenDto>.Fail("Email or Password is wrong!", 404, true);
        }

        var token = _tokenService.CreateToken(user);

        var userRefreshToken = _userRefreshRepository.Where(x => x.UserId == user.Id).SingleOrDefault();

        if (userRefreshToken is null)
        {
            await _userRefreshRepository.AddAsync(new UserRefreshToken()
                { UserId = user.Id, Code = token.RefreshToken, Expiration = token.RefreshTokenExpiration });
        }
        else
        {
            userRefreshToken.Code = token.RefreshToken;
            userRefreshToken.Expiration = token.RefreshTokenExpiration;
        }
        await _unitOfWork.SaveChangesAsync();


        return Response<TokenDto>.Success(token, 200);
    }

    public async Task<Response<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken)
    {
        var existRefreshToken = await _userRefreshRepository.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();

        if (existRefreshToken is null)
        {
            return Response<TokenDto>.Fail("refresh token not found", 404, true);
        }

        var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);

        if (user is null)
        {
            return Response<TokenDto>.Fail("user id not found", 404, true);
        }

        var tokenDto = _tokenService.CreateToken(user);

        existRefreshToken.Code = tokenDto.RefreshToken;
        existRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;

        await _unitOfWork.SaveChangesAsync();

        return Response<TokenDto>.Success(tokenDto, 200);
    }

    public async Task<Response<NoDataDto>> RevokeRefreshTokenAsync(string refreshToken)
    {
        var existRefreshToken = await _userRefreshRepository.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();

        if (existRefreshToken is null)
        {
            return Response<NoDataDto>.Fail("refresh token is not found", 404, true);
        }

        _userRefreshRepository.Remove(existRefreshToken);
        await _unitOfWork.SaveChangesAsync();

        return Response<NoDataDto>.Success(200);
    }

    public async Task<Response<ClientTokenDto>> CreateTokenByClient(ClientLoginDto clientLoginDto)
    {
        var client = _clients.SingleOrDefault(x =>
            x.ClientId == clientLoginDto.ClientId && x.Secret == clientLoginDto.ClientSecret);

        if (clientLoginDto is null)
        {
            return Response<ClientTokenDto>.Fail("ClientID or Secret not found", 404, true);
        }

        var token = _tokenService.CreateTokenByClient(client);

        return Response<ClientTokenDto>.Success(token, 200);
    }
}
