using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrgChart.API.DTOs;
using OrgChart.API.Services;
using SharepointCSOMLib;
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
        private readonly ISharePointService sharePointService;
        private readonly ILogger<EmployeesController> logger;

        public EmployeesController(IMicrosoftGraphService microsoftGraphService,
            ISharePointService sharePointService,
            ILogger<EmployeesController> logger)
        {
            this.microsoftGraphService = microsoftGraphService;
            this.sharePointService = sharePointService;
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
        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery]string query, [FromQuery]string userId)
        {
            try
            {
                var result = await microsoftGraphService.SearchUsers(query, userId);
                return Ok(new APIResponse<IEnumerable<ADUser>> { IsSuccess = true, Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered searching users");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }
        [HttpGet("SearchManagers")]
        public async Task<IActionResult> SearchManagers([FromQuery] string query, [FromQuery] string userId)
        {
            try
            {
                var result = await microsoftGraphService.SearchManagers(query, userId);
                return Ok(new APIResponse<IEnumerable<ADUser>> { IsSuccess = true, Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered searching managers");
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
        public async Task<IActionResult> AssignManager(string userId, ManagerUpdateRequest req, [FromQuery]bool force=false)
        {
            try
            {
                await microsoftGraphService.AssignUserManager(userId, req.ManagerId, force);
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = null });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching user orgchart");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpPost("AssignManagers")]
        public async Task<IActionResult> AssignManagers(ManagerUpdateRequest req, [FromQuery]bool force = false)
        {
            try
            {
                await microsoftGraphService.AssignUsersManager(req.UserIds, req.ManagerId, force);
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
                await microsoftGraphService.UnassignUsersManager(req.UserIds);
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

        //=======
        // assign to self
        [HttpPost("AssignToSelf")]
        public async Task<IActionResult> AssignToSelf(ApprovalItem item)
        {
            try
            {
                if(item.ManagerEmail == item.ToManagerEmail)
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = true, Message = "Employee cannot be reassigned to self", Data = null });
                }
                else
                {
                    if (string.IsNullOrEmpty(item.ManagerEmail))
                    {
                        // bypass approval
                        await microsoftGraphService.AssignUserManager(item.EmployeeEmail, item.ToManagerEmail);
                    }
                    else
                    {
                        // add approval
                        item.ApprovalStatus = ApprovalStatus.PENDING.ToString();
                        await sharePointService.AddApprovalItem(item);
                    }
                }
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = null });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered while assigning user to self");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        // batch assign to self
        [HttpPost("BatchAssignToSelf")]
        public async Task<IActionResult> BatchAssignToSelf(IEnumerable<ApprovalItem> items)
        {
            try
            {
                if (items.Count() == 0)
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = true, Message = "List is empty", Data = null });
                }else if(items.Any(i=> i.ManagerEmail == i.ToManagerEmail))
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = true, Message = "One or more employee(s) in list cannot be reassigned to self", Data = null });
                }
                else
                {
                    var directs = items.Where(i => string.IsNullOrEmpty(i.ManagerEmail));
                    var approvals = items.Where(i => !string.IsNullOrEmpty(i.ManagerEmail));

                    if(directs.Count() > 0)
                    {
                        await microsoftGraphService.AssignUsersManager(directs.Select(i => i.EmployeeEmail), directs.First().ToManagerEmail);
                    }
                    if(approvals.Count() > 0)
                    {
                        approvals = approvals.Select(i =>
                        {
                            i.ApprovalStatus = ApprovalStatus.PENDING.ToString();
                            return i;
                        });
                        await sharePointService.BatchAddApprovalItem(approvals);
                    }
                }
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = null });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered while assigning users to self in batch");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }
        // assign to other
        [HttpPost("AssignToOthers")]
        public async Task<IActionResult> AssignToOther(ApprovalItem item)
        {
            try
            {
                if (item.RequestorEmail == item.ToManagerEmail)
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = true, Message = "Employee cannot be assigned to self", Data = null });
                }
                if (string.IsNullOrEmpty(item.ManagerEmail))
                    {
                        // bypass approval
                        await microsoftGraphService.AssignUserManager(item.EmployeeEmail, item.ToManagerEmail);
                    }
                    else
                    {
                        // add approval
                        item.ApprovalStatus = ApprovalStatus.PENDING.ToString();
                        await sharePointService.AddApprovalItem(item);
                    }
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = null });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered while assigning user to other");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }
        // batch assign to others
        [HttpPost("BatchAssignToOthers")]
        public async Task<IActionResult> BatchAssignToOthers(IEnumerable<ApprovalItem> items)
        {
            try
            {
                if (items.Count() == 0)
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = true, Message = "List is empty", Data = null });
                }
                else if (items.Any(i => i.RequestorEmail == i.ToManagerEmail))
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = true, Message = "One or more employee(s) in list cannot be assigned to self", Data = null });
                }
                else
                {
                    var directs = items.Where(i => string.IsNullOrEmpty(i.ManagerEmail));
                    var approvals = items.Where(i => !string.IsNullOrEmpty(i.ManagerEmail));

                    if (directs.Count() > 0)
                    {
                        await microsoftGraphService.AssignUsersManager(directs.Select(i => i.EmployeeEmail), directs.First().ToManagerEmail);
                    }
                    if (approvals.Count() > 0)
                    {
                        approvals = approvals.Select(i =>
                        {
                            i.ApprovalStatus = ApprovalStatus.PENDING.ToString();
                            return i;
                        });
                        await sharePointService.BatchAddApprovalItem(approvals);
                    }
                }
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = null });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered while assigning users to others i batch");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        // approve
        [HttpPost("ApproveItem")]
        public async Task<IActionResult> ApproveItem(ApprovalItem item)
        {
            try
            {
                var _item = await sharePointService.GetApprovalItem(item.Id);
                if (_item == null)
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = true, Message = "Item id is invalid", Data = null });
                }

                await microsoftGraphService.AssignUserManager(_item.EmployeeEmail, _item.ToManagerEmail, true);
                await sharePointService.UpdateApprovalItem(item.Id, ApprovalStatus.APPROVED.ToString());

                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = null });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered while approving item");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }
        // decline
        [HttpPost("DeclineItem")]
        public async Task<IActionResult> DeclineItem(ApprovalItem item)
        {
            try
            {
                var _item = await sharePointService.GetApprovalItem(item.Id);
                if (_item == null)
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = true, Message = "Item id is invalid", Data = null });
                }

                if (string.IsNullOrEmpty(item.Comment))
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = true, Message = "Comment is required", Data = null });
                }

                //await microsoftGraphService.AssignUserManager(_item.EmployeeEmail, _item.ToManagerEmail, true);
                await sharePointService.UpdateApprovalItem(item.Id, ApprovalStatus.DECLINED.ToString(), item.Comment);

                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = null });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered while approving item");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpGet("{userId}/ApprovalItems/Initiated")]
        public async Task<IActionResult> GetPendingRequestedApprovalItems(string userId)
        {
            try
            {
                var items = await sharePointService.GetInitiatedPendingApprovalItems(userId);
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = items });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching pending requested approval items");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpGet("{userId}/ApprovalItems/PendingAction")]
        public async Task<IActionResult> GetApprovalItemsPendingAction(string userId)
        {
            try
            {
                var items = await sharePointService.GetApprovalItemsPendingAction(userId);
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = items });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching approval items pending action");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }
    }
}
