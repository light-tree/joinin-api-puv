using BusinessObject.DTOs;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface ICommentService
    {
        public IEnumerable<Comment> GetComments(Guid taskID);
        public Guid CreateComment(CommentDTOForCreating comment, Guid userID);

        public int DeleteComment(Guid commentID, Guid memberID);
    }
}
