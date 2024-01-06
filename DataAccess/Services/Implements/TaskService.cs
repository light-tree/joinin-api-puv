using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DataAccess.Services.Implements
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IAssignedTaskRepository _assignedTaskRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;

        public TaskService(ITaskRepository taskRepository, IMemberRepository memberRepository, IAssignedTaskRepository assignedTaskRepository, ICommentRepository commentRepository, IGroupRepository groupRepository, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _memberRepository = memberRepository;
            _assignedTaskRepository = assignedTaskRepository;
            _commentRepository = commentRepository;
            _groupRepository = groupRepository;
            _userRepository = userRepository;
        }

        public Guid CreateTask(TaskDTOForCreating task, Guid userId)
        {
            Group group = _groupRepository.FindById(task.GroupId);
            if (group == null)
                throw new Exception("Group does not exist.");
            Member member = _memberRepository.FindByUserIdAndGroupId(userId, task.GroupId);
            if (member == null)
                throw new Exception("Creater not belong to group or 1 between member or group is not exist.");
            if (member.Role != MemberRole.LEADER && member.Role != MemberRole.SUB_LEADER)
                throw new Exception($"Member must be {MemberRole.SUB_LEADER} or {MemberRole.LEADER} to create task.");
            if (_taskRepository.FindByNameAndGroupId(task.Name, task.GroupId) != null)
                throw new Exception("Duplicated task's name in this group.");
            if((task.EndDateDeadline - task.StartDateDeadline).TotalDays < 1)
                throw new Exception("End date deadline a must be greater than start date deadline and separated by at least 1 day.");

            Guid groupCreaterUserId = _memberRepository.FindByIdAndGroupId(group.CreatedById.Value, group.Id).UserId;
            UserType createrUserType = _userRepository.GetUserTypeById(groupCreaterUserId);

            int maxMainTaskEachGroup = createrUserType == UserType.FREEMIUM ? 
                BusinessRuleData.MAX_MAIN_TASK_NUMBER_EACH_GROUP_FOR_FREEMIUM : 
                BusinessRuleData.MAX_MAIN_TASK_NUMBER_EACH_GROUP_FOR_PREMIUM;

            int maxSubTaskEachMainTask = createrUserType == UserType.FREEMIUM ?
                BusinessRuleData.MAX_SUB_TASK_NUMBER_EACH_MAIN_TASK_FOR_FREEMIUM :
                BusinessRuleData.MAX_SUB_TASK_NUMBER_EACH_MAIN_TASK_FOR_PREMIUM;

            //Created task is a sub-task
            if (task.MainTaskId != null)
            {
                BusinessObject.Models.Task mainTask = _taskRepository.FindById(task.MainTaskId.Value);
                if (mainTask == null)
                    throw new Exception("Main task does not exist.");
                else if (mainTask.MainTaskId != null)
                    throw new Exception("Main task is already a subtask.");
                if (_taskRepository.CountSubTaskByMainTaskId(task.MainTaskId.Value) >= maxSubTaskEachMainTask)
                    throw new Exception($"Number of subtasks for the main task has reached the limit for this group's creater account ({maxSubTaskEachMainTask}).");
            }
            else
            {
                if (_taskRepository.CountMainTaskByGroupId(task.GroupId) >= maxMainTaskEachGroup)
                    throw new Exception($"Number of main tasks for the group has reached the limit for this group's creater account ({maxMainTaskEachGroup}).");
            }

            return _taskRepository.CreateTask(task, member.Id).Id;
        }

        public CommonResponse FilterTasks(Guid userId, Guid? groupId, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            if(groupId == null)
                return _taskRepository.FilterToDoTasks(userId, name, pageSize, page, orderBy, value);
            else
            {
                Member member = _memberRepository.FindByUserIdAndGroupId(userId, groupId.Value);
                if (member == null)
                    throw new Exception("User does not belong to group or 1 between member or group is not exist");
                return _taskRepository.FilterGroupTasks(userId, groupId.Value, name, pageSize, page, orderBy, value);
            }
        }

        public TaskDetailDTO GetDetailById(Guid id, Guid userId)
        {
            return _taskRepository.FindByIdAndUserId(id, userId);
        }

        public Guid UpdateTask(TaskDTOForUpdating taskDTO, Guid userId)
        {
            BusinessObject.Models.Task task = _taskRepository.FindById(taskDTO.Id);
            if (task == null) throw new Exception("Task is not exist.");

            Member member = _memberRepository.FindByUserIdAndGroupId(userId, task.GroupId);

            if (member == null)
                throw new Exception("Updater not belong to group or 1 between member or group is not exist.");

            BusinessObject.Models.Task updatedTask = _taskRepository.FindByNameAndGroupId(task.Name, task.GroupId);

            DateTime startDateDeadline = taskDTO.StartDateDeadline == null ? updatedTask.StartDateDeadline : taskDTO.StartDateDeadline.Value;
            DateTime endDateDeadline = taskDTO.EndDateDeadline == null ? updatedTask.EndDateDeadline : taskDTO.EndDateDeadline.Value;

            if (updatedTask != null && updatedTask.Id != taskDTO.Id)
                throw new Exception("Duplicated task's name in this group.");

            if ((endDateDeadline - startDateDeadline).TotalDays < 1)
                throw new Exception("End date deadline a must be greater than start date deadline and separated by at least 1 day.");

            //MemberRole memberRole = member.Role;
            //if (memberRole != MemberRole.LEADER && memberRole != MemberRole.SUB_LEADER)
            //    throw new Exception($"Member must be {MemberRole.SUB_LEADER} or {MemberRole.LEADER} to update task.");
            _taskRepository.UpdateTask(taskDTO);

            return task.Id;
        }

        public int DeleteTask(Guid taskId, Guid userId)
        {
            BusinessObject.Models.Task deletedTask = _taskRepository.FindById(taskId);
            if (deletedTask == null) throw new Exception("Task does not exist.");

            Member member = _memberRepository.FindByUserIdAndGroupId(userId, deletedTask.GroupId);
            if (member == null)
                throw new Exception("Deleter not belong to group or 1 between member or group is not exist.");

            if (member.Role != MemberRole.LEADER && member.Role != MemberRole.SUB_LEADER)
                throw new Exception($"Member must be {MemberRole.SUB_LEADER} or {MemberRole.LEADER} to delete task.");

            List<BusinessObject.Models.Task> deletedSubTasks = _taskRepository.FindByMainTaskId(taskId);
            if(deletedSubTasks.Count != 0)
            {
                foreach (BusinessObject.Models.Task task in deletedSubTasks)
                {
                    DeleteTask(task.Id, userId);
                }
            }

            _commentRepository.DeleteByTaskId(taskId);
            _assignedTaskRepository.DeleteByTaskId(taskId);
            return _taskRepository.DeleteByTaskId(taskId);
        }

        public Guid UpdateTaskStatus(TaskDTOForUpdatingStatus taskStatus, Guid userId)
        {
            BusinessObject.Models.Task task = _taskRepository.FindById(taskStatus.Id);
            if (task == null) throw new Exception("Task is not exist.");

            Member member = _memberRepository.FindByUserIdAndGroupId(userId, task.GroupId);
            if (member == null)
                throw new Exception("Updater not belong to group or 1 between member or group is not exist.");

            if(member.Role != MemberRole.LEADER && member.Role != MemberRole.SUB_LEADER)
            {
                AssignedTask assignedTask = _assignedTaskRepository.FindByTaskIdAndAssignedForId(taskStatus.Id, member.Id);
                if (assignedTask == null)
                    throw new Exception($"Only {MemberRole.LEADER}, {MemberRole.SUB_LEADER} or assigned {MemberRole.MEMBER} can update task's status.");
            }

            _taskRepository.UpdateTaskStatus(taskStatus, userId);
            return task.Id;
        }


        public BusinessObject.Models.Task findById(Guid taskId)
        {
            return _taskRepository.FindById(taskId);
        }

        public BusinessObject.Models.Task findByIdIncludeMember(Guid taskId)
        {
            return _taskRepository.FindTaskByIdIncludeAssignMember(taskId);
        }
    }
}
