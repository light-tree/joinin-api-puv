using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using DataAccess.Security;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Authorization;
using System.Transactions;
using BusinessObject.DTOs;
using System.Collections.Generic;
using System.Linq.Expressions;
using API_JoinIn.Utils.Email;
using BusinessObject.Models;
using API_JoinIn.Utils.Notification.Implements;
using Microsoft.AspNetCore.SignalR;
using API_JoinIn.Utils.Notification;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService transactionService;
        private readonly IJwtService jwtService;
        private readonly IUserService userService;
        private readonly IEmailService emailService;
        private readonly IHubContext<NotificationSignalSender> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;

        public TransactionController(ITransactionService transactionService,
                                        IJwtService jwtService,
                                        IUserService userService,
                                        IEmailService emailService,
                                        IHubContext<NotificationSignalSender> hubContext,
                                        IConfiguration configuration,
                                       INotificationService notificationService
                                        )
        {
            this.transactionService = transactionService;   
            this.jwtService = jwtService;
            this.userService = userService;
            this.emailService = emailService;
            _configuration = configuration;
            _notificationService = notificationService;
            _hubContext = hubContext;

        }

        ///<summary>
        ///User create transaction with transactionType is 0 (PAYPAL) or 1 (MOMO)
        ///</summary>
        [Authorize]
        [HttpPost("/create/{transactionType}")]
        public async Task<IActionResult> CreateTransaction(TransactionType transactionType)
        {
            CommonResponse commonResponse = new CommonResponse();
            var userId = "";
            try
            {
                var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var decodedToken = jwtService.DecodeJwtToken(jwtToken);
                if (decodedToken != null)
                {
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                    if (userIdClaim != null)
                    {
                        userId = userIdClaim.Value;
                        // Do something with user ID here
                    }
                    else throw new Exception("Internal Server Error.");
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    BusinessObject.Models.User user = await userService.FindUserByGuid(Guid.Parse(userId));
                    BusinessObject.Models.Transaction tran = await transactionService.CreateTransaction(user, transactionType);

                    commonResponse.Message = "Create transaction succesfully.";
                    commonResponse.Data = tran.TransactionCode;
                    commonResponse.Status = 200;
                    scope.Complete();
                }
                    return Ok(commonResponse);

                

            } catch(Exception ex) {
                commonResponse.Message = "Internal server error.";
                commonResponse.Status = 500;
                return StatusCode(500, commonResponse);
            }
        }

        ///<summary>
        ///Admin update transaction's status
        ///</summary>
       [Authorize(Roles ="Admin")]
        [HttpPut("admin/update")]
        public async Task<IActionResult> UpdateTransactionStatus(TransactionDTO transactionDTO) 
         {
            CommonResponse commonResponse = new CommonResponse();

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var trans = transactionService.GetTransactionByCode(transactionDTO.Code);

                    if (trans ==null)
                    {
                        commonResponse.Status = 400;
                        commonResponse.Message = "Transaction not found";
                        return BadRequest(commonResponse);

                    }
                    

                    var tmp = await transactionService.UpdateTransactionStatus(trans.Result.Id, transactionDTO.Status);
                    if (tmp == null)
                    {
                        commonResponse.Message = "Internal server error.";
                        commonResponse.Status = 500;
                        commonResponse.Data = tmp;
                        return StatusCode(500,commonResponse);
                    }

                   

                    commonResponse.Message = "Update succesfully.";
                    commonResponse.Status = 200;


                  
                    //send email

                    var userId = "";
                 /// chuyển từ trạng thái wating sang success sẽ gửi mail

                    if(tmp.Status == BusinessObject.Enums.TransactionStatus.SUCCESS)
                    {
                       
                        DateTime startDate = tmp.TransactionDate.Value;
                        DateTime endDate = tmp.TransactionDate.Value.AddDays(30);
                        string transactionUpdateMessage = NotificationMessage.BuildAcceptTransaction(transactionDTO.Code);
                        NotificationDTO notificationDTO = new NotificationDTO();
                        notificationDTO.link = "";
                        notificationDTO.message = transactionUpdateMessage;
                      
                        notificationDTO.Image = _configuration["NotificationImgLink"];
                        notificationDTO.CreatedDate = DateTime.Now;
                        notificationDTO.UserId = tmp.UserId;
                        notificationDTO.Name = "Notification of " + tmp.User.FullName;
                        notificationDTO.Status = NotificationStatus.NOT_SEEN_YET;
                        notificationDTO.Type = NotificationType.TRANSACTION_UPDATE;
                        notificationDTO.CreatedDate = DateTime.Now;

                        string notification = JsonConvert.SerializeObject(notificationDTO);

                        var n = _notificationService.AddNotification(
                         new Notification
                         {
                             Name = "Notification of " + tmp.User.FullName,
                             Content = notification,
                             Image = _configuration["NotificationImgLink"],
                             Status = NotificationStatus.NOT_SEEN_YET,
                             Type = NotificationType.TRANSACTION_UPDATE,
                             UserId = tmp.User.Id,
                             CreatedDate = notificationDTO.CreatedDate
                         }
                         );

                        notification = JsonConvert.SerializeObject(notificationDTO);
                        await _hubContext.Clients.All.SendAsync(tmp.User.Id.ToString(), notification);
                        await emailService.SendEmailConfirmTransaction(tmp.User.Email, tmp.User.FullName,startDate,endDate);
                            
                    }
                    scope.Complete();

                    return Ok(commonResponse);

                }
                
            }
            catch
            {
                commonResponse.Message = "Internal server error.";
                commonResponse.Status = 500;
                return StatusCode(500, commonResponse);
            }
         }

        ///<summary>
        ///Admin search transactions
        ///</summary>
       [Authorize(Roles = "Admin")]
        [HttpGet("/admin/get-transaction")]
        public async Task<IActionResult> GetTransaction(Guid? id,
                                                    string? code,
                                                    BusinessObject.Enums.TransactionStatus? transactionStatus,
                                                    Guid? userId,
                                                    DateTime? startDate,
                                                    DateTime? endDate,
                                                    int pageNumber=1,
                                                    int pageSize=10
      
                                                    )
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                commonResponse =  await transactionService.FillterTransaction(id, code, transactionStatus, userId, startDate, endDate, pageNumber, pageSize, orderByExpression: t => t.CreatedDate);
                commonResponse.Status = 200;                                   
                return Ok(commonResponse);
            }
            catch
            {
                commonResponse.Status = 500;
                commonResponse.Message = "Internal Server Error.";
                return StatusCode(500, commonResponse);
            }

        }


    }
}
