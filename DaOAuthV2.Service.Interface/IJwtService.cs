using DaOAuthV2.Service.DTO;

namespace DaOAuthV2.Service.Interface
{
    public interface IJwtService
    {
        ChangeMailJwtTokenDto GenerateMailToken();
        ChangeMailJwtTokenDto ExtractMailToken(string token);

        JwtTokenDto GenerateToken(CreateTokenDto value);
        JwtTokenDto ExtractToken(ExtractTokenDto tokenInfo);
    }
}
