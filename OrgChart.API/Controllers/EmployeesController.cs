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
        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] string userId)
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
        public async Task<IActionResult> AssignManager(string userId, ManagerUpdateRequest req, [FromQuery] bool force = false)
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
        public async Task<IActionResult> AssignManagers(ManagerUpdateRequest req, [FromQuery] bool force = false)
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
                if (item.ManagerEmail.ToLower() == item.ToManagerEmail.ToLower())
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "Employee cannot be reassigned to self", Data = null });
                }
                else
                {
                    if (await sharePointService.IsEmployeePendingRequestExists(item.EmployeeEmail))
                    {
                        return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "A pending request already exist for the specified employee", Data = null });
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
                            item.ApprovalType = ApprovalTypes.Approval.ToString();
                            await sharePointService.AddApprovalItem(item);
                        }
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
                    return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "List is empty", Data = null });
                }
                else if (items.Any(i => i.ManagerEmail.ToLower() == i.ToManagerEmail.ToLower()))
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "One or more employee(s) in list cannot be reassigned to self", Data = null });
                }
                else
                {
                    var pendingItems = new List<string>();
                    foreach (var i in items)
                    {
                        if (await sharePointService.IsEmployeePendingRequestExists(i.EmployeeEmail.ToLower()))
                        {
                            pendingItems.Add(i.EmployeeEmail);
                        }
                    }

                    if (pendingItems.Count > 0)
                    {
                        return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "Pending request(s) already exist for the employees: " + string.Join(", ", pendingItems), Data = null });
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
                                i.ApprovalType = ApprovalTypes.Approval.ToString();
                                i.ApprovalStatus = ApprovalStatus.PENDING.ToString();
                                return i;
                            });
                            await sharePointService.BatchAddApprovalItem(approvals);
                        }
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
                if (item.RequestorEmail.ToLower() == item.ToManagerEmail.ToLower())
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "Employee cannot be assigned to self", Data = null });
                }
                if (item.ManagerEmail.ToLower() == item.ToManagerEmail.ToLower())
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "Employee cannot be reassigned to manager", Data = null });
                }

                if (await sharePointService.IsEmployeePendingRequestExists(item.EmployeeEmail))
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "A pending request already exist for the specified employee", Data = null });
                }
                else
                {
                    if (string.IsNullOrEmpty(item.ManagerEmail))
                    {
                        // bypass approval
                        //await microsoftGraphService.AssignUserManager(item.EmployeeEmail, item.ToManagerEmail);

                        // add acceptance approval
                        item.ApprovalStatus = ApprovalStatus.PENDING.ToString();
                        item.ApprovalType = ApprovalTypes.Acceptance.ToString();
                        await sharePointService.AddApprovalItem(item);
                    }
                    else if (item.RequestorEmail.ToLower() == item.ManagerEmail.ToLower())
                    {
                        //await microsoftGraphService.AssignUserManager(item.EmployeeEmail, item.ToManagerEmail, true);
                        // add acceptance approval
                        item.ApprovalStatus = ApprovalStatus.PENDING.ToString();
                        item.ApprovalType = ApprovalTypes.Acceptance.ToString();
                        await sharePointService.AddApprovalItem(item);
                    }
                    else
                    {
                        // add approval
                        item.ApprovalStatus = ApprovalStatus.PENDING.ToString();
                        item.ApprovalType = ApprovalTypes.Approval.ToString();
                        await sharePointService.AddApprovalItem(item);
                    }
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
                    return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "List is empty", Data = null });
                }
                else if (items.Any(i => i.RequestorEmail.ToLower() == i.ToManagerEmail.ToLower()))
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "One or more employee(s) in list cannot be assigned to self", Data = null });
                }
                else if (items.Any(i => i.ManagerEmail.ToLower() == i.ToManagerEmail.ToLower()))
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "One or more employee(s) in list cannot be reassigned to manager", Data = null });
                }
                else
                {
                    var pendingItems = new List<string>();
                    foreach (var i in items)
                    {
                        if (await sharePointService.IsEmployeePendingRequestExists(i.EmployeeEmail.ToLower()))
                        {
                            pendingItems.Add(i.EmployeeEmail);
                        }
                    }

                    if (pendingItems.Count > 0)
                    {
                        return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "Pending request(s) already exist for the employees: " + string.Join(", ", pendingItems), Data = null });
                    }
                    else
                    {
                        var directs = items.Where(i => string.IsNullOrEmpty(i.ManagerEmail) || i.RequestorEmail.ToLower() == i.ManagerEmail.ToLower());
                        var approvals = items.Where(i => !(string.IsNullOrEmpty(i.ManagerEmail) || i.RequestorEmail.ToLower() == i.ManagerEmail.ToLower()));

                        if (directs.Count() > 0)
                        {
                            //await microsoftGraphService.AssignUsersManager(directs.Select(i => i.EmployeeEmail), directs.First().ToManagerEmail, true);
                            directs = directs.Select(i =>
                            {
                                i.ApprovalStatus = ApprovalStatus.PENDING.ToString();
                                i.ApprovalType = ApprovalTypes.Acceptance.ToString();
                                return i;
                            });
                            await sharePointService.BatchAddApprovalItem(directs);
                        }
                        if (approvals.Count() > 0)
                        {
                            approvals = approvals.Select(i =>
                            {
                                i.ApprovalStatus = ApprovalStatus.PENDING.ToString();
                                i.ApprovalType = ApprovalTypes.Approval.ToString();
                                return i;
                            });
                            await sharePointService.BatchAddApprovalItem(approvals);
                        }
                    }
                }
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = null });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered while assigning users to others in batch");
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
                    return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "Item id is invalid", Data = null });
                }
                var comment = string.IsNullOrEmpty(item.Comment) ? null : item.Comment;

                // get user
                var user = await microsoftGraphService.GetUser(_item.EmployeeEmail.ToLower());
                if (_item.ApprovalType == ApprovalTypes.Approval.ToString() && user.Manager.Email.ToLower() != _item.ManagerEmail.ToLower())
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = false, Message = $"You're no longer the manager of {_item.EmployeeName}. Kindly decline this request.", Data = null });
                }

                if (_item.ApprovalType == ApprovalTypes.Approval.ToString())
                {
                    if (await sharePointService.IsManagerHasMultiplePendingRequestForEmployee(_item.EmployeeEmail, _item.ManagerEmail))
                    {
                        return BadRequest(new APIResponse<object> { IsSuccess = false, Message = $"There are more than one pending request for {_item.EmployeeName}. Kindly decline others so you can approve only one", Data = null });
                    }
                    else
                    {
                        // by pass acceptance if assign to self
                        if (_item.RequestorEmail.ToLower() == _item.ToManagerEmail.ToLower())
                        {
                            await microsoftGraphService.AssignUserManager(_item.EmployeeEmail, _item.ToManagerEmail, true);
                            await sharePointService.UpdateApprovalItem(item.Id, ApprovalStatus.APPROVED.ToString(), comment);
                        }
                        else
                        {
                            await sharePointService.UpdateApprovalItem(item.Id, ApprovalStatus.APPROVED.ToString(), comment);
                            _item.ApprovalStatus = ApprovalStatus.PENDING.ToString();
                            _item.ApprovalType = ApprovalTypes.Acceptance.ToString();
                            _item.Comment = null;
                            _item.ReviewDate = null;
                            await sharePointService.AddApprovalItem(_item);
                        }
                    }
                }
                else if (_item.ApprovalType == ApprovalTypes.Acceptance.ToString())
                {
                    await microsoftGraphService.AssignUserManager(_item.EmployeeEmail, _item.ToManagerEmail, true);
                    await sharePointService.UpdateApprovalItem(item.Id, ApprovalStatus.APPROVED.ToString(), comment);
                }
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
                    return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "Item id is invalid", Data = null });
                }

                if (string.IsNullOrEmpty(item.Comment))
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "Comment is required", Data = null });
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

        // cancel
        [HttpPost("CancelItem")]
        public async Task<IActionResult> CancelItem(ApprovalItem item)
        {
            try
            {
                var _item = await sharePointService.GetApprovalItem(item.Id);
                if (_item == null)
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "Item id is invalid", Data = null });
                }

                if (_item.ApprovalStatus != ApprovalStatus.PENDING.ToString())
                {
                    return BadRequest(new APIResponse<object> { IsSuccess = false, Message = "Request cannot be canceled as it has been acted upon by the employee manager", Data = null });
                }
                else
                {
                    await sharePointService.DeleteApprovalItem(item.Id);
                }

                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = null });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered while canceling item");
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

        [HttpGet("{userId}/ApprovalItems/PendingAcceptance")]
        public async Task<IActionResult> GetApprovalItemsPendingAcceptance(string userId)
        {
            try
            {
                var items = await sharePointService.GetApprovalItemsPendingAcceptance(userId);
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = items });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching approval items pending acceptance");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }


        [HttpGet("{userId}/profile")]
        public async Task<IActionResult> GetUserProfile(string userId)
        {
            try
            {
                var profile = await microsoftGraphService.GetProfile(userId);
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = profile });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered fetching user profile");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpPost("{userId}/UpdateProfile")]
        public async Task<IActionResult> UpdateUserProfile(string userId, Profile profile)
        {
            try
            {
                await microsoftGraphService.UpdateProfile(userId, profile);
                return Ok(new APIResponse<object> { IsSuccess = true, Message = "Success", Data = null });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered updating user profile");
                return StatusCode(500, new APIResponse<object> { IsSuccess = false, Message = ex.Message });
            }
        }




        //[HttpGet("test")]
        //public async Task<IActionResult> test()
        //{
        //    //var obj = await microsoftGraphService.GetUserPhoto("prince@mariofc.onmicrosoft.com");
        //    //return Ok(obj);

        //    //await microsoftGraphService.UpdateUserPhoto("prince@mariofc.onmicrosoft.com", "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");
        //    //return Ok(new { message = "Uploaded successfully" });

        //    await microsoftGraphService.DeleteUserPhoto("prince@mariofc.onmicrosoft.com");
        //    return Ok(new { message = "Deleted successfully" });
        //}
    }
}
