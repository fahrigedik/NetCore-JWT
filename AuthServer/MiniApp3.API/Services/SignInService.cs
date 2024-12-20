using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MiniApp3.API.Services;

public static class SignInService
{
    public static SecurityKey GetSymmetricSecurityKey(string securityKey)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
    }
}

