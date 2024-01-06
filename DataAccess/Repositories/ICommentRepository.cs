using BusinessObject.DTOs;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface ICommentRepository
    {
        int DeleteByTaskId(Guid taskId);
        IEnumerable<Comment> GetCommentsByTaskID(Guid taskID);
        Guid CreateComment(CommentDTOForCreating comment, Guid memberID);
        int DeleteComment(Guid id);

        Comment FindComment(Guid commentID, Guid memberID);
        int DeleteByGroupId(Guid groupId);
    }
}
