//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Core.AuditTrail.Interfaces.Services;
//using Core.AuditTrail.Models;
//using Core.Containers.Implementation.Commands;
//using Core.Containers.Interfaces.Services;
//using Core.Database.Enums;
//using Core.Database.Tables;
//using Core.Relationships.Implementation.Commands;
//using Core.Relationships.Interfaces.Services;
//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;

//namespace Core.Api.Controllers
//{
//    [Route("api/containers"), ApiController, EnableCors("CorsRules")]
//    public class ContainerController : ControllerBase
//    {
//        private IContainerService _containerService;
//        private readonly IAuditTrailService _auditTrailService;
//        private IRelationshipService _relationshipService;

//        public ContainerController(IContainerService containerService, IAuditTrailService auditTrailService, IRelationshipService relationshipService)
//        {
//            _containerService = containerService;
//            _auditTrailService = auditTrailService;
//            _relationshipService = relationshipService;
//        }

//        [HttpPost, DisableRequestSizeLimit]
//        public async Task<IActionResult> Create([FromBody] CreateContainerCommand command)
//        {
//            var newValue = await _containerService.Create(command);
//            _auditTrailService.LogAction(AuditTrailAction.CreateContainer, newValue.Id, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(command) });
//            if (command.ParentRootId.HasValue)
//                _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Container, ToType = ObjectType.Container, FromId = command.ParentRootId.Value, ToId = newValue.RootId, Branch = command.Branch });

//            return Ok(newValue);
//        }

//        [HttpDelete("{id}")]
//        public IActionResult Delete(Guid id) => Ok(_containerService.Delete(new DeleteContainerCommand() { Id = id }));
//    }
//}