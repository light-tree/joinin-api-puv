using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using DataAccess.Security;
using DataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("majors")]
    public class MajorController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IMajorService _majorService;
        private readonly ILogger<MajorController> _logger;

        public MajorController(IJwtService jwtService, IMajorService majorService, ILogger<MajorController> logger)
        {
            _jwtService = jwtService;
            _majorService = majorService;
            _logger = logger;
        }

        ///<summary>
        ///Get all majors
        ///</summary>
        [HttpGet]
        public IActionResult FilterMajors(
            string? name, 
            int? pageSize, 
            int? page, 
            string? orderBy, 
            string? value)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                _logger.LogInformation("Get information in MajorController");
                _logger.LogError("Get error in MajorController");
                commonResponse = _majorService.FilterMajors(null, name, pageSize, page, orderBy, value);
                return new OkObjectResult(commonResponse);
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Status = StatusCodes.Status500InternalServerError;
                return Ok(commonResponse);
            }
        }




        //[HttpGet]
        //public IActionResult GetAllMajor()
        //{
        //    Guid userId = Guid.Empty;
        //    var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        //    var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
        //    if (decodedToken != null)
        //    {
        //        var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
        //        if (userIdClaim != null)
        //        {
        //            userId = Guid.Parse(userIdClaim.Value);
        //            // Do something with user ID here
        //        }
        //        else throw new Exception("Internal server error");
        //    }
        //    CommonResponse response = new CommonResponse();
        //    try {
        //        //get All Major
        //        var majors = _majorService.GetAllMajors();
        //        response.Status = StatusCodes.Status200OK;
        //        response.Message = "Get all major success";
        //        response.Data = majors;
        //        return new OkObjectResult(response);
        //    }catch (Exception ex)
        //    {
        //        response.Status = StatusCodes.Status500InternalServerError;
        //        response.Message = ex.Message;
        //        return new OkObjectResult(response);
        //    }
        //}

        ///<summary>
        ///Get major's detail
        ///</summary>
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetMajorDetails(Guid id)
        {
            Guid userId = Guid.Empty;
            var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
            if (decodedToken != null)
            {
                var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                if (userIdClaim != null)
                {
                    userId = Guid.Parse(userIdClaim.Value);
                    // Do something with user ID here
                }
                else throw new Exception("Internal server error");
            }
            CommonResponse response = new CommonResponse();
            try
            {
                //get All Major
                var majors = _majorService.FindMajorById(id);
                response.Status = StatusCodes.Status200OK;
                response.Message = "Get major details success";
                response.Data = majors;
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }

        ///<summary>
        ///Update major
        ///</summary>
        [HttpPut]
        [Authorize]
        public IActionResult UpdateMajor(MajorDTOForUpdate major)
        {
            Guid userId = Guid.Empty;
            var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
            if (decodedToken != null)
            {
                var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                if (userIdClaim != null)
                {
                    userId = Guid.Parse(userIdClaim.Value);
                    // Do something with user ID here
                }
                else throw new Exception("Internal server error");
            }
            CommonResponse response = new CommonResponse();
            try
            {
                //update Major
                var majorsID = _majorService.UpdateMajor(major);
                response.Status = StatusCodes.Status200OK;
                response.Message = "Update successfully major";
                response.Data = majorsID;
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }

        ///<summary>
        ///Create major
        ///</summary>
        [HttpPost]
        [Authorize]
        public IActionResult CreateMajor(MajorDTOForCreate major)
        {
            Guid userId = Guid.Empty;
            var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
            if (decodedToken != null)
            {
                var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                if (userIdClaim != null)
                {
                    userId = Guid.Parse(userIdClaim.Value);
                    // Do something with user ID here
                }
                else throw new Exception("Internal server error");
            }
            CommonResponse response = new CommonResponse();
            try
            {
                //create All Major
                var majorsID = _majorService.CreateMajor(major);
                response.Status = StatusCodes.Status200OK;
                response.Message = "Create successfully major";
                response.Data = majorsID;
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }
    }
}
