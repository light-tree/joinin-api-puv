using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly Context _context;
        public UserRepository(Context context)
        {
            this._context = context;
        }

        public User AddUser(User user)
        {
            try
            {
                //User user = new User();
                //user.Email = email;
                //user.FullName = fullName;
                //user.Password= password;
                //user.Phone = phone;
                //user.BirthDay = dob;
                //user.Gender = gender;
                //user.Description = description;
                //user.OtherContact = otherContact;
                //user.Skill= skill;
                //user.Avatar = avatarLink;
                //user.Theme= themeLink;
                //user.Status= userStatus;
                //user.IsAdmin = false;
                _context.Users.Add(user);
                _context.SaveChanges();
                return user;
            }
            catch (Exception e)
            {

                return null;
            }
        }


        public async Task<bool> CheckDuplicatedEmail(string email)
        {
            bool check = await _context.Users.AnyAsync(u => u.Email == email);
            return check;
        }

        public async Task<User> FindAccountByEmail(string email)
        {
            try
            {
                return  _context.Users.FirstOrDefault(u => u.Email == email);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<User> FindAccountByGUID(Guid id)
        {
            try
            {
                return await _context.Users.Include(u => u.UserMajors).ThenInclude(u => u.Major).FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public User FindUserByGUID(Guid id)
        {
            try
            {
                return _context.Users.Include(u => u.UserMajors).ThenInclude(u => u.Major).FirstOrDefault(u => u.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<User> FindAccountByToken(string token)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(p => p.Token == token);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UserType GetUserTypeById(Guid userId)
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw new Exception($"User with Id: {userId} does not exist");

            if (user.EndDatePremium != null && user.EndDatePremium.Value > DateTime.Now)
                return UserType.PREMIUM;
            else
                return UserType.FREEMIUM;
        }

        public async Task<bool> UpdateUserProfile(User user)
        {
            try
            {
                
                _context.Entry(user).State = EntityState.Modified;
               await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public async Task<List<User>> FilterUser(string email, string name, int pageSize =1, int pageNumber = 10)
        {
            try
            {
                IQueryable<User> Users = _context.Users.Where(u =>u.IsAdmin == false).Include(u => u.UserMajors).ThenInclude(u => u.Major);

                if (!String.IsNullOrEmpty(email))
                {
                    Users = Users.Where(u => u.Email.Contains(email.ToLower()));
                }
                if (!String.IsNullOrEmpty(name))
                {
                    Users = Users.Where(u => u.FullName.Contains(name));
                }
                var x=  Users.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize).ToList();
                return x;
            } catch
            {
                return null;
            }
        }

        public DashBoardDTO GetDashBoardInformation()
        {
            DashBoardDTO dashBoardDTO = new DashBoardDTO();
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime endOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + 1);

            dashBoardDTO.TotalRevenue = _context.Transactions.Where(t => t.Status == TransactionStatus.SUCCESS).Count() * BusinessRuleData.PREMIUM_FEE_PER_MONTH;
            dashBoardDTO.TotalPremiumUser = _context.Users.Where(u => u.Status == UserStatus.ACTIVE && u.EndDatePremium != null && u.EndDatePremium >= DateTime.Now).Count();
            dashBoardDTO.TotalUser = _context.Users.Where(u => u.Status == UserStatus.ACTIVE).Count();
            dashBoardDTO.TotalFreemiumUser = dashBoardDTO.TotalUser - dashBoardDTO.TotalPremiumUser;
            dashBoardDTO.TotalUserGrownPercentLastWeek = 
                ((((float)_context.Users.Where(u => u.Status == UserStatus.ACTIVE && u.IsAdmin == false).Count()) / 
                ((float)_context.Users.Where(u => u.Status == UserStatus.ACTIVE && u.CreatedDate < endOfWeek && u.IsAdmin == false).Count()))
                - 1) * 100;
            dashBoardDTO.FreeUserCount = new List<int>();
            dashBoardDTO.PreUserCount = new List<int>();
            dashBoardDTO.ActiveUserCount = new List<int>();
            int weekCount = 1;
            int preUserCount = 0;
            List <Transaction> transactions = new List<Transaction>();
            DateTime startDate = new DateTime(2023, 6, 12);
            do
            {
                DateTime endDate = startDate.AddDays(7);
                transactions = _context.Transactions.Where(t => t.Status == TransactionStatus.SUCCESS && t.TransactionDate >= startDate).ToList();

                preUserCount = _context.Users
                            .Where(u =>
                                u.Status == UserStatus.ACTIVE &&
                                u.CreatedDate >= startDate &&
                                u.CreatedDate < endDate &&
                                u.EndDatePremium != null
                        ).Select(u => transactions.Any(t => t.UserId == u.Id)).Count();
                dashBoardDTO.PreUserCount.Add(preUserCount);

                dashBoardDTO.FreeUserCount.Add(_context.Users
                        .Where(u =>
                            u.Status == UserStatus.ACTIVE &&
                            u.CreatedDate >= startDate &&
                            u.CreatedDate < endDate
                        ).Count() - preUserCount);

                ActiveUserCount? activeUserCount = _context.ActiveUserCounts
                    .FirstOrDefault(auc => auc.StartDate == startDate);
                dashBoardDTO.ActiveUserCount.Add(activeUserCount != null ? activeUserCount.UserCount : 0);

                weekCount++;
                startDate = endDate;
            }
            while (weekCount <= 7);

            PlatformUserCount? facebookUserCount = _context.PlatformUserCounts.FirstOrDefault(puc => puc.PlatformName == "Facebook");
            dashBoardDTO.FacebookUser = facebookUserCount == null ? 0 : facebookUserCount.UserCount;

            PlatformUserCount? tiktokUserCount = _context.PlatformUserCounts.FirstOrDefault(puc => puc.PlatformName == "Tiktok");
            dashBoardDTO.TiktokUser = tiktokUserCount == null ? 0 : tiktokUserCount.UserCount;

            PlatformUserCount? unknownUserCount = _context.PlatformUserCounts.FirstOrDefault(puc => puc.PlatformName == "Unknown");
            dashBoardDTO.UnknownUser = unknownUserCount == null ? 0 : unknownUserCount.UserCount;

            dashBoardDTO.GroupCount = new List<int> 
            { 
                _context.Groups.Where(g => g.MemberCount >= 1 && g.MemberCount <= 2).Count(),
                _context.Groups.Where(g => g.MemberCount >= 3 && g.MemberCount <= 6).Count(),
                _context.Groups.Where(g => g.MemberCount >= 7 && g.MemberCount <= 10).Count(),
                _context.Groups.Where(g => g.MemberCount >= 11 && g.MemberCount <= 20).Count()
            };

            return dashBoardDTO;
        }

        public Task<int> UpdateLastLoginDate(User account)
        {
            DateTime? lastLoginDate = account.LastLoginDate;
            DateTime currentLoginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            int indexOfCurrentWeek = -(int)currentLoginDate.AddDays(-1).DayOfWeek;
            DateTime startDateOfCurrentWeek = currentLoginDate.AddDays(indexOfCurrentWeek);
            bool isNextWeek = lastLoginDate == null || lastLoginDate.Value < startDateOfCurrentWeek;
            if (isNextWeek)
            {
                ActiveUserCount? activeUserCount = _context.ActiveUserCounts.FirstOrDefault(auc => auc.StartDate == startDateOfCurrentWeek);
                if (activeUserCount == null)
                {
                    _context.ActiveUserCounts.Add(new ActiveUserCount
                    {
                        StartDate = startDateOfCurrentWeek,
                        EndDate = startDateOfCurrentWeek.AddDays(7),
                        UserCount = 1
                    });
                    _context.SaveChanges();
                }
                else
                {
                    activeUserCount.UserCount += 1;
                    _context.ActiveUserCounts.Update(activeUserCount);
                    _context.SaveChanges();
                }
            }
            account.LastLoginDate = DateTime.Now;
            _context.Update(account);
            return _context.SaveChangesAsync();
        }



        public async Task<CommonResponse> FilterUserForAdmin(string email, string name, int pageSize = 1, int pageNumber = 10)
        {
            try
            {
                IQueryable<User> Users = _context.Users.Where(u => u.IsAdmin == false).Include(u => u.UserMajors).ThenInclude(u => u.Major);
                Pagination pagination = new Pagination();
                pagination.PageSize = pageSize;
                pagination.CurrentPage = pageNumber;
             
                if (!String.IsNullOrEmpty(email))
                {
                    Users = Users.Where(u => u.Email.Contains(email.ToLower()));
                }
                if (!String.IsNullOrEmpty(name))
                {
                    Users = Users.Where(u => u.FullName.Contains(name));
                }
                pagination.Total = Users.Count();
                var x = Users.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize).ToList();
                CommonResponse commonResponse = new CommonResponse();
                commonResponse.Data = x;
                commonResponse.Pagination = pagination;
                return commonResponse;
            }
            catch
            {
                return null;
            }
        }
    }



}
