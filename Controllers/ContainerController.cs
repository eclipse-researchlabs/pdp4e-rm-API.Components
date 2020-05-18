using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.AuditTrail.Interfaces.Services;
using Core.AuditTrail.Models;
using Core.Containers.Implementation.Commands;
using Core.Containers.Interfaces.Services;
using Core.Database.Enums;
using Core.Database.Tables;
using Core.Relationships.Implementation.Commands;
using Core.Relationships.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Core.Api.Components.Controllers
{
    public class ContainerController : ControllerBase
    {
        private IContainerService _containerService;
        private readonly IAuditTrailService _auditTrailService;
        private IRelationshipService _relationshipService;

        public ContainerController(IContainerService containerService, IAuditTrailService auditTrailService, IRelationshipService relationshipService)
        {
            _containerService = containerService;
            _auditTrailService = auditTrailService;
            _relationshipService = relationshipService;
        }

        [NonAction]
        public Container Create(CreateContainerCommand command)
        {
            var newValue = _containerService.Create(command).Result;
            _auditTrailService.LogAction(AuditTrailAction.CreateContainer, newValue.Id, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(command) });
            if (command.ParentRootId.HasValue)
                _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Container, ToType = ObjectType.Container, FromId = command.ParentRootId.Value, ToId = newValue.RootId, Branch = command.Branch, CreateByUserId = command.CreateByUserId });

            return newValue;
        }

        [NonAction]
        public bool Delete(Guid id) => _containerService.Delete(new DeleteContainerCommand() { Id = id });
    }
}