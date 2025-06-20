using ExternalUserServiceLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalUserServiceLibrary.Clients
{
    public interface IReqResClient
    {
        Task<UserResponse> GetAllUsersAsync(int page);
        Task<User> GetUserByIdAsync(int userId);
    }
}
