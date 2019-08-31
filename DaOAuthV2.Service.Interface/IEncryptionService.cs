using System;
using System.Collections.Generic;
using System.Text;

namespace DaOAuthV2.Service.Interface
{
    public interface IEncryptionService
    {
        bool AreEqualsSha256(string toCompare, byte[] hash);

        byte[] Sha256Hash(string toHash);
    }
}
