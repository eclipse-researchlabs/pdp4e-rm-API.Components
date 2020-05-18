//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Core.Assets.Implementation.Commands;
//using Core.Assets.Implementation.Commands.Edges;
//using Core.Assets.Interfaces.Services;
//using Core.AuditTrail.Interfaces.Services;
//using Core.AuditTrail.Models;
//using Core.Database.Enums;
//using Core.Database.Models;
//using Core.Database.Tables;
//using Core.Relationships.Implementation.Commands;
//using Core.Relationships.Interfaces.Services;
//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;

//namespace Core.Api.Components.Controllers.Assets
//{
//    public class AssetsGroupsController : ControllerBase
//    {
//        private IRelationshipService _relationshipService;
//        private IAuditTrailService _auditTrailService;
//        private IAssetService _assetService;

//        public AssetsGroupsController(IRelationshipService relationshipService, IAuditTrailService auditTrailService, IAssetService assetService)
//        {
//            _relationshipService = relationshipService;
//            _auditTrailService = auditTrailService;
//            _assetService = assetService;
//        }

//        [HttpPost, ProducesResponseType(201)]
//        public async Task<IActionResult> CreateGroup([FromBody] CreateAssetCommand command)
//        {
//            command.IsGroup = true;
//            var newValue = await _assetService.Create(command);
//            foreach (var item in command.Assets)
//                _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.AssetGroup, FromId = newValue.Id, ToType = ObjectType.Asset, ToId = item });
//            if (command.ContainerRootId.HasValue) _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Container, FromId = command.ContainerRootId.Value, ToType = ObjectType.AssetGroup, ToId = newValue.Id });

//            _auditTrailService.LogAction(AuditTrailAction.CreateAssetGroup, newValue.Id, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(command) });
//            return Ok(newValue);
//        }

//        [HttpDelete("{ids}"), ProducesResponseType(201)]
//        public IActionResult DeleteGroup(string ids)
//        {
//            foreach (var id in ids.Split(',').ToList().ConvertAll(Guid.Parse))
//            {
//                _relationshipService.Delete(x => x.FromType == ObjectType.Container && x.ToType == ObjectType.AssetGroup && x.ToId == id);
//                _relationshipService.Delete(x => x.FromType == ObjectType.AssetGroup && x.ToType == ObjectType.Asset && x.FromId == id);
//                _auditTrailService.LogAction(AuditTrailAction.RemoveAssetGroup, id, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(id) });
//            }
//            return Ok();
//        }

//    }
//}