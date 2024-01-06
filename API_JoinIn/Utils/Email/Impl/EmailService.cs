 using System.Net.Mail;
using System.Net;
using BusinessObject.Enums;
using BusinessObject.Models;
using BusinessObject.DTOs.Anoucement;
using System.Runtime.CompilerServices;
using BusinessObject.DTOs.Email;
using System.Linq.Expressions;
using System.Globalization;
using System.Reflection;
using API_JoinIn.Utils.Email.EmailTemplate;

namespace API_JoinIn.Utils.Email.Impl
{
    public class EmailService : IEmailService
    {

        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private string _smtpPassword;
        public EmailService(IConfiguration configuration)
        {
            _smtpHost = configuration["SmtpConfig:Host"];
            _smtpPort = int.Parse(configuration["SmtpConfig:Port"]);
            _smtpUsername = configuration["SmtpConfig:Username"];
            _smtpPassword = configuration["SmtpConfig:Password"];

        }
        public async System.Threading.Tasks.Task SendEmail(string toEmail, string emailVerificationLink, string subject, string body)
        {
            try
            {
                //// tạo confirmlink 
                //var emailVerificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(toEmail);
                //var defaultApp = FirebaseApp.DefaultInstance;

                //if (string.IsNullOrEmpty(_smtpPassword))
                //{
                //    _smtpPassword = "ymzoogjfgjjogmbx";
                //}
                // gửi email confirm
                var from = new MailAddress(_smtpUsername);
                var to = new MailAddress(toEmail);
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = true;

                        await smtpClient.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {

            }



        }
        public async System.Threading.Tasks.Task SendConfirmationEmail(string toEmail, string emailVerificationLink)
        {
            try
            {
                //// tạo confirmlink 
                //var emailVerificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(toEmail);
                //var defaultApp = FirebaseApp.DefaultInstance;



                ///then you can read the file as you would expect yew!
                //File.ReadAllText(rootDirectory + "/Utils/Email/EmailTemplate/VerificationTemplate.html");
                if (string.IsNullOrEmpty(_smtpPassword))
                {
                    _smtpPassword = "ymzoogjfgjjogmbx";
                }

                // gửi email confirm
                var from = new MailAddress(_smtpUsername);
                var to = new MailAddress(toEmail);
                var subject = "Verify your email";

                string htmlBody = string.Empty;
              


                try
                {

                    htmlBody = StaticTemplate.VerificationEmailTemplate;
                        htmlBody = htmlBody.Replace("{emailVerificationLink}", emailVerificationLink);
                    
                }
                catch
                {
                    var body = @"<!DOCTYPE html>
                                <html>
                                <head>
                                <meta charset='utf-8' />
                                <title>Email Confirmation</title>
                                </head>
                                <body>
                                <div style='background-color:#F5F5F5;padding:20px;'>
                                <h2>Email Confirmation</h2>
                                <p>Please click on the link below to confirm your email address:</p>
                                <a href='" + emailVerificationLink + @"' style='display:inline-block;background-color:#4CAF50;color:#FFFFFF;padding:8px 16px;text-decoration:none;border-radius:4px;'>Confirm Email Address</a>
                                <p>If you didn't request to confirm this email address, please disregard this message.</p>
                                </div>
                                </body>
                                </html>";
                 
                    htmlBody = body;
                  
                }



                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = subject;
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        // Thời gian chờ tối đa: 30 giây
                        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        var cancellationToken = cancellationTokenSource.Token;

                        // Gửi email và chờ đợi trong thời gian chờ tối đa
                        await System.Threading.Tasks.Task.WhenAny(smtpClient.SendMailAsync(message), System.Threading.Tasks.Task.Delay(-1, cancellationToken));
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public async System.Threading.Tasks.Task SendRecoveryPasswordEmail(string toEmail, string passwordRecoveryLink)
        {
            try
            {
                if (string.IsNullOrEmpty(_smtpPassword))
                {
                    _smtpPassword = "ymzoogjfgjjogmbx";
                }


                // gửi email confirm

                var subject = "Reset Password";

                string htmlBody = string.Empty;
                try
                {

                    htmlBody = StaticTemplate.ResetPasswordTemplate;
                        htmlBody = htmlBody.Replace("{emailVerificationLink}", passwordRecoveryLink);
                    
                }
                catch
                {
                    htmlBody = @"<!DOCTYPE html>
                                <html>
                                <head>
                                    <meta charset='utf-8' />
                                    <title>Reset Password</title>
                                </head>
                                <body>
                                    <div style='background-color:#F5F5F5;padding:20px;'>
                                        <h2>Reset your password</h2>
                                        <p>Please click into the link to reset your pasword:</p>
                                        <a href='" + passwordRecoveryLink + @"' style='display:inline-block;background-color:#4CAF50;color:#FFFFFF;padding:8px 16px;text-decoration:none;border-radius:4px;'>Reset</a>
                                        <p>If you didn't ask to confirm this email address, please ignore this message.</p>
                                    </div>
                                </body>
                                </html>";
                }



                var from = new MailAddress(_smtpUsername);
                var to = new MailAddress(toEmail);
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;

                  
                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = subject;
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        var cancellationToken = cancellationTokenSource.Token;

                        // Gửi email và chờ đợi trong thời gian chờ tối đa
                        await System.Threading.Tasks.Task.WhenAny(smtpClient.SendMailAsync(message), System.Threading.Tasks.Task.Delay(-1, cancellationToken));
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }


        public async System.Threading.Tasks.Task SendEmailNotifyAssignTaskToMember(string toEmail,
                                                            string Name,
                                                            string TaskName,
                                                            string atGroup,
                                                            string Description,
                                                            string StartDate,
                                                            string Deadline,
                                                            ImportantLevel importantLevel,
                                                            BusinessObject.Enums.TaskStatus Status
                                                            )
        {
            var from = new MailAddress(_smtpUsername);
            var to = new MailAddress(toEmail);
            var subject = "Task Assigned";

            string htmlBody = string.Empty;
            try
            {

                htmlBody = StaticTemplate.AsignTaskTemplate;
                    htmlBody = htmlBody.Replace("{Name}", Name);
                    htmlBody = htmlBody.Replace("{GroupName}", atGroup);
                    htmlBody = htmlBody.Replace("{TaskName}", TaskName);
                    htmlBody = htmlBody.Replace("{Description}", Description);
                    htmlBody = htmlBody.Replace("{StartDate}", StartDate);
                    htmlBody = htmlBody.Replace("{Deadline}", Deadline);

                    switch (Status)
                    {
                        case BusinessObject.Enums.TaskStatus.FINISHED:
                            htmlBody = htmlBody.Replace("{Status}", "FINISHED");
                            break;
                        case BusinessObject.Enums.TaskStatus.WORKING:
                            htmlBody = htmlBody.Replace("{Status}", "WORKING");
                            break;
                        case BusinessObject.Enums.TaskStatus.NOT_STARTED_YET:
                            htmlBody = htmlBody.Replace("{Status}", "NOT STARTED YET");
                            break;
                        default:
                            break;
                    }
                    //check important level
                    string tmp = "";
                    switch (importantLevel)
                    {

                        case ImportantLevel.OPTIONAL:
                            tmp = "<b><span style=\"color:#0000FF;\">OPTIONAL</span></b>";
                            break;
                        case ImportantLevel.LOW:
                            tmp = "<b><span style=\"color:#008000;\">LOW</span></b>";
                            break;
                        case ImportantLevel.MEDIUM:
                            tmp = "<b><span style=\"color:#FFFF00;\">MEDIUM</span></b>";
                            break;
                        case ImportantLevel.HIGH:
                            tmp = "<b><span style=\"color:#FFA500;\">HIGH</span></b>";
                            break;
                        case ImportantLevel.VERY_HIGH:
                            tmp = "<b><span style=\"color:#FF0000;\">VERY HIGH</span></b>";
                            break;
                        default:
                            tmp = "<b><span style=\"color:#0000FF;\">{ImportantLevel}</span></b>";
                            break;
                    }


                    htmlBody = htmlBody.Replace("{ImportantLevel}", tmp);


                
            }
            catch
            {
                htmlBody = @"<!DOCTYPE html>
                                        <html>

                                        <head>
                                          <meta charset=""UTF-8"">
                                          <title>New Task Assignment Notification</title>
                                          <style type=""text/css"">
                                            body {
                                              font-family: Arial, sans-serif;
                                              font-size: 14px;
                                            }
                                            h1 {
                                              font-size: 18px;
                                              margin-bottom: 0;
                                              color: navy;
                                            }
                                            p {
                                              margin-top: 5px;
                                              margin-bottom: 10px;
                                              color: #222;
                                            }
                                            table {
                                              border-collapse: collapse;
                                              width: 100%;
                                              max-width: 600px;
                                              margin: 20px auto;
                                            }
                                            table td {
                                              padding: 10px;
                                              border: 1px solid #ccc;
                                              text-align: center;
                                            }
                                            th{
                                              background-color: #eee;
                                              padding: 10px;
                                              text-align: center;
                                            }
                                          </style>
                                        </head>

                                        <body>
                                          <h1>New Task Assignment Notification</h1>
                                          <p>Hello [Team Member's Name],</p>
  
                                          <p>Your leader has assigned you a new task. Details of the task are listed below:</p>
  
                                          <table>
                                            <tbody>
                                              <tr>
                                                <th>Task Name:</th>
                                                <td>[Task Name]</td>
                                              </tr>
                                              <tr>
                                                <th>Description:</th>
                                                <td>[Task Description]</td>
                                              </tr>
                                              <tr>
                                                <th>Deadline:</th>
                                                <td>[Deadline]</td>
                                              </tr>
                                            </tbody>
                                          </table>
  
                                          <p>Please complete the task before the deadline to ensure project progress. If you encounter any difficulties during the task completion process, please contact your leader for support.</p>
  
                                          <p>Good luck!</p>

                                          <p>Best regards,</p>
                                          <p>[Sender's Name]</p>
                                        </body>

                                        </html>";
            }


            try
            {
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = subject;
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        var cancellationToken = cancellationTokenSource.Token;

                        // Gửi email và chờ đợi trong thời gian chờ tối đa
                        await System.Threading.Tasks.Task.WhenAny(smtpClient.SendMailAsync(message), System.Threading.Tasks.Task.Delay(-1, cancellationToken));
                    }
                }
            }
            catch
            {

            }

        }

        public async System.Threading.Tasks.Task SendNotificationEmail(string toEmail, string Name, List<AnoucementDTO> notification)
        {



            try
            {

                string[] notificationContent = new string[notification.Count];

                string htmlBody = string.Empty;

                string content = " <tr style=\"border-bottom:1px solid #ebebeb; margin-bottom:26px; padding-bottom:29px; display:block;\">\r\n <td width=\"84\">\r\n  <a style=\"height: 64px; width: 64px; text-align:center; display:block;\">\r\n<img src=\"https://api.adorable.io/avatars/60/abott@adorable.png\" alt=\"Profile Picture\" style=\"border-radius:50%;\">\r\n</a>\r\n</td>\r\n<td style=\"vertical-align:top;\">\r\n<h3 style=\"color: #4d4d4d; font-size: 20px; font-weight: 400; line-height: 30px; margin-bottom: 3px; margin-top:0;\">\r\n<strong>{0}: </strong>{1}, {2} {3}.\r\n</h3>\r\n<span style=\"color:#737373; font-size:14px;\">5 Minutes Ago</span>\r\n </td>\r\n</tr> ";



                string formattedContent = "";
                foreach (AnoucementDTO arr in notification)
                {
                    string name = arr.Name;
                    string Content = arr.Content;
                    DateTime date = arr.CreatedDate.Date;
                    DateTime time = arr.CreatedDate;

                    formattedContent += string.Format(content, name, Content, date.ToString("dd MMMM yyyy"), date.Hour);

                }

                using (StreamReader reader = new StreamReader("./Utils/Email/EmailTemplate/AnoucementTemplate.html"))
                {
                    htmlBody = reader.ReadToEnd();
                    htmlBody = htmlBody.Replace("{Name}", Name);
                    htmlBody = htmlBody.Replace("{Body}", formattedContent);



                    //check important level
                    string tmp = "";




                }
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    var from = new MailAddress(_smtpUsername);
                    var to = new MailAddress(toEmail);
                    var subject = "Notification";
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = subject;
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        var cancellationToken = cancellationTokenSource.Token;

                        // Gửi email và chờ đợi trong thời gian chờ tối đa
                        await System.Threading.Tasks.Task.WhenAny(smtpClient.SendMailAsync(message), System.Threading.Tasks.Task.Delay(-1, cancellationToken));
                    }
                }
            }
            catch
            {

            }


        }


        public async System.Threading.Tasks.Task SendVerifyCode(string toEmail, string VerifyCode)
        {
            if (string.IsNullOrEmpty(_smtpPassword))
            {
                _smtpPassword = "ymzoogjfgjjogmbx";
            }
            var htmlBody = "";
            try
            {

                htmlBody = StaticTemplate.VerifyCodeTemplate;
                    htmlBody = htmlBody.Replace("{VerifyCode}", VerifyCode);
                

                // gửi email 
                var from = new MailAddress(_smtpUsername);
                var to = new MailAddress(toEmail);
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = "Verify code";
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        var cancellationToken = cancellationTokenSource.Token;

                        // Gửi email và chờ đợi trong thời gian chờ tối đa
                        await System.Threading.Tasks.Task.WhenAny(smtpClient.SendMailAsync(message), System.Threading.Tasks.Task.Delay(-1, cancellationToken));
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        private async System.Threading.Tasks.Task SendEmailInovationToUser(InvitedMemberDTO invitedMemberDTO)
        {

            var htmlBody = "";
            try
            {

                htmlBody = StaticTemplate.InvitePeopelTemplate;
                    htmlBody = htmlBody.Replace("{link}", invitedMemberDTO.inovationLink);
                    htmlBody = htmlBody.Replace("{content}", invitedMemberDTO.content);
                    htmlBody = htmlBody.Replace("{receiverName}", invitedMemberDTO.receiverName);
                    htmlBody = htmlBody.Replace("{link}", invitedMemberDTO.senderName);
                    htmlBody = htmlBody.Replace("{groupName}", invitedMemberDTO.groupName);
                    htmlBody = htmlBody.Replace("{senderName}", invitedMemberDTO.senderName);
                
                

                // gửi email 
                var from = new MailAddress(_smtpUsername);
                var to = new MailAddress(invitedMemberDTO.email);
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = "Email from JoinIn";
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        var cancellationToken = cancellationTokenSource.Token;

                        // Gửi email và chờ đợi trong thời gian chờ tối đa
                        await System.Threading.Tasks.Task.WhenAny(smtpClient.SendMailAsync(message), System.Threading.Tasks.Task.Delay(-1, cancellationToken));
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public async System.Threading.Tasks.Task SendEmailInovationToListUser(List<InvitedMemberDTO> invitedMemberDTOs)
        {
            foreach (InvitedMemberDTO i in invitedMemberDTOs)
            {
                await SendEmailInovationToUser(i);
            }

        }


        public async System.Threading.Tasks.Task SendEmailConfirmTransaction(string toEmail,
                                                                               string recieverName,
                                                                               DateTime startDate,
                                                                               DateTime endDate)
        {

            var htmlBody = "";
            try
            {

                    htmlBody = StaticTemplate.TransactionTemplate;
                    htmlBody = htmlBody.Replace("{recieverName}", recieverName);
                    htmlBody = htmlBody.Replace("{startDate}", startDate.ToString("dd/MM/yyyy"));
                    htmlBody = htmlBody.Replace("{endDate}", endDate.ToString("dd/MM/yyyy"));
                  
                


                // gửi email 
                var from = new MailAddress(_smtpUsername);
                var to = new MailAddress(toEmail);
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = "Email from JoinIn";
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        var cancellationToken = cancellationTokenSource.Token;

                        // Gửi email và chờ đợi trong thời gian chờ tối đa
                        await System.Threading.Tasks.Task.WhenAny(smtpClient.SendMailAsync(message), System.Threading.Tasks.Task.Delay(-1, cancellationToken));
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }


        public async System.Threading.Tasks.Task SendEmailNotificationToJoinGroup(string toEmail,
                                                      string MemberName,
                                                      string LeaderName,
                                                      string GropName,
                                                      
                                                      bool IsAccepted)
        {
            var template = "";
            if (IsAccepted)
            {
                template = StaticTemplate.NotificationApplicationApproveTemplate;
            } else
            {
                template = StaticTemplate.NotificationApplicationDisApproveTemplate;
            }

            var htmlBody = template;
            try
            {
                    htmlBody = htmlBody.Replace("{LeaderName}", LeaderName);
                    htmlBody = htmlBody.Replace("{MemberName}", MemberName);
                    htmlBody = htmlBody.Replace("{GropName}", GropName);

                // gửi email 
                var from = new MailAddress(_smtpUsername);
                var to = new MailAddress(toEmail);
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = "Email from JoinIn";
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        var cancellationToken = cancellationTokenSource.Token;

                        // Gửi email và chờ đợi trong thời gian chờ tối đa
                        await System.Threading.Tasks.Task.WhenAny(smtpClient.SendMailAsync(message), System.Threading.Tasks.Task.Delay(-1, cancellationToken));
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public async System.Threading.Tasks.Task SendEmailNotificationWhenHasNewApplication(string toEmail,
                                                     string ApplicantName,
                                                     string LeaderName,
                                                     string GroupName,
                                                     string ApplicantEmail
                                                     )
        {

            var template = StaticTemplate.NewMemberApplyToGroupNotificationTemplate;
            

            var htmlBody = template;
            try
            {
 
                    htmlBody = htmlBody.Replace("{LeaderName}", LeaderName);
                    htmlBody = htmlBody.Replace("{ApplicantName}", ApplicantName);
                    htmlBody = htmlBody.Replace("{GroupName}", GroupName);
                    htmlBody = htmlBody.Replace("{ApplicantEmail}", ApplicantEmail);

// gửi email 
                var from = new MailAddress(_smtpUsername);
                var to = new MailAddress(toEmail);
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = "Email from JoinIn";
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        var cancellationToken = cancellationTokenSource.Token;

                        // Gửi email và chờ đợi trong thời gian chờ tối đa
                        await System.Threading.Tasks.Task.WhenAny(smtpClient.SendMailAsync(message), System.Threading.Tasks.Task.Delay(-1, cancellationToken));
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public async System.Threading.Tasks.Task SendEmailNotificationWhenMemberOutGroup(string toEmail,
                                                   string LeftMemberName,
                                                   string LeaderName,
                                                   string GroupName
                                                   )
        {

            var template = StaticTemplate.AnoucementAboutLeaveGroupTemplate;
            var htmlBody = template;
            try
            {
              
                   
                    htmlBody = htmlBody.Replace("{LeaderName}", LeaderName);
                    htmlBody = htmlBody.Replace("{MemberName}", LeftMemberName);
                    htmlBody = htmlBody.Replace("{GroupName}", GroupName);
                // gửi email 
                var from = new MailAddress(_smtpUsername);
                var to = new MailAddress(toEmail);
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = "Email from JoinIn";
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        var cancellationToken = cancellationTokenSource.Token;

                        // Gửi email và chờ đợi trong thời gian chờ tối đa
                        await System.Threading.Tasks.Task.WhenAny(smtpClient.SendMailAsync(message), System.Threading.Tasks.Task.Delay(-1, cancellationToken));
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public async System.Threading.Tasks.Task SendEmailNotificationRemoveMemberFromGroup(string toEmail,
                                                 string LeftMemberName,
                                                 string LeaderName,
                                                 string GroupName,
                                                 string Description,
                                                 string ContactEmail
                                                 )
        {

            var template = StaticTemplate.AnoucementAboutRemoveMemberFromGroupTemplate;


            var htmlBody = template;
            try
            {
                    htmlBody = htmlBody.Replace("{LeaderName}", LeaderName);
                    htmlBody = htmlBody.Replace("{LeftMemberName}", LeftMemberName);
                    htmlBody = htmlBody.Replace("{GroupName}", GroupName);
                    htmlBody = htmlBody.Replace("{Description}", Description);
                    htmlBody = htmlBody.Replace("{ContactEmail}", ContactEmail);

                


                // gửi email 
                var from = new MailAddress(_smtpUsername);
                var to = new MailAddress(toEmail);
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = "Email from JoinIn";
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        var cancellationToken = cancellationTokenSource.Token;

                        // Gửi email và chờ đợi trong thời gian chờ tối đa
                        await System.Threading.Tasks.Task.WhenAny(smtpClient.SendMailAsync(message), System.Threading.Tasks.Task.Delay(-1, cancellationToken));
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public async System.Threading.Tasks.Task SendEmailNotifyChangeTaskToMember(string toEmail,
                                                           string Name,
                                                           string TaskName,
                                                           string atGroup,
                                                           string Description,
                                                           string StartDate,
                                                           string Deadline,
                                                           ImportantLevel importantLevel,
                                                           BusinessObject.Enums.TaskStatus Status
                                                           )
        {
            var from = new MailAddress(_smtpUsername);
            var to = new MailAddress(toEmail);
            var subject = "Task Assigned";

            string htmlBody = string.Empty;
            try
            {

                htmlBody = StaticTemplate.ChangeTaskNotificationTemplate;
                    htmlBody = htmlBody.Replace("{Name}", Name);
                    htmlBody = htmlBody.Replace("{GroupName}", atGroup);
                    htmlBody = htmlBody.Replace("{TaskName}", TaskName);
                    htmlBody = htmlBody.Replace("{Description}", Description);
                    htmlBody = htmlBody.Replace("{StartDate}", StartDate);
                    htmlBody = htmlBody.Replace("{Deadline}", Deadline);

                    switch(Status)
                    {
                        case BusinessObject.Enums.TaskStatus.FINISHED:
                            htmlBody = htmlBody.Replace("{Status}", "FINISHED");
                            break;
                        case BusinessObject.Enums.TaskStatus.WORKING:
                            htmlBody = htmlBody.Replace("{Status}", "WORKING");
                            break;
                        case BusinessObject.Enums.TaskStatus.NOT_STARTED_YET:
                            htmlBody = htmlBody.Replace("{Status}", "NOT STARTED YET");
                            break;
                        default:
                            break;
                    }
                   

                    //check important level
                    string tmp = "";
                    switch (importantLevel)
                    {
                        case ImportantLevel.OPTIONAL:
                            tmp = "<b><span style=\"color:#0000FF;\">OPTIONAL</span></b>";
                            break;
                        case ImportantLevel.LOW:
                            tmp = "<b><span style=\"color:#008000;\">LOW</span></b>";
                            break;
                        case ImportantLevel.MEDIUM:
                            tmp = "<b><span style=\"color:#FFFF00;\">MEDIUM</span></b>";
                            break;
                        case ImportantLevel.HIGH:
                            tmp = "<b><span style=\"color:#FFA500;\">HIGH</span></b>";
                            break;
                        case ImportantLevel.VERY_HIGH:
                            tmp = "<b><span style=\"color:#FF0000;\">VERY HIGH</span></b>";
                            break;
                        default:
                            tmp = "<b><span style=\"color:#0000FF;\">{ImportantLevel}</span></b>";
                            break;
                    }


                    htmlBody = htmlBody.Replace("{ImportantLevel}", tmp);


                
            }
            catch
            {
                var body = @"<!DOCTYPE html>
                                        <html>

                                        <head>
                                          <meta charset=""UTF-8"">
                                          <title>New Task Assignment Notification</title>
                                          <style type=""text/css"">
                                            body {
                                              font-family: Arial, sans-serif;
                                              font-size: 14px;
                                            }
                                            h1 {
                                              font-size: 18px;
                                              margin-bottom: 0;
                                              color: navy;
                                            }
                                            p {
                                              margin-top: 5px;
                                              margin-bottom: 10px;
                                              color: #222;
                                            }
                                            table {
                                              border-collapse: collapse;
                                              width: 100%;
                                              max-width: 600px;
                                              margin: 20px auto;
                                            }
                                            table td {
                                              padding: 10px;
                                              border: 1px solid #ccc;
                                              text-align: center;
                                            }
                                            th{
                                              background-color: #eee;
                                              padding: 10px;
                                              text-align: center;
                                            }
                                          </style>
                                        </head>

                                        <body>
                                          <h1>New Task Assignment Notification</h1>
                                          <p>Hello [Team Member's Name],</p>
  
                                          <p>Your team haschange some task that you has been assign. Details of the task are listed below:</p>
  
                                          <table>
                                            <tbody>
                                              <tr>
                                                <th>Task Name:</th>
                                                <td>[Task Name]</td>
                                              </tr>
                                              <tr>
                                                <th>Description:</th>
                                                <td>[Task Description]</td>
                                              </tr>
                                              <tr>
                                                <th>Deadline:</th>
                                                <td>[Deadline]</td>
                                              </tr>
                                            </tbody>
                                          </table>
  
                                          <p>Please complete the task before the deadline to ensure project progress. If you encounter any difficulties during the task completion process, please contact your leader for support.</p>
  
                                          <p>Good luck!</p>

                                          <p>Best regards,</p>
                                          <p>[Sender's Name]</p>
                                        </body>

                                        </html>";
            }


            try
            {
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = subject;
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        var cancellationToken = cancellationTokenSource.Token;

                        // Gửi email và chờ đợi trong thời gian chờ tối đa
                        await System.Threading.Tasks.Task.WhenAny(smtpClient.SendMailAsync(message), System.Threading.Tasks.Task.Delay(-1, cancellationToken));
                    }
                }
            }
            catch
            {

            }

        }


        public async System.Threading.Tasks.Task SendEmailNotificationDeleteTask(string toEmail,
                                                string TaskName,
                                                string Description,
                                                string LeaderName
                                             
                                                )
        {

            var template = StaticTemplate.DeleteTaskNotificationTemplate;


            var htmlBody = template;
            try
            {
                   
                    htmlBody = htmlBody.Replace("{LeaderName}", LeaderName);
                    htmlBody = htmlBody.Replace("{Description}", Description);
                    htmlBody = htmlBody.Replace("{TaskName}", TaskName);
                // gửi email 
                var from = new MailAddress(_smtpUsername);
                var to = new MailAddress(toEmail);
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = "Email from JoinIn";
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        var cancellationToken = cancellationTokenSource.Token;

                        // Gửi email và chờ đợi trong thời gian chờ tối đa
                        await System.Threading.Tasks.Task.WhenAny(smtpClient.SendMailAsync(message), System.Threading.Tasks.Task.Delay(-1, cancellationToken));
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public async System.Threading.Tasks.Task SendEmailNotificationMemberLeftGroupToLeader(string toEmail,
                                            string MemberName,
                                            string LeaderName,
                                            string GroupName,
                                            string Description
                                            )
        {

            var template = StaticTemplate.AnoucementAboutLeaveGroupToLeaderTemplate;


            var htmlBody = template;
            try
            {
                    htmlBody = htmlBody.Replace("{LeaderName}", LeaderName);
                    htmlBody = htmlBody.Replace("{MemberName}", MemberName);
                    htmlBody = htmlBody.Replace("{GroupName}", GroupName);
                    htmlBody = htmlBody.Replace("{Description}", Description);
                   
                // gửi email 
                var from = new MailAddress(_smtpUsername);
                var to = new MailAddress(toEmail);
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = "Email from JoinIn";
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        var cancellationToken = cancellationTokenSource.Token;

                        // Gửi email và chờ đợi trong thời gian chờ tối đa
                        await System.Threading.Tasks.Task.WhenAny(smtpClient.SendMailAsync(message), System.Threading.Tasks.Task.Delay(-1, cancellationToken));
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}
