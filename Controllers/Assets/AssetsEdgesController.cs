//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Core.Assets.Implementation.Commands.Edges;
//using Core.Assets.Interfaces.Services;
//using Core.Database.Enums;
//using Core.Database.Models;
//using Core.Relationships.Implementation.Commands;
//using Core.Relationships.Interfaces.Services;
//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;

//namespace Core.Api.Controllers.Assets
//{
//    [Route("api/assets/edges"), ApiController, EnableCors("CorsRules")]
//    public class AssetsEdgesController : ControllerBase
//    {
//        private IRelationshipService _relationshipService;
//        private IAssetEdgeService _assetEdgeService;

//        public AssetsEdgesController(IRelationshipService relationshipService, IAssetEdgeService assetEdgeService)
//        {
//            _relationshipService = relationshipService;
//            _assetEdgeService = assetEdgeService;
//        }

//        [HttpPost, ProducesResponseType(201)]
//        public async Task<IActionResult> CreateEdge([FromBody] CreateEdgeCommand command)
//        {
//            var newValue = await _relationshipService.Create(new CreateRelationshipCommand()
//            {
//                FromType = ObjectType.Asset,
//                FromId = command.Asset1Guid,
//                ToType = ObjectType.Asset,
//                ToId = command.Asset2Guid,
//                Payload = JsonConvert.SerializeObject(new AssetEdgePayloadModel() { Name = command.Name, Asset1Anchor = command.Asset1Anchor, Asset2Anchor = command.Asset2Anchor })
//            });
//            if (command.ContainerRootId.HasValue) _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Container, FromId = command.ContainerRootId.Value, ToType = ObjectType.AssetEdge, ToId = newValue.Id });
//            return Ok(newValue);
//        }

//        [HttpPut("name"), ProducesResponseType(201)]
//        public IActionResult UpdateEdgeLabel([FromBody] ChangeEdgeLabelCommand command)
//        {
//            _assetEdgeService.Update(command);
//            //_auditTrailService.LogAction(AuditTrailAction.CreateAssetEdge, newValue.Id, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(command) });
//            return Ok();
//        }

//        [HttpDelete("{ids}")]
//        public IActionResult DeleteEdges(string ids)
//        {
//            foreach (var id in ids.Split(',').ToList().ConvertAll(Guid.Parse))
//                _assetEdgeService.Delete(id);
//            return Ok();
//        }
//    }
//}