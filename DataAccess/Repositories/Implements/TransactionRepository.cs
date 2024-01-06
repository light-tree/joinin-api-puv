using BusinessObject.Data;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace DataAccess.Repositories.Implements
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly Context _context;
        public TransactionRepository(Context context)
        {
            this._context = context;
        }

        public async Task<Transaction> CreateTransaction(User user,TransactionType transactionType)
        {
            Transaction transaction = new Transaction();
            transaction.User = user;
            transaction.CreatedDate = DateTime.Now;
            transaction.Status = TransactionStatus.WAITING;
            transaction.TransactionCode = GenerateString();
          
            try
            {
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
              
                return transaction;
            } catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<Transaction> UpdateTransaction(Transaction transaction)
        {
            try
            {
                _context.Entry(transaction).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();
                return transaction;
            } catch(Exception ex)
            {
                return null;
            }
        }

      

        private string GenerateString()
        {
            // Generate your string using any logic you desire
            // For simplicity, let's generate a random string
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<CommonResponse> FilterTransaction(Guid? id,
                                                        string? code,
                                                        TransactionStatus? transactionStatus,
                                                        Guid? userId,
                                                        DateTime? startDate,
                                                        DateTime? endDate,
                                                        int pageNumber=1,
                                                        int pageSize=10,
                                                        Expression<Func<Transaction, object>> orderByExpression = null)
        {
            IQueryable<Transaction> transactions = _context.Transactions
                                                  .Include(t => t.User)
            .Select(t => new Transaction
             {
                 Id = t.Id,
                 CreatedDate = t.CreatedDate,
                 Status = t.Status,
                 TransactionCode = t.TransactionCode,
                 Type = t.Type,
                 TransactionDate= t.TransactionDate,
                 // Chọn các thuộc tính cần lấy từ User
                 User = new User
                 {
                     Id = t.User.Id,
                     Email = t.User.Email,
                     FullName = t.User.FullName
                 }
             });
            Pagination pagination = new Pagination();
            pagination.PageSize = pageSize;
            pagination.CurrentPage = pageNumber;
           


            if (orderByExpression != null)
            {
                transactions = transactions.OrderByDescending(orderByExpression);
            }
            if (!string.IsNullOrEmpty(code))
            {
                transactions = transactions.Where(t => t.TransactionCode != null && t.TransactionCode.Contains(code) );
            }

            if (transactionStatus != null)
            {
                transactions = transactions.Where(t => t.Status == transactionStatus);
            }

            if (id != null)
            {
                transactions = transactions.Where(t => t.Id == id);
            }

            if (userId != null)
            {
                transactions = transactions.Where(t => t.UserId == userId);
            }

            if (startDate != null && endDate != null)
            {
                transactions = transactions.Where(t => t.CreatedDate >= startDate && t.CreatedDate <= endDate);
            }
            else if (startDate != null)
            {
                DateTime now = DateTime.Now;
                transactions = transactions.Where(t => t.CreatedDate <= now);
            }
            else if (endDate != null)
            {
                DateTime now = DateTime.Now;
                transactions = transactions.Where(t => t.CreatedDate >= now);
            }

            pagination.Total = transactions.Count();
            CommonResponse commonResponse = new CommonResponse();
            var rs = transactions.Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize).ToList();
            commonResponse.Data = rs;
            commonResponse.Pagination = pagination;

            if(rs.Count == 0)
            {
                commonResponse.Message = "Transation not found";
            }
            return  commonResponse;


        }


        public Transaction GetTransactionById(Guid id)
        {
            try
            {

                return _context.Transactions.FirstOrDefault(t => t.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public Transaction GetTransactionByCode(string code)
        {
            try
            {

                return _context.Transactions.FirstOrDefault(t => t.TransactionCode == code);
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> CountTransaction()
        {
            return _context.Transactions.Count();
        }

    }
}
