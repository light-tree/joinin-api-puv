using BusinessObject.DTOs;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Implements
{
    public class CommentService : ICommentService
    {
        ICommentRepository _commentRepository;
        IAssignedTaskRepository _assignedTaskRepository;
        ITaskRepository _taskRepository;
        IMemberRepository _memberRepository;

        public CommentService(ICommentRepository commentRepository, IAssignedTaskRepository assignedTaskRepository, IMemberRepository memberRepository, ITaskRepository taskRepository)
        {
            this._commentRepository = commentRepository;
            this._assignedTaskRepository = assignedTaskRepository;
            this._memberRepository = memberRepository;
            this._taskRepository = taskRepository;
        }
        public Guid CreateComment(CommentDTOForCreating comment, Guid userID)
        {
            var Task = _taskRepository.FindById(comment.TaskId);
            if (Task == null)
                throw new Exception("Task doesn't exit in system");
            var member = _memberRepository.FindByUserIdAndGroupId(userID, Task.GroupId);
            var assignTask = _assignedTaskRepository.FindByTaskIdAndAssignedForId(comment.TaskId, member.Id);
            if (assignTask == null)
            {
                if(member.Role != MemberRole.SUB_LEADER && member.Role != MemberRole.LEADER)
                {
                    throw new Exception("This user does not have permission to comment on this task");
                }
            }
            return _commentRepository.CreateComment(comment, member.Id);
        }

        public Comment FindComment(Guid commentID, Guid memberID)
        {
            return _commentRepository.FindComment(commentID, memberID);
        }
        public int DeleteComment(Guid commentID, Guid memberID)
        {
            if(FindComment(commentID, memberID) == null)
                throw new Exception("No comment from you");
            return _commentRepository.DeleteComment(commentID);
        }

        public IEnumerable<Comment> GetComments(Guid taskID)
        {
            return _commentRepository.GetCommentsByTaskID(taskID);
        }
    }
}
