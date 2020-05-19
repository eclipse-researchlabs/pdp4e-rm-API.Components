using Core.AuditTrail.Interfaces.Services;
using Core.AuditTrail.Models;
using Core.Database.Tables;
using Core.Users.Implementation.Commands;
using Core.Users.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace Core.Api.Components.Controllers
{
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IAuditTrailService _auditTrailService;

        public UsersController(IUserService userService, IAuditTrailService auditTrailService)
        {
            _userService = userService;
            _auditTrailService = auditTrailService;
        }

        [NonAction]
        public Guid Create(CreateUserCommand command)
        {
            var newValue = _userService.Create(command).Result;
            _auditTrailService.LogAction(AuditTrailAction.CreateUser, newValue, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(command) });
            return newValue;
        }
    }
}