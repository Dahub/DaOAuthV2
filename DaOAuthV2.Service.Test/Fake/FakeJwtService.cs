using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;

namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeJwtService : IJwtService
    {
        private JwtTokenDto _token;

        public FakeJwtService(JwtTokenDto token)
        {
            _token = token;
        }

        public JwtTokenDto ExtractToken(ExtractTokenDto tokenInfo)
        {
            return _token;
        }

        public JwtTokenDto GenerateToken(CreateTokenDto value)
        {
            return _token;
        }
    }
}
