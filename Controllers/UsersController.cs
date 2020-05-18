//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Core.AuditTrail.Implementation.Commands;
//using Core.AuditTrail.Interfaces.Services;
//using Core.AuditTrail.Models;
//using Core.Database.Tables;
//using Core.Users.Implementation.Commands;
//using Core.Users.Interfaces.Services;
//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;

//namespace Core.Api.Controllers
//{
//    [Route("api/users"), ApiController, EnableCors("CorsRules")]
//    public class UsersController : ControllerBase
//    {
//        private IUserService _userService;
//        private IAuditTrailService _auditTrailService;

//        public UsersController(IUserService userService, IAuditTrailService auditTrailService)
//        {
//            _userService = userService;
//            _auditTrailService = auditTrailService;
//        }

//        [HttpPost, ProducesResponseType(201)]
//        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
//        {
//            var newValue = await _userService.Create(command);
//            _auditTrailService.LogAction(AuditTrailAction.CreateUser, newValue, new AuditTrailPayloadModel(){ Data = JsonConvert.SerializeObject(command) });
//            return Created(newValue.ToString(), newValue);
//        }
//    }
//}