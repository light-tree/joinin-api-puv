using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.DTOs.User;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IUserService
    {
        public User AddUser(User user);

        public Task<bool> CheckDuplicatedEmail(string email);

        public Task<bool> UpdateUser(User user);

        public Task<User> FindUserByGuid(Guid guid);

        public Task<User> FindUserByEmail(string email);

        public Task<User> FindUserByToken(string token);

        Task<List<UserProfileResponseDTO>> findUserByEmail(string email, int pageSize, int pageNumber);

        DashBoardDTO GetDashBoardInformation();

        User FindUserById(Guid id);
        Task<CommonResponse> FilterUserForAdmin(string? email, string? name, int pageSize, int pageNumber);
    }
}
