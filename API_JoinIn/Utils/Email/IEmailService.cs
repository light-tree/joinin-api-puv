using BusinessObject.DTOs.Anoucement;
using BusinessObject.DTOs.Email;
using BusinessObject.Enums;
using BusinessObject.Models;

namespace API_JoinIn.Utils.Email
{
    public interface IEmailService
    {
        public  System.Threading.Tasks.Task SendConfirmationEmail(string toEmail, string emailVerificationLink);
        public  System.Threading.Tasks.Task SendRecoveryPasswordEmail(string toEmail, string passwordRecoveryLink);

        public System.Threading.Tasks.Task SendEmailNotifyAssignTaskToMember(string toEmail,
                                                          string Name,
                                                          string TaskName,
                                                          string atGroup,
                                                          string Description,
                                                          string StartDate,
                                                          string Deadline,
                                                          ImportantLevel importantLevel,
                                                          BusinessObject.Enums.TaskStatus Status
                                                          );

        public  System.Threading.Tasks.Task SendNotificationEmail(string toEmail,string Name, List<AnoucementDTO> notification);

        public System.Threading.Tasks.Task SendVerifyCode(string toEmail, string VerifyCode);
        System.Threading.Tasks.Task SendEmailInovationToListUser(List<InvitedMemberDTO> invitedMemberDTO);
        System.Threading.Tasks.Task SendEmailConfirmTransaction(string toEmail, string recieverName, DateTime startDate, DateTime endDate);
   
      
   
        System.Threading.Tasks.Task SendEmailNotificationWhenMemberOutGroup(string toEmail, string LeftMemberName, string LeaderName, string GroupName);
       
        System.Threading.Tasks.Task SendEmailNotificationWhenHasNewApplication(string toEmail, string ApplicantName, string LeaderName, string GroupName, string ApplicantEmail);
     
        System.Threading.Tasks.Task SendEmailNotificationToJoinGroup(string toEmail, string MemberName, string LeaderName, string GropName, bool IsAccepted);
        System.Threading.Tasks.Task SendEmailNotificationRemoveMemberFromGroup(string toEmail, string LeftMemberName, string LeaderName, string GroupName, string Description, string ContactEmail);
        System.Threading.Tasks.Task SendEmailNotifyChangeTaskToMember(string toEmail, string Name, string TaskName, string atGroup, string Description, string StartDate, string Deadline, ImportantLevel importantLevel, BusinessObject.Enums.TaskStatus Status);
        System.Threading.Tasks.Task SendEmailNotificationDeleteTask(string toEmail, string TaskName, string Description, string LeaderName);
        System.Threading.Tasks.Task SendEmailNotificationMemberLeftGroupToLeader(string toEmail, string MemberName, string LeaderName, string GroupName, string Description);
    }
}
