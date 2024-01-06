using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IUserRepository
    {

        public User AddUser(User user);

        public Task<bool> CheckDuplicatedEmail(string email);

        public Task<bool> UpdateUserProfile(User user);

        public Task<User> FindAccountByEmail(string email);

        public Task<User> FindAccountByGUID(Guid id);

        public Task<User> FindAccountByToken(string token);

        UserType GetUserTypeById(Guid userId);

        Task<List<User>> FilterUser(string email, string name, int pageSize = 1, int pageNumber = 10);

        User FindUserByGUID(Guid id);

        DashBoardDTO GetDashBoardInformation();

        Task<int> UpdateLastLoginDate(User account);
        Task<CommonResponse> FilterUserForAdmin(string email, string name, int pageSize = 1, int pageNumber = 10);
    }
}
