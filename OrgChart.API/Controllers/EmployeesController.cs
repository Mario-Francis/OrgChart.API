using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrgChart.API.DTOs;
using OrgChart.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IMicrosoftGraphService microsoftGraphService;
        private readonly ILogger<EmployeesController> logger;

        public EmployeesController(IMicrosoftGraphService microsoftGraphService, ILogger<EmployeesController> logger)
        {
            this.microsoftGraphService = microsoftGraphService;
            this.logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var employees = await microsoftGraphService.GetUsers();
                return Ok(new APIResponse<IEnumerable<ADUser>> { IsSuccess = true, Message = "Success", Data = employees });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching employees");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetEmployee(string userId)
        {
            try
            {
                var employee = await microsoftGraphService.GetUser(userId);
                return Ok(new APIResponse<ADUser> { IsSuccess = true, Message = "Success", Data = employee });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching employee");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpGet("{userId}/DirectReports")]
        public async Task<IActionResult> GetDirectReports(string userId)
        {
            try
            {
                var result = await microsoftGraphService.GetUserDirectReports(userId);
                return Ok(new APIResponse<IEnumerable<ADUser>> { IsSuccess = true, Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching direct reports");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpGet("{userId}/Managers")]
        public async Task<IActionResult> GetManagers(string userId)
        {
            try
            {
                var result = await microsoftGraphService.GetUserManagers(userId);
                return Ok(new APIResponse<IEnumerable<ADUser>> { IsSuccess = true, Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching managers");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpGet("{userId}/OrgChart")]
        public async Task<IActionResult> GetOrgChart(string userId)
        {
            try
            {
                var result = await microsoftGraphService.GetUserOrgChart(userId);
                return Ok(new APIResponse<IEnumerable<ADUser>> { IsSuccess = true, Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching user orgchart");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpGet("WithoutManagers")]
        public async Task<IActionResult> WithoutManagers()
        {
            try
            {
                var result = await microsoftGraphService.GetUsersWithoutManagers();
                return Ok(new APIResponse<IEnumerable<ADUser>> { IsSuccess = true, Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching user orgchart");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpGet("WithManagers")]
        public async Task<IActionResult> WithManagers()
        {
            try
            {
                var result = await microsoftGraphService.GetUsersWithManagers();
                return Ok(new APIResponse<IEnumerable<ADUser>> { IsSuccess = true, Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching user orgchart");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpPost("{userId}/AssignManager")]
        public async Task<IActionResult> AssignManager(string userId, ManagerUpdateRequest req)
        {
            try
            {
                await microsoftGraphService.AssignUserManager(userId, req.managerId);
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = null });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching user orgchart");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpPost("AssignManagers")]
        public async Task<IActionResult> AssignManagers(ManagerUpdateRequest req)
        {
            try
            {
                await microsoftGraphService.AssignUsersManager(req.userIds, req.managerId);
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = null });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching user orgchart");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpPost("{userId}/UnassignManager")]
        public async Task<IActionResult> UnassignManager(string userId)
        {
            try
            {
                await microsoftGraphService.UnassignUserManager(userId);
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = null });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching user orgchart");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpPost("UnassignManagers")]
        public async Task<IActionResult> UnassignManagers(ManagerUpdateRequest req)
        {
            try
            {
                await microsoftGraphService.UnassignUsersManager(req.userIds);
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = null });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching user orgchart");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpGet("{userId}/ExistsInGroup/{groupId}")]
        public async Task<IActionResult> ExistsInGroup(string userId, string groupId)
        {
            try
            {
                var exists = await microsoftGraphService.UserExistsInGroup(userId, groupId);
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = exists });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching user orgchart");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }
    }
}
