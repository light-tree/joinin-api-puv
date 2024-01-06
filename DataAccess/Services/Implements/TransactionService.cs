using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Data;
using BusinessObject.DTOs.Common;

namespace DataAccess.Services.Implements
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;

        public TransactionService(ITransactionRepository transactionRepository, IUserRepository userRepository, IGroupRepository groupRepository)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
        }

        public async Task<Transaction> CreateTransaction(User user, TransactionType transactionType)
        {
            try
            {
                return await _transactionRepository.CreateTransaction(user, transactionType);
               
            }
            catch
            {
                return null;
            }

        }

        public async Task<CommonResponse> FillterTransaction(Guid? id,
                                                                            string? code,
                                                                            TransactionStatus? transactionStatus,
                                                                            Guid? userId,
                                                                            DateTime? startDate,
                                                                            DateTime? endDate,
                                                                            int pageNumber, 
                                                                            int pageSize,
                                                                            Expression<Func<Transaction, object>> orderByExpression)
        {
            try
            {
                return await  _transactionRepository.FilterTransaction(id,code,transactionStatus,userId,startDate,endDate,pageNumber,pageSize,orderByExpression);
            }
            catch
            {
                return null;
            }

        }

        public async Task<Transaction> UpdateTransactionStatus(Guid id, TransactionStatus status)
        {
            try
            {
                Transaction rs = _transactionRepository.GetTransactionById(id);
               
              
               
                if (rs != null)
                {
                    // nếu trạng thái như cũ thì giữ nguyên
                    if (status == rs.Status)
                    {
                        return rs;
                    }
                   
                    

                    User user = await _userRepository.FindAccountByGUID(rs.UserId);
                  

                    switch (status)
                    {

                        case TransactionStatus.SUCCESS:
                           
                            if (user.EndDatePremium == null || user.EndDatePremium.Value <= DateTime.Now) {
                                user.EndDatePremium = DateTime.Now.AddDays(30);

                            } else if  (user.EndDatePremium != null && user.EndDatePremium.Value >= DateTime.Now)
                            {
                                user.EndDatePremium = user.EndDatePremium.Value.AddDays(30);
                            }
                            _groupRepository.UpdateGroupSizeAsPremiumByCreaterId(rs.UserId);
                            rs.TransactionDate = DateTime.Now;
                            
                            break;
                        case TransactionStatus.WAITING:
                            if (user.EndDatePremium == null || user.EndDatePremium.Value <= DateTime.Now)
                            {
                                user.EndDatePremium = DateTime.Now.AddDays(-30);

                            }
                            else if (user.EndDatePremium != null && user.EndDatePremium.Value >= DateTime.Now)
                            {
                                user.EndDatePremium = user.EndDatePremium.Value.AddDays(-30);
                            }

                            rs.TransactionDate = null;
                            break;
                       
                        default:
                            break;

                    }


                    rs.Status = status;
                  
                   await _transactionRepository.UpdateTransaction(rs);
                    rs.User = user;
                    return rs;
                }
                return null;
               
            } catch
            {
                return null;
            }

        }


        public async Task<Transaction> GetTransactionByCode(string code)
        {
            return  _transactionRepository.GetTransactionByCode(code);
        }

        public async Task<int> CountTransaction()
        {
            return await _transactionRepository.CountTransaction();
        }
     
    }
}
