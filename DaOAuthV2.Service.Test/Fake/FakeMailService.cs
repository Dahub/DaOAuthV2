﻿using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using System;
using System.Threading.Tasks;

namespace DaOAuthV2.Service.Test.Fake
{
    internal class FakeMailService : IMailService
    {
        public string SendGridApiKey => throw new NotImplementedException();

        public Task<bool> SendEmail(SendEmailDto mailInfo)
        {
            bool ReturnTrue()
            {
                return true;
            }

            return new Task<bool>(ReturnTrue); 
        }
    }
}