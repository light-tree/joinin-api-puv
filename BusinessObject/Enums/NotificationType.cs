using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Enums
{
    public enum NotificationType
    {
        TASK_ASSIGN, 
        UNASSIGN_TASK, 
        TASK_UPDATE, 
        NEW_FEEDBACK, 
        NEW_TASK_COMMENT, 
        NEW_APPLY, 
        NEW_INVITE, 
        GROUP_LEAVING, 
        GROUP_REMOVING,
        TASK_DELETE,
        MEMBER_REMOVING,
        TRANSACTION_UPDATE,
        APPROVE_APPLICATION,
        DISAPPROVE_APPLICATION,
        ASSIGN_ROLE
    }
}
