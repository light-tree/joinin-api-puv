using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Services
{
    public interface ITaskService
    {
        Guid CreateTask(TaskDTOForCreating task, Guid userId);
        int DeleteTask(Guid taskId, Guid userId);
        CommonResponse FilterTasks(Guid userId, Guid? groupId, string? name, int? pageSize, int? page, string? orderBy, string? value);
        BusinessObject.Models.Task findById(Guid taskId);
        BusinessObject.Models.Task findByIdIncludeMember(Guid taskId);
        TaskDetailDTO GetDetailById(Guid id, Guid userId);
        Guid UpdateTask(TaskDTOForUpdating task, Guid userId);
        Guid UpdateTaskStatus(TaskDTOForUpdatingStatus task, Guid userId);
    }
}
