using Core.Assets.Implementation.Commands;
using Core.Assets.Implementation.Commands.Assets;
using Core.Assets.Interfaces.Services;
using Core.AuditTrail.Interfaces.Services;
using Core.AuditTrail.Models;
using Core.Database.Enums;
using Core.Database.Tables;
using Core.Relationships.Implementation.Commands;
using Core.Relationships.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Core.Api.Components.Controllers.Assets
{
    public class AssetsController : ControllerBase
    {
        protected IAssetService _assetService;
        private IRelationshipService _relationshipService;
        private IAuditTrailService _auditTrailService;

        public AssetsController(IAssetService assetService, IRelationshipService relationshipService, IAuditTrailService auditTrailService)
        {
            _assetService = assetService;
            _relationshipService = relationshipService;
            _auditTrailService = auditTrailService;
        }

        [NonAction]
        public Asset Create(CreateAssetCommand command)
        {
            var newValue = _assetService.Create(command).Result;
            if (command.ContainerRootId.HasValue) _relationshipService.Create(new CreateRelationshipCommand()
            {
                FromType = ObjectType.Container,
                FromId = command.ContainerRootId.Value,
                ToType = ObjectType.Asset,
                ToId = newValue.Id,
                CreateByUserId = command.CreateByUserId,
                Payload = JsonConvert.SerializeObject(new AssetPayloadModel() { X = command.PayloadData?.X, Y = command.PayloadData?.Y })
            });
            _auditTrailService.LogAction(AuditTrailAction.CreateAsset, newValue.Id, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(command) });
            return newValue;
        }

        [NonAction]
        public bool LinkAssetToContainer(LinkAssetToContainerCommand command)
        {
            _relationshipService.Create(new CreateRelationshipCommand()
            {
                FromType = ObjectType.Container,
                FromId = command.ContainerId,
                ToType = ObjectType.Asset,
                ToId = command.AssetId,
                CreateByUserId = command.CreateByUserId,
                Payload = JsonConvert.SerializeObject(new AssetPayloadModel() { X = command.X, Y = command.Y })
            });
            return true;
        }

        [NonAction]
        public bool MovePosition(UpdateAssetPositionCommand command)
        {
            var newValue = _assetService.MovePosition(command).Result;
            _auditTrailService.LogAction(AuditTrailAction.MoveAsset, command.AssetId, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(command) });
            return newValue;
        }

        [NonAction]
        public bool UpdateName(ChangeAssetNameCommand command)
        {
            _assetService.ChangeName(command);
            //_auditTrailService.LogAction(AuditTrailAction.MoveAsset, command.AssetId, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(command) });
            return true;
        }

        [NonAction]
        public bool UpdateIndex(UpdateAssetIndexCommand command) => _assetService.UpdateIndex(command);

        [NonAction]
        public bool UpdateDfdQuestionaire(UpdateDfdQuestionaireCommand command)
        {
            _assetService.UpdateDfdQuestionaire(command);
            //_auditTrailService.LogAction(AuditTrailAction.MoveAsset, command.AssetId, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(command) });
            return true;
        }

        [NonAction]
        public bool UpdateDfdType(UpdateAssetDfdTypeCommand command) => _assetService.UpdateDfdType(command);

        public bool DeleteAsset(string ids)
        {
            if (ids == "undefined") return false;
            foreach (var id in ids.Split(',').ToList().ConvertAll(Guid.Parse))
            {
                _assetService.Delete(id);
                //_auditTrailService.LogAction(AuditTrailAction.RemoveAssetGroup, id, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(id) });
            }
            return true;
        }
    }
}