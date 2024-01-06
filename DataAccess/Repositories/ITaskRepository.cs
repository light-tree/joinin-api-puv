using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Repositories
{
    public interface ITaskRepository
    {
        BusinessObject.Models.Task CreateTask(TaskDTOForCreating task, Guid createdById);
        CommonResponse FilterToDoTasks(Guid userId, string? name, int? pageSize, int? page, string? orderBy, string? value);
        CommonResponse FilterGroupTasks(Guid userId, Guid groupId, string? name, int? pageSize, int? page, string? orderBy, string? value);
        BusinessObject.Models.Task FindByNameAndGroupId(string name, Guid groupId);
        BusinessObject.Models.Task FindById(Guid id);
        TaskDetailDTO FindByIdAndUserId(Guid id, Guid userId);
        int UpdateTask(TaskDTOForUpdating taskDTO);
        List<BusinessObject.Models.Task> FindByMainTaskId(Guid taskId);
        int DeleteByTaskId(Guid taskId);
        int UpdateTaskStatus(TaskDTOForUpdatingStatus taskDTO, Guid userId);
        int CountSubTaskByMainTaskId(Guid mainTaskId);
        int CountMainTaskByGroupId(Guid groupId);
        List<BusinessObject.Models.Task> GetAllTasksByMemberIdByStatus(Guid memberId, BusinessObject.Enums.TaskStatus status);
        int DeleteByGroupId(Guid groupId);
        BusinessObject.Models.Task FindTaskByIdIncludeAssignMember(Guid id);
    }
}
