using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface ITransactionRepository
    {
      
        Task<int> CountTransaction();
        Task<Transaction> CreateTransaction(User user, TransactionType transactionType);
       
        Task<CommonResponse> FilterTransaction(Guid? id, string? code, TransactionStatus? transactionStatus, Guid? userId, DateTime? startDate, DateTime? endDate, int pageNumber = 1, int pageSize = 10, Expression<Func<Transaction, object>> orderByExpression = null);
        Transaction GetTransactionByCode(string code);
        Transaction GetTransactionById(Guid id);
        Task<Transaction> UpdateTransaction(Transaction transaction);
    }
}
