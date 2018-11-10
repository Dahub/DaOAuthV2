﻿using DaOAuthV2.Service.DTO;

namespace DaOAuthV2.Service.Interface
{
    public interface IUserService
    {
        int CreateUser(CreateUserDto toCreate);
        void UpdateUser(UpdateUserDto toUpdate);
        void DeleteUser(string userName);
        UserDto GetUser(string login, string password);
    }
}
