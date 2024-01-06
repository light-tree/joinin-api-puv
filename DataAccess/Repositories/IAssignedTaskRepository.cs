using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IAssignedTaskRepository
    {
        List<AssignedTask> CreateAssignedTasks(List<Guid> assignedForIds, Guid newTaskId, Guid createdById);
        int Delete(List<AssignedTask> deletedAssignedTasks);
        void DeleteAssignedTask(Guid memberId, Guid groupId, BusinessObject.Enums.TaskStatus taskStatus);
        int DeleteByAssignedForId(Guid lostAssignedForId);
        int DeleteByGroupId(Guid groupId);
        int DeleteByTaskId(Guid taskId);
        List<AssignedTask> FindByTaskId(Guid id);
        AssignedTask FindByTaskIdAndAssignedForId(Guid taskId, Guid assignedForId);
        List<AssignedTask> FindNotFinishedByAssignedForId(Guid assignedForId);
    }
}
