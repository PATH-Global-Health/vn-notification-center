using Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotificationCenter.Extentions;
using Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationCenter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public IActionResult Get(int? pageIndex = 0, int? pageSize = 10)
        {
            var rs = _notificationService.Get(User.GetId().Value, pageIndex.Value, pageSize.Value);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.ErrorMessages);
        }

        [HttpPost]
        public IActionResult Add([FromBody] NotificationAddModel model)
        {
            var rs = _notificationService.Add(model);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.ErrorMessages);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("Seen/{notificationId}")]
        public IActionResult Seen(Guid notificationId)
        {
            var rs = _notificationService.Seen(User.GetId().Value, notificationId);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.ErrorMessages);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("UnSeen")]
        public IActionResult GetUnSeen()
        {
            var rs = _notificationService.CountUnSeen(User.GetId().Value);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.ErrorMessages);
        }

        [HttpDelete("id")]
        public IActionResult Delete(Guid id)
        {
            var rs = _notificationService.Delete(id);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.ErrorMessages);
        }
    }
}
