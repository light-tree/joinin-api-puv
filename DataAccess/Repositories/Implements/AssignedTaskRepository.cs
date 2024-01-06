using BusinessObject.Data;
using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = BusinessObject.Enums.TaskStatus;

namespace DataAccess.Repositories.Implements
{
    public class AssignedTaskRepository : IAssignedTaskRepository
    {
        private readonly Context _context;

        public AssignedTaskRepository(Context context)
        {
            _context = context;
        }

        public List<AssignedTask> CreateAssignedTasks(List<Guid> assignedForIds, Guid newTaskId, Guid createdById)
        {
            DateTime dateTime = DateTime.Now;
            List<AssignedTask> assignedTasks = new List<AssignedTask>();
            foreach (var assignedForId in assignedForIds)
            {
                AssignedTask assignedTask = FindByTaskIdAndAssignedForId(newTaskId, assignedForId);
                if (assignedTask != null)
                    throw new Exception("Member with Id: " + assignedForId + " is assigned duplicately with this task.");
                assignedTasks.Add(new()
                {
                    TaskId = newTaskId,
                    AssignedById = createdById,
                    AssignedForId = assignedForId,
                    AssignedDate = dateTime,
                });
            }
            _context.AssignedTasks.AddRange(assignedTasks);
            _context.SaveChanges();
            return assignedTasks;
        }

        public AssignedTask FindByTaskIdAndAssignedForId(Guid taskId, Guid assignedForId)
        {
            return _context.AssignedTasks.FirstOrDefault(a => a.TaskId == taskId && a.AssignedForId == assignedForId);
        }

        public int DeleteByAssignedForId(Guid lostAssignedForId)
        {
            AssignedTask lostAssignedTask = _context.AssignedTasks.FirstOrDefault(a => a.AssignedForId == lostAssignedForId);
            _context.AssignedTasks.Remove(lostAssignedTask);
            return _context.SaveChanges();
        }

        public int DeleteByTaskId(Guid taskId)
        {
            List<AssignedTask> deletedAssignedTasks = _context.AssignedTasks.Where(a => a.TaskId == taskId).ToList();
            _context.AssignedTasks.RemoveRange(deletedAssignedTasks);
            return _context.SaveChanges();
        }

        public List<AssignedTask> FindByTaskId(Guid id)
        {
            return _context.AssignedTasks.Where(at => at.TaskId == id).ToList();
        }

        public List<AssignedTask> FindNotFinishedByAssignedForId(Guid assignedForId)
        {
            return _context.AssignedTasks
                .Include(at => at.Task)
                //.Where(at => at.Task.Status != BusinessObject.Enums.TaskStatus.FINISHED && at.AssignedForId == assignedForId)
                .Where(at => at.AssignedForId == assignedForId)
                .ToList();
        }

        public int Delete(List<AssignedTask> deletedAssignedTasks)
        {
            _context.AssignedTasks.RemoveRange(deletedAssignedTasks);
            return _context.SaveChanges();
        }

        public List<BusinessObject.Models.AssignedTask> GetAllAssignedTasksByMemberIdByStatus(Guid memberId, TaskStatus status)
        {
            return _context.AssignedTasks
                .Include(t => t.Task)
                .Where(r => r.AssignedForId == memberId && r.Task.Status == status)
                .ToList();
        }

        public void DeleteAssignedTask(Guid memberId, Guid groupId, BusinessObject.Enums.TaskStatus taskStatus)
        {
            var assignedTask = _context.AssignedTasks.Where(r => r.AssignedForId == memberId && r.Task.GroupId == groupId && r.Task.Status == taskStatus);

            if (assignedTask != null && assignedTask.Count() > 0)
            {
                foreach (var tmp in assignedTask)
                {
                    _context.AssignedTasks.Remove(tmp);
                }
                _context.SaveChanges();
            }

           
        }

        public int DeleteByGroupId(Guid groupId)
        {
            List<AssignedTask> assignedTasks = _context.AssignedTasks
                .Include(at => at.Task)
                .Where(at => at.Task.GroupId == groupId)
                .ToList();

            _context.AssignedTasks.RemoveRange(assignedTasks);
            return _context.SaveChanges();
        }
    }
}
