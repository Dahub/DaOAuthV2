﻿using DaOAuthV2.ApiTools;

namespace DaOAuthV2.Service.DTO
{
    public class ClientListDto : IDto
    {
        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public string ClientType { get; set; }

        public string DefaultReturnUri { get; set; }

        public bool IsActif { get; set; }
    }
}
