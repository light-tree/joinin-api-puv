using BusinessObject.DTOs;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Services.Implements
{
    public class AssignedTaskService : IAssignedTaskService
    {
        private readonly IAssignedTaskRepository _assignedTaskRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IMemberRepository _memberRepository;

        public AssignedTaskService(IAssignedTaskRepository assignedTaskRepository, ITaskRepository taskRepository, IMemberRepository memberRepository)
        {
            _assignedTaskRepository = assignedTaskRepository;
            _taskRepository = taskRepository;
            _memberRepository = memberRepository;
        }

        public Guid AssignTask(Guid userId, AssignedTasksDTO assignedTasksDTO)
        {
            if (assignedTasksDTO.AssignedForIds.GroupBy(x => x).Any(g => g.Count() > 1))
                throw new Exception("Exist duplicated member Id.");

            BusinessObject.Models.Task task = _taskRepository.FindById(assignedTasksDTO.TaskId);
            if (task == null)
                throw new Exception("Task does not exist.");

            Member assignedBy = _memberRepository.FindByUserIdAndGroupId(userId, task.GroupId);
            if (assignedBy == null)
                throw new Exception("Assigner not belong to group or 1 between member or group is not exist.");

            MemberRole memberRole = assignedBy.Role;
            if (memberRole != MemberRole.SUB_LEADER && memberRole != MemberRole.LEADER)
                throw new Exception($"Member must be {MemberRole.SUB_LEADER} or {MemberRole.LEADER} to assign task.");

            foreach (Guid assignedFor in assignedTasksDTO.AssignedForIds)
            {
                if (_memberRepository.FindByIdAndGroupId(assignedFor, task.GroupId) == null)
                    throw new Exception("Member with Id: " + assignedFor + " not belong to group or 1 between member or group is not exist.");
            }

            List<Guid> currentAssignedForIds = _assignedTaskRepository.FindByTaskId(task.Id).Select(a => a.AssignedForId).ToList();
            List<Guid> newAssignedForIds = assignedTasksDTO.AssignedForIds;
            List<Guid> lostAssignedForIds = currentAssignedForIds.Except(newAssignedForIds).ToList();

            //Assigned task is a sub-task
            if (task.MainTaskId != null)
            {
                //New assgigned member
                List<Guid> currentAssignedMemberIdsForMainTask = _assignedTaskRepository.FindByTaskId(task.MainTaskId.Value).Select(at => at.AssignedForId).ToList();
                List<Guid> addedAssignedMemberIdsForMainTask = newAssignedForIds.Except(currentAssignedMemberIdsForMainTask).ToList();
                _assignedTaskRepository.CreateAssignedTasks(addedAssignedMemberIdsForMainTask, task.MainTaskId.Value, assignedBy.Id);
            }

            foreach (Guid id in lostAssignedForIds)
            {
                //Removed assgigned member
                _assignedTaskRepository.DeleteByAssignedForId(id);
            }

            //New assgigned member
            List<Guid> addedAssignedForIds = newAssignedForIds.Except(currentAssignedForIds).ToList();
            _assignedTaskRepository.CreateAssignedTasks(addedAssignedForIds, task.Id, assignedBy.Id);

            return task.Id;
        }
    }
}
