using DaOAuthV2.Service.DTO;

namespace DaOAuthV2.Service.Interface
{
    public interface IJwtService
    {
        JwtTokenDto GenerateToken(CreateTokenDto value);
        JwtTokenDto ExtractToken(ExtractTokenDto tokenInfo);
    }
}
