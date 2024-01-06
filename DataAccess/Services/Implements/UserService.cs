using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.DTOs.User;
using BusinessObject.Models;
using DataAccess.Repositories;
using DataAccess.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public User AddUser(User user)
        {
            user.Password = PasswordHasher.Hash(user.Password);
            return userRepository.AddUser(user);

        }
        public async Task<bool> UpdateUser(User user)
        {
            return await userRepository.UpdateUserProfile(user);

        }


        public async Task<bool> CheckDuplicatedEmail(string email)
        {
            return await userRepository.CheckDuplicatedEmail(email);
        }

        public async Task<User> FindUserByGuid(Guid id)
        {
            return await userRepository.FindAccountByGUID(id);
        }

        public User FindUserById(Guid id)
        {
            return  userRepository.FindUserByGUID(id);
        }

        public async Task<User> FindUserByEmail(string email)
        {
            return await userRepository.FindAccountByEmail(email);
        }

        public async Task<User> FindUserByToken(string token)
        {
           return await userRepository.FindAccountByToken(token);
        }

        public async Task<List<UserProfileResponseDTO>> findUserByEmail(string email, int pageSize, int pageNumber)
        {
            var rs =  userRepository.FilterUser(email, "", pageSize, pageNumber);
            List<UserProfileResponseDTO> listResponse = new List<UserProfileResponseDTO>();
            if (rs == null)
            {
                return null;

            } else
            {

                foreach (User user in rs.Result)
                {

                    UserProfileResponseDTO userProfileResponse = new UserProfileResponseDTO();
                    userProfileResponse.FullName = user.FullName;
                    userProfileResponse.Avatar = user.Avatar;
                    userProfileResponse.BirthDay = user.BirthDay;
                    userProfileResponse.Gender = user.Gender;
                    userProfileResponse.Email = user.Email;
                    userProfileResponse.Description = user.Description;
                    userProfileResponse.Skill = user.Skill;
                    userProfileResponse.Id = user.Id;
                    
                     userProfileResponse.majors = user.UserMajors.Select(u => new Major { Id = u.Major.Id, Name = u.Major.Name ,  ShortName = u.Major.ShortName}).ToList();
                       
                  
                    listResponse.Add(userProfileResponse);
                }
            }
            return listResponse;
        }

        public DashBoardDTO GetDashBoardInformation()
        {
            return userRepository.GetDashBoardInformation();
        }

        public async Task<CommonResponse> FilterUserForAdmin(string? email,string? name, int pageSize, int pageNumber)
        {
            CommonResponse commonResponse =  await userRepository.FilterUserForAdmin(email, name, pageSize, pageNumber);
            List<UserProfileResponseDTO> listResponse = new List<UserProfileResponseDTO>();
            if (commonResponse.Data == null)
            {
               
                commonResponse.Status = 200;
                commonResponse.Message = "User is empty.";

            }
            else
            {

                foreach (User user in (List<User>)commonResponse.Data)
                {

                    UserProfileResponseDTO userProfileResponse = new UserProfileResponseDTO();
                    userProfileResponse.FullName = user.FullName;
                    userProfileResponse.Avatar = user.Avatar;
                    userProfileResponse.BirthDay = user.BirthDay;
                    userProfileResponse.Gender = user.Gender;
                    userProfileResponse.Email = user.Email;
                    userProfileResponse.Description = user.Description;
                    userProfileResponse.Skill = user.Skill;
                    userProfileResponse.Id = user.Id;
                    userProfileResponse.Status = user.Status;

                    userProfileResponse.majors = user.UserMajors.Select(u => new Major { Id = u.Major.Id, Name = u.Major.Name, ShortName = u.Major.ShortName }).ToList();


                    listResponse.Add(userProfileResponse);
                }
            }
            commonResponse.Data = listResponse.OrderBy(p => p.FullName);
            commonResponse.Status = 200;
            commonResponse.Message = "Search successfully.";
            return commonResponse;
        }
    }


}
