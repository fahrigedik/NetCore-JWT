using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MiniApp2.API.Services;

public static class SignInService
{
    public static SecurityKey GetSymmetricSecurityKey(string securityKey)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
    }
}

