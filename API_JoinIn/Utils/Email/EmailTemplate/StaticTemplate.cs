namespace API_JoinIn.Utils.Email.EmailTemplate
{
    public static class StaticTemplate
    {
        public static string VerificationEmailTemplate = @"<!DOCTYPE html>
<html>
<head>

  <meta charset=""utf-8"">
  <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
  <title>Email Confirmation</title>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <style type=""text/css"">
  /**
   * Google webfonts. Recommended to include the .woff version for cross-client compatibility.
   */
  @media screen {
    @font-face {
      font-family: 'Source Sans Pro';
      font-style: normal;
      font-weight: 400;
      src: local('Source Sans Pro Regular'), local('SourceSansPro-Regular'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff');
    }
    @font-face {
      font-family: 'Source Sans Pro';
      font-style: normal;
      font-weight: 700;
      src: local('Source Sans Pro Bold'), local('SourceSansPro-Bold'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff');
    }
  }
  /**
   * Avoid browser level font resizing.
   * 1. Windows Mobile
   * 2. iOS / OSX
   */
  body,
  table,
  td,
  a {
    -ms-text-size-adjust: 100%; /* 1 */
    -webkit-text-size-adjust: 100%; /* 2 */
  }
  /**
   * Remove extra space added to tables and cells in Outlook.
   */
  table,
  td {
    mso-table-rspace: 0pt;
    mso-table-lspace: 0pt;
  }
  /**
   * Better fluid images in Internet Explorer.
   */
  img {
    -ms-interpolation-mode: bicubic;
  }
  /**
   * Remove blue links for iOS devices.
   */
  a[x-apple-data-detectors] {
    font-family: inherit !important;
    font-size: inherit !important;
    font-weight: inherit !important;
    line-height: inherit !important;
    color: inherit !important;
    text-decoration: none !important;
  }
  /**
   * Fix centering issues in Android 4.4.
   */
  div[style*=""margin: 16px 0;""] {
    margin: 0 !important;
  }
  body {
    width: 100% !important;
    height: 100% !important;
    padding: 0 !important;
    margin: 0 !important;
  }
  /**
   * Collapse table borders to avoid space between cells.
   */
  table {
    border-collapse: collapse !important;
  }
  a {
    color: #1a82e2;
  }
  img {
    height: auto;
    line-height: 100%;
    text-decoration: none;
    border: 0;
    outline: none;
  }
  </style>

</head>
<body style=""background-color: #e9ecef;"">

  <!-- start preheader -->
  <div class=""preheader"" style=""display: none; max-width: 0; max-height: 0; overflow: hidden; font-size: 1px; line-height: 1px; color: #fff; opacity: 0;"">
    Verification you email
  </div>
  <!-- end preheader -->

  <!-- start body -->
  <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">

    <!-- start logo -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""center"" valign=""top"" style=""padding: 36px 24px;"">
              <a href="""" target=""_blank"" style=""display: inline-block;"">
                <img src=""./logo.png"" alt=""Logo"" border=""0"" width=""48"" style=""display: block; width: 48px; max-width: 48px; min-width: 48px;"">
              </a>
            </td>
          </tr>
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end logo -->

    <!-- start hero -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 36px 24px 0; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; border-top: 3px solid #d4dadf;"">
              <h1 style=""margin: 0; font-size: 32px; font-weight: 700; letter-spacing: -1px; line-height: 48px;"">Confirm Your Email Address</h1>
            </td>
          </tr>
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end hero -->

    <!-- start copy block -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
              <p style=""margin: 0;"">Tap the button below to confirm your email address. If you didn't create an account with <a href=""{}"">JoinIn</a>, you can safely delete this email.</p>
            </td>
          </tr>
          <!-- end copy -->

          <!-- start button -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"">
              <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                <tr>
                  <td align=""center"" bgcolor=""#ffffff"" style=""padding: 12px;"">
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td align=""center"" bgcolor=""#1a82e2"" style=""border-radius: 6px;"">
                          <a href=""{emailVerificationLink}"" target=""_blank"" style=""display: inline-block; padding: 16px 36px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; color: #ffffff; text-decoration: none; border-radius: 6px;"">Verify now</a>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <!-- end button -->

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
              <p style=""margin: 0;"">If that doesn't work, copy and paste the following link in your browser:</p>
              <p style=""margin: 0;""><a href=""{emailVerificationLink}"" target=""{emailVerificationLink}"">{emailVerificationLink}</a></p>
            </td>
          </tr>
          <!-- end copy -->

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-bottom: 3px solid #d4dadf"">
              <p style=""margin: 0;"">Cheers,<br> JoinIn </p>
            </td>
          </tr>
          <!-- end copy -->

        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end copy block -->

    <!-- start footer -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 24px;"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">

          <!-- start permission -->
          <tr>
            <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">You received this email because we received a request for verify your account. If you didn't request verification, you can safely delete this email.</p>
            </td>
          </tr>
          <!-- end permission -->

          <!-- start unsubscribe -->
          <tr>
            <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">To stop receiving these emails, you can <a href=""https://www.blogdesire.com"" target=""_blank"">unsubscribe</a> at any time.</p>
              <p style=""margin: 0;""></p>
            </td>
          </tr>
          <!-- end unsubscribe -->

        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end footer -->

  </table>
  <!-- end body -->

</body>
</html>";
        public static string AnoucementAboutLeaveGroupTemplate = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <title>Notification on leaving the team</title>
</head>
<body style=""font-family: Arial, sans-serif; background-color: #f1f1f1;"">
    <div style=""max-width: 600px; margin: 0 auto; background-color: #fff; padding: 20px;"">
        <h2 style=""color: #555;"">Notification on leaving the team</h2>
        <p style=""color: #555;"">Welcome to this notification email!</p>
        <p style=""color: #555;"">I'm sorry to inform you that our team member <strong>{MemberName}</strong> were not in our team({GroupName}).</p>
        <p style=""color: #555;"">I hope everyone will continue working together and achieve the best results.</p>
        <p style=""color: #555;"">If you have any questions or comments, please feel free to contact me.</p>
        <p style=""color: #555;"">Thank you!</p>
        <p style=""color: #555;"">{LeaderName}</p>
    </div>
</body>
</html>
";
        public static string AnoucementAboutLeaveGroupToLeaderTemplate = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <title>Email about move out group</title>
</head>
<body>
    <table style=""width: 100%; max-width: 600px; margin: 0 auto; font-family: Arial, sans-serif; font-size: 14px; line-height: 1.6; color: #444;"">
        <tr>
            <td style=""background-color: #f8f8f8; padding: 20px; text-align: center;"">
                <h2 style=""color: #444;"">Notification: Team Member Departure</h2>
            </td>
        </tr>
        <tr>
            <td style=""padding: 20px;"">
                <p>Hello {LeaderName},</p>
                <p>I hope you're having a good day. I am sending this email to inform you that a member of our team({GroupName}) has decided to leave. Here are the details:</p>
                <ul>
                    <li><strong>Member's Name:</strong> {MemberName}</li>
                    <li><strong>Reason for Departure:</strong> {Description}</li>
                </ul>
                <p>We need to discuss this matter to ensure that the team continues to operate effectively. Please let me know a suitable schedule so that we can further discuss this issue.</p>
                <p>Thank you for reading this email. I look forward to hearing back from you.</p>
                <p>Best regards,</p>
                <p>{MemberName}</p>
            </td>
        </tr>
        <tr>
            <td style=""background-color: #f8f8f8; padding: 20px; text-align: center;"">
                <p style=""font-size: 12px; color: #888;"">This is an automated email. Please do not reply.</p>
            </td>
        </tr>
    </table>
</body>
</html>
";
        public static string AnoucementAboutRemoveMemberFromGroupTemplate = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Group Membership Removal Notification</title>
</head>
<body style=""font-family: Arial, sans-serif;"">
    <h1>Group Membership Removal Notification</h1>
    <p>Dear {LeftMemberName},</p>
    <p>{Description}</p>
    <p>If you have any questions or concerns regarding this decision, please do not hesitate to contact us at {ContactEmail}.</p>
    <p>Best regards,</p>
    <p>
        {LeaderName}<br>
        {GroupName}
    </p>
    <hr>
 
</body>
</html>
";
        public static string AsignTaskTemplate = @"<!DOCTYPE html>
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

        th {
            background-color: #A2D9CE;
            color: #333;
            font-weight: bold;
            text-transform: uppercase;
        }

    </style>
</head>
<body>
    <h1>New Task Assignment Notification</h1>
    <p>Hello {Name},</p>
    <p>Your leader at team {GroupName} has assigned you a new task. Details of the task are listed below:</p>
    <table>
        <tbody>
            <tr>
                <th>Task Name:</th>
                <td>{TaskName}</td>
            </tr>

            <tr>
                <th>Start Date:</th>
                <td>{StartDate}</td>
            </tr>
            <tr>
                <th>Deadline:</th>
                <td>{Deadline}</td>
            </tr>
            <tr>
                <th>Important:</th>
                <td>{ImportantLevel}</td>
            </tr>
            <tr>
                <th>Description:</th>
                <td>{Description}</td>
            </tr>
            <tr>
                <th>Status:</th>
                <td>{Status}</td>
            </tr>

        </tbody>
    </table>
    <p>Please complete the task before the deadline to ensure project progress. If you encounter any difficulties during the task completion process, please contact your leader for support.</p>
    <p>Good luck!</p>
    <p>Best regards,</p>
    <p>Joinin</p>
</body>
</html>";
        public static string ChangeTaskNotificationTemplate = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <title>Update Task Assignment Notification</title>
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

        th {
            background-color: #A2D9CE;
            color: #333;
            font-weight: bold;
            text-transform: uppercase;
        }

    </style>
</head>
<body>
    <h1>Update Task Assignment Notification</h1>
    <p>Hello {Name},</p>
    <p>Your team members at team {GroupName} has make some change of task. Details of the task are listed below:</p>
    <table>
        <tbody>
            <tr>
                <th>Task Name:</th>
                <td>{TaskName}</td>
            </tr>

            <tr>
                <th>Start Date:</th>
                <td>{StartDate}</td>
            </tr>
            <tr>
                <th>Deadline:</th>
                <td>{Deadline}</td>
            </tr>
            <tr>
                <th>Important:</th>
                <td>{ImportantLevel}</td>
            </tr>
            <tr>
                <th>Description:</th>
                <td>{Description}</td>
            </tr>
            <tr>
                <th>Status:</th>
                <td>{Status}</td>
            </tr>
        </tbody>
    </table>
    <p>Please complete the task before the deadline to ensure project progress. If you encounter any difficulties during the task completion process, please contact your leader for support.</p>
    <p>Good luck!</p>
    <p>Best regards,</p>
    <p>Joinin</p>
</body>
</html>";
        public static string DeleteTaskNotificationTemplate = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Task Deleted</title>
    <style>
        body {
            background-color: #f2f2f2;
            font-family: Arial, sans-serif;
            line-height: 1.5;
            padding: 20px;
        }

      

    </style>
</head>
<body>
    <h1>Task Deleted</h1>
    <p>Dear Team,</p>
    <p>We regret to inform you that the following task has been deleted:</p>
    <ul>
        <li>Task Name: {TaskName}</li>
        <li>Description: {Description}</li>
    </ul>
    <p>If you have any questions or concerns, please don't hesitate to reach out to us.</p>
    <p>Thank you,</p>
    <p>{LeaderName}</p>
</body>
</html>
";
        public static string InvitePeopelTemplate = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <title>Join our group invitation</title>
</head>
<body style=""font-family: Arial; font-size: 14px;"">
    <p>Hello,{receiverName}</p>

    <p>I would like to invite you to join our group ""{groupName}"".</p>

    <p>{content}</p>

    <p>If you are interested in joining us, please click the button below to accept the invitation:</p>

    <a href=""{link}"" style=""display:inline-block;background-color:#008CBA;color:#ffffff;padding:10px 16px;text-decoration:none;border-radius:5px;margin-top:10px;"">Accept the invitation</a>

    <p>Thank you and I look forward to collaborating with you in the future.</p>

    <p>Best regards,</p>

    <p>{senderName}</p>
</body>
</html>";
        public static string NewMemberApplyToGroupNotificationTemplate = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>New Membership Application</title>
    <style>
        body {
            font-family: Arial, sans-serif;
        }

        .container {
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f2f2f2;
        }

        h2 {
            text-align: center;
            color: #333;
        }

        p {
            color: #555;
            line-height: 1.5;
        }

        .footer {
            text-align: center;
            color: #777;
            font-size: 12px;
            margin-top: 20px;
        }

        .button {
            display: inline-block;
            padding: 10px 20px;
            background-color: #337ab7;
            color: #fff;
            text-decoration: none;
            border-radius: 4px;
            margin-top: 20px;
        }

            .button:hover {
                background-color: #23527c;
            }
    </style>
</head>
<body>
    <div class=""container"">
        <h2>New Membership Application</h2>
        <p>Hello {LeaderName},</p>
        <p>We would like to inform you that there is a new membership application for the group ""{GroupName}"".</p>
        <p>Applicant Name: {ApplicantName}</p>
        <p>Applicant Email:  {ApplicantEmail}</p>
        <p>Please review the application and take appropriate action.</p>
        <p>Thank you.</p>
        <p>Best regards,</p>
        <p>JoinIn.</p>
        <p class=""footer"">This is an automated email. Please do not reply.</p>
    </div>
</body>
</html>
";
        public static string NotificationApplicationApproveTemplate = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Welcome to the Group!</title>
    <style>
        body {
            font-family: Arial, sans-serif;
        }

        .container {
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f2f2f2;
        }

        h2 {
            text-align: center;
            color: #333;
        }

        p {
            color: #555;
            line-height: 1.5;
        }

        .footer {
            text-align: center;
            color: #777;
            font-size: 12px;
            margin-top: 20px;
        }

        .button {
            display: inline-block;
            padding: 10px 20px;
            background-color: #337ab7;
            color: #fff;
            text-decoration: none;
            border-radius: 4px;
            margin-top: 20px;
        }

            .button:hover {
                background-color: #23527c;
            }
    </style>
</head>
<body>
    <div class=""container"">
        <h2>Welcome to the Group!</h2>
        <p>Hello {MemberName},</p>
        <p>We are delighted to inform you that your request to join the group({GroupName}) has been approved.</p>
        <p>From now on, you will have access to the group's content and resources. We hope you have a wonderful experience and build positive relationships with other members.</p>
        <p>If you have any questions or need any assistance, please feel free to contact us at this email address.</p>
        <p>Thank you for joining our group!</p>
        <p>Best regards,</p>
        <p>{LeaderName}</p>
        <p class=""footer"">This is an automated email. Please do not reply.</p>
     
    </div>
</body>
</html>
";
        public static string NotificationApplicationDisApproveTemplate = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Your request to join the Group</title>
    <style>
        body {
            font-family: Arial, sans-serif;
        }

        .container {
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f2f2f2;
        }

        h2 {
            text-align: center;
            color: #333;
        }

        p {
            color: #555;
            line-height: 1.5;
        }

        .footer {
            text-align: center;
            color: #777;
            font-size: 12px;
            margin-top: 20px;
        }

        .button {
            display: inline-block;
            padding: 10px 20px;
            background-color: #337ab7;
            color: #fff;
            text-decoration: none;
            border-radius: 4px;
            margin-top: 20px;
        }

            .button:hover {
                background-color: #23527c;
            }
    </style>
</head>
<body>
    <div class=""container"">
        <h2>Your request to join the Group</h2>
        <p>Hello {MemberName},</p>
        <p>We regret to inform you that your request to join the group has been declined.</p>
        <p>We appreciate your interest in our group, but unfortunately, we are unable to approve your request at this time.</p>
        <p>If you have any further questions or would like more information, please feel free to contact us at this email address.</p>
        <p>Thank you for your understanding.</p>
        <p>Best regards,</p>
        <p>{LeaderName}</p>
        <p class=""footer"">This is an automated email. Please do not reply.</p>
    </div>
</body>
</html>
";
        public static string ResetPasswordTemplate = @"<!DOCTYPE html>
<html>
<head>

  <meta charset=""utf-8"">
  <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
  <title>Email Confirmation</title>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <style type=""text/css"">
  /**
   * Google webfonts. Recommended to include the .woff version for cross-client compatibility.
   */
  @media screen {
    @font-face {
      font-family: 'Source Sans Pro';
      font-style: normal;
      font-weight: 400;
      src: local('Source Sans Pro Regular'), local('SourceSansPro-Regular'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff');
    }
    @font-face {
      font-family: 'Source Sans Pro';
      font-style: normal;
      font-weight: 700;
      src: local('Source Sans Pro Bold'), local('SourceSansPro-Bold'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff');
    }
  }
  /**
   * Avoid browser level font resizing.
   * 1. Windows Mobile
   * 2. iOS / OSX
   */
  body,
  table,
  td,
  a {
    -ms-text-size-adjust: 100%; /* 1 */
    -webkit-text-size-adjust: 100%; /* 2 */
  }
  /**
   * Remove extra space added to tables and cells in Outlook.
   */
  table,
  td {
    mso-table-rspace: 0pt;
    mso-table-lspace: 0pt;
  }
  /**
   * Better fluid images in Internet Explorer.
   */
  img {
    -ms-interpolation-mode: bicubic;
  }
  /**
   * Remove blue links for iOS devices.
   */
  a[x-apple-data-detectors] {
    font-family: inherit !important;
    font-size: inherit !important;
    font-weight: inherit !important;
    line-height: inherit !important;
    color: inherit !important;
    text-decoration: none !important;
  }
  /**
   * Fix centering issues in Android 4.4.
   */
  div[style*=""margin: 16px 0;""] {
    margin: 0 !important;
  }
  body {
    width: 100% !important;
    height: 100% !important;
    padding: 0 !important;
    margin: 0 !important;
  }
  /**
   * Collapse table borders to avoid space between cells.
   */
  table {
    border-collapse: collapse !important;
  }
  a {
    color: #1a82e2;
  }
  img {
    height: auto;
    line-height: 100%;
    text-decoration: none;
    border: 0;
    outline: none;
  }
  </style>

</head>
<body style=""background-color: #e9ecef;"">

  <!-- start preheader -->
  <div class=""preheader"" style=""display: none; max-width: 0; max-height: 0; overflow: hidden; font-size: 1px; line-height: 1px; color: #fff; opacity: 0;"">
    Verification you email
  </div>
  <!-- end preheader -->

  <!-- start body -->
  <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">

    <!-- start logo -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""center"" valign=""top"" style=""padding: 36px 24px;"">
              <a href="""" target=""_blank"" style=""display: inline-block;"">
                <img src=""./logo.png"" alt=""Logo"" border=""0"" width=""48"" style=""display: block; width: 48px; max-width: 48px; min-width: 48px;"">
              </a>
            </td>
          </tr>
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end logo -->

    <!-- start hero -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 36px 24px 0; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; border-top: 3px solid #d4dadf;"">
              <h1 style=""margin: 0; font-size: 32px; font-weight: 700; letter-spacing: -1px; line-height: 48px;"">Reset your password</h1>
            </td>
          </tr>
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end hero -->

    <!-- start copy block -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
              <p style=""margin: 0;"">Tap the button below to confirm reset your pasword.</p>
            </td>
          </tr>
          <!-- end copy -->

          <!-- start button -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"">
              <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                <tr>
                  <td align=""center"" bgcolor=""#ffffff"" style=""padding: 12px;"">
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td align=""center"" bgcolor=""#1a82e2"" style=""border-radius: 6px;"">
                          <a href=""{emailVerificationLink}"" target=""_blank"" style=""display: inline-block; padding: 16px 36px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; color: #ffffff; text-decoration: none; border-radius: 6px;"">Reset now</a>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <!-- end button -->

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
            
            </td>
          </tr>
          <!-- end copy -->

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-bottom: 3px solid #d4dadf"">
              <p style=""margin: 0;"">Cheers,<br> JoinIn </p>
            </td>
          </tr>
          <!-- end copy -->

        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end copy block -->

    <!-- start footer -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 24px;"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">

          <!-- start permission -->
          <tr>
            <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">You received this email because we received a request for reset your password. If you didn't request change the pasword, you can safely delete this email.</p>
            </td>
          </tr>
          <!-- end permission -->

          <!-- start unsubscribe -->
          <tr>
            <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">To stop receiving these emails, you can <a href=""https://www.blogdesire.com"" target=""_blank"">unsubscribe</a> at any time.</p>
              <p style=""margin: 0;""></p>
            </td>
          </tr>
          <!-- end unsubscribe -->

        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end footer -->

  </table>
  <!-- end body -->

</body>
</html>";
        public static string TransactionTemplate = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <title>Successful subscription to JoinIn service package</title>
</head>
<body>
    <div style=""width: 100%; max-width: 600px; margin: 0 auto;"">
        <h2 style=""text-align: center;"">SUCCESSFUL SUBSCRIPTION TO JOININ SERVICE PACKAGE</h2>
        <hr>
        <p><b>Hello {recieverName},</b></p>
        <p>Thank you for subscribing to our JoinIn service package. Your subscription has been successful and your account is now active.</p>
        <p>The details of your subscription are as follows:</p>
        <table style=""width: 100%;"">
            <tr>
                <td style=""width: 50%;""><b>Package Name:</b></td>
                <td>JoinIn Premium Plan</td>
            </tr>
            <tr>
                <td style=""width: 50%;""><b>Price:</b></td>
                <td>50,000 VND</td>
            </tr>
            <tr>
                <td style=""width: 50%;""><b>Start Date:</b></td>
                <td>{startDate}</td>
            </tr>
            <tr>
                <td style=""width: 50%;""><b>End Date:</b></td>
                <td>{endDate}</td>
            </tr>
        </table>
        <hr>
        <p>If you have any questions or concerns, please do not hesitate to contact us at [support email address] or [support phone number].</p>
        <p>Thank you for choosing JoinIn as your service provider. We look forward to serving you.</p>
        <br>
        <p>Sincerely,</p>
        <p>JoinIn</p>
    </div>
</body>
</html>
";
        public static string VerifyCodeTemplate = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <title>Verify Code</title>
</head>
<body>
    <div style=""font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;"">
        <h2>Verify Your Account</h2>
        <p style=""font-size: 24px; font-weight: bold;"">{VerifyCode}</p>
        <br>
        <p>Best regards,</p>
        <p>[JoinIn]</p>
    </div>
</body>
</html>
";
    }
}
