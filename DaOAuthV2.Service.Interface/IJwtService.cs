using DaOAuthV2.Service.DTO;

namespace DaOAuthV2.Service.Interface
{
    public interface IJwtService
    {
        MailJwtTokenDto GenerateMailToken(string userName);
        MailJwtTokenDto ExtractMailToken(string token);

        JwtTokenDto GenerateToken(CreateTokenDto value);
        JwtTokenDto ExtractToken(ExtractTokenDto tokenInfo);
    }
}
