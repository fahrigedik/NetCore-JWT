namespace AuthServer.Core.Dtos;

public class ClientLoginDto
{
    public string ClientId { get; set; } //email
    public string ClientSecret { get; set; } //password
}

