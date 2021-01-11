// /********************************************************************************
//  * Copyright (c) 2020,2021 Beawre Digital SL
//  *
//  * This program and the accompanying materials are made available under the
//  * terms of the Eclipse Public License 2.0 which is available at
//  * http://www.eclipse.org/legal/epl-2.0.
//  *
//  * SPDX-License-Identifier: EPL-2.0 3
//  *
//  ********************************************************************************/

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
        private IAuditTrailService _auditTrailService;
        private IUserService _userService;

        public UsersController(IUserService userService, IAuditTrailService auditTrailService)
        {
            _userService = userService;
            _auditTrailService = auditTrailService;
        }

        [NonAction]
        public Guid Create(CreateUserCommand command)
        {
            var newValue = _userService.Create(command).Result;
            _auditTrailService.LogAction(AuditTrailAction.CreateUser, newValue, new AuditTrailPayloadModel() {Data = JsonConvert.SerializeObject(command)});
            return newValue;
        }
    }
}