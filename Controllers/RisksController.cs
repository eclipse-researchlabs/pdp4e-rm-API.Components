//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Core.Assets.Implementation.Commands.Risks;
//using Core.Assets.Interfaces.Services;
//using Core.AuditTrail.Interfaces.Services;
//using Core.AuditTrail.Models;
//using Core.Database.Enums;
//using Core.Database.Tables;
//using Core.Relationships.Implementation.Commands;
//using Core.Relationships.Interfaces.Services;
//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;

//namespace Core.Api.Controllers
//{
//    [Route("api"), ApiController, EnableCors("CorsRules")]
//    public class RisksController : ControllerBase
//    {
//        private IRiskService _riskService;
//        private IRelationshipService _relationshipService;
//        private IAuditTrailService _auditTrailService;

//        public RisksController(IRiskService riskService, IRelationshipService relationshipService, IAuditTrailService auditTrailService)
//        {
//            _riskService = riskService;
//            _relationshipService = relationshipService;
//            _auditTrailService = auditTrailService;
//        }

//        [HttpPost("risks"), ProducesResponseType(201)]
//        public async Task<IActionResult> CreateRisk([FromBody] CreateRiskCommand command)
//        {
//            var newValue = await _riskService.Create(command);
//            _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Asset, FromId = command.AssetId, ToType = ObjectType.Risk, ToId = newValue.Id });

//            foreach (var item in command.Vulnerabilities)
//                _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Risk, FromId = newValue.Id, ToType = ObjectType.Vulnerabilitie, ToId = item });

//            foreach (var item in command.Risks)
//                _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Risk, FromId = newValue.Id, ToType = ObjectType.Risk, ToId = item });

//            foreach (var item in command.Treatments)
//                _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Risk, FromId = newValue.Id, ToType = ObjectType.Treatment, ToId = item.Id });

//            _auditTrailService.LogAction(AuditTrailAction.CreateRisk, newValue.Id, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(command) });
//            return Created(newValue.Id.ToString(), newValue);
//        }

//        [HttpPut("risks")]
//        public IActionResult UpdateRisk([FromBody] UpdateRiskCommand command)
//        {
//            var newValue = _riskService.Update(command);

//            _relationshipService.Delete(x => x.FromType == ObjectType.Risk && x.ToType == ObjectType.Vulnerabilitie && x.FromId == command.RootId);
//            _relationshipService.Delete(x => x.FromType == ObjectType.Risk && x.ToType == ObjectType.Risk && x.FromId == command.RootId);
//            _relationshipService.Delete(x => x.FromType == ObjectType.Risk && x.ToType == ObjectType.Treatment && x.FromId == command.RootId);
//            foreach (var item in command.Vulnerabilities)
//                _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Risk, FromId = command.RootId, ToType = ObjectType.Vulnerabilitie, ToId = item });

//            foreach (var item in command.Risks)
//                _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Risk, FromId = command.RootId, ToType = ObjectType.Risk, ToId = item });

//            foreach (var item in command.Treatments)
//                _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Risk, FromId = command.RootId, ToType = ObjectType.Treatment, ToId = item.Id });

//            return Ok();
//        }

//        [HttpDelete("assets/{assetId}/risks/{id}")]
//        public IActionResult Delete(Guid assetId, Guid id) {
//            _relationshipService.Delete(x => x.FromType == ObjectType.Asset && x.ToType == ObjectType.Risk && x.FromId == assetId && x.ToId == id);
//            return Ok();
//        }
//    }
//}