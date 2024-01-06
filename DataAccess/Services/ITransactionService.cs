using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess.Services
{
    public interface ITransactionService
    {
        Task<int> CountTransaction();
        Task<Transaction> CreateTransaction(User user, TransactionType transactionType);
  
        Task<CommonResponse> FillterTransaction(Guid? id, string? code, TransactionStatus? transactionStatus, Guid? userId, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize, Expression<Func<Transaction, object>> orderByExpression);
        Task<Transaction> GetTransactionByCode(string code);
        Task<Transaction> UpdateTransactionStatus(Guid id, TransactionStatus status);
    }
}
