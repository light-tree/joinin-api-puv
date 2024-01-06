using BusinessObject.Models;

namespace API_JoinIn.Utils.Notification
{
    public class NotificationMessage
    {
        public static string TASK_ASSIGNATION = "You has been assigned to task '{0}' in group '{1}'.";
        public static string TASK_UPDATE = "Task '{0}' in group '{1}' has been updated.";
        public static string NEW_FEEDBACK = "New feedback has been received in group '{0}'.";
        public static string NEW_TASK_COMMENT = "New comment has been posted on task '{0}' in group '{1}'.";
        public static string NEW_APPLICATION = "New application received from user '{0}' for group '{1}'.";
        public static string NEW_INVITATION = "You have been invited to join group '{0}'.";
        public static string GROUP_LEAVING = "User '{0}' has left group '{1}'.";
        public static string GROUP_REMOVING = "Group '{0}' has been removed.";
        public static string TASK_DELETE = "Task '{0}' in group '{1}' has been deleted.";
        public static string MEMBER_REMOVING = "You has been remove in group '{0}'.";
        public static string TRANSACTION_UPDATE = "You order {0} has been accepted, now you can use the vip service at JoinIn.";
        public static string APRROVE_APPLICATION = "Your application at group {0} has been approved.";
        public static string DISAPRROVE_APPLICATION = "Your application at group {0} has been disapproved.";
        public static string ASSIGN_ROLE = "You has assigned role {0} in group {1}";


        public static string BuildTaskAssignationMessage( string task, string group)
        {
            return string.Format(TASK_ASSIGNATION, task, group);
        }

        public static string BuildTaskUpdateMessage(string task, string group)
        {
            return string.Format(TASK_UPDATE, task, group);
        }

        public static string BuildNewFeedbackMessage(string group)
        {
            return string.Format(NEW_FEEDBACK,group);
        }

        public static string BuildNewTaskCommentMessage(string task, string group)
        {
            return string.Format(NEW_TASK_COMMENT, task, group);
        }

        public static string BuildNewApplicationMessage(string user, string group)
        {
            return string.Format(NEW_APPLICATION, user, group);
        }

        public static string BuildNewInvitationMessage(string group)
        {
            return string.Format(NEW_INVITATION, group);
        }

        public static string BuildGroupLeavingMessage(string user, string group)
        {
            return string.Format(GROUP_LEAVING, user, group);
        }

        public static string BuildGroupRemovingMessage(string group)
        {
            return string.Format(GROUP_REMOVING, group);
        }

        public static string BuildTaskDeletetedMessage(string task, string group)
        {
            return string.Format(TASK_DELETE, task, group);
        }

        public static string BuildRemoveMemberMessage(string group)
        {
            return string.Format(MEMBER_REMOVING,  group);
        }
        public static string BuildAcceptTransaction(string transactionCode)
        {
            return string.Format(TRANSACTION_UPDATE, transactionCode);
        }
        public static string BuildApproveApplication(string group)
        {
            return string.Format(APRROVE_APPLICATION, group);
        }

        public static string BuildDisApproveApplication(string group)
        {
            return string.Format(DISAPRROVE_APPLICATION, group);
        }

        public static string BuildAssignedRoleToMember(string role, string group)
        {
            return string.Format(ASSIGN_ROLE, role, group);
        }
    }
}
