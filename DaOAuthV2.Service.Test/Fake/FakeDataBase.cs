using DaOAuthV2.Constants;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DaOAuthV2.Service.Test.Fake
{
    internal class FakeDataBase
    {
        private static FakeDataBase _instance;

        internal ClientType PublicClientType { get; private set; }

        internal ClientType ConfidentialClientType { get; private set; }

        internal static FakeDataBase Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FakeDataBase();
                }

                return _instance;
            }
        }

        internal static void Reset()
        {
            _instance = null;
        }

        private FakeDataBase()
        {
            PublicClientType = new ClientType()
            {
                Id = 1,
                Wording = ClientTypeName.Public
            };

            ConfidentialClientType = new ClientType()
            {
                Id = 2,
                Wording = ClientTypeName.Confidential
            };

            ClientTypes.Add(PublicClientType);
            ClientTypes.Add(ConfidentialClientType);
        }

        internal IList<RessourceServer> RessourceServers = new List<RessourceServer>();

        internal IList<Code> Codes = new List<Code>();

        internal IList<Role> Roles = new List<Role>();

        internal IList<UserRole> UsersRoles = new List<UserRole>();

        internal IList<ClientType> ClientTypes = new List<ClientType>();

        internal IList<Client> Clients = new List<Client>();

        internal IList<ClientReturnUrl> ClientReturnUrls = new List<ClientReturnUrl>();

        internal IList<User> Users = new List<User>();

        internal IList<UserClient> UsersClient = new List<UserClient>();

        internal IList<Scope> Scopes = new List<Scope>();

        internal IList<ClientScope> ClientsScopes = new List<ClientScope>();
    }
}
