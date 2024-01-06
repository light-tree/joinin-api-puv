using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class CommentRepository : ICommentRepository
    {
        private readonly Context _context;

        public CommentRepository(Context context)
        {
            _context = context;
        }

        public int DeleteByTaskId(Guid taskId)
        {
            List<Comment> deletedComments = _context.Comments.Where(c => c.TaskId == taskId).ToList();
            _context.Comments.RemoveRange(deletedComments);
            return _context.SaveChanges();
        }

        public Guid CreateComment(CommentDTOForCreating comment, Guid memberID)
        {
            Comment commentCreate = new Comment()
            {
                Content = comment.Content,
                CreatedDate = DateTime.Now,
                Status = CommentStatus.ACTIVE,
                TaskId = comment.TaskId,
                MemberId = memberID,
            };
            _context.Comments.Add(commentCreate);
            _context.SaveChanges();
            return commentCreate.Id;
        }

        public IEnumerable<Comment> GetCommentsByTaskID(Guid taskID)
        {
            return _context.Comments.Where(c => c.TaskId == taskID).OrderBy(c => c.CreatedDate).ToList();
        }

        public int DeleteComment(Guid id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            _context.Comments.Remove(comment);
            return _context.SaveChanges();
        }

        public Comment FindComment(Guid commentID, Guid memberID)
        {
            return _context.Comments.Where(c => c.Id == commentID && c.MemberId == memberID).FirstOrDefault();
        }

        public int DeleteByGroupId(Guid groupId)
        {
            List<Comment> comments = _context.Comments
                .ToList();

            List<Comment> deletedComments = new List<Comment>();
            foreach (Comment comment in comments)
            {
                if(_context.Tasks.FirstOrDefault(t => t.Id == comment.TaskId && t.GroupId == groupId) != null)
                {
                    deletedComments.Add(comment);
                }
            }
            _context.Comments.RemoveRange(deletedComments);
            return _context.SaveChanges();
        }
    }
}
