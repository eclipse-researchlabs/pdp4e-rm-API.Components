using Core.Assets.Implementation.Commands.Risks;
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
using System.Threading.Tasks;
using Core.Assets.Implementation.Commands.Treatments;

namespace Core.Api.Components.Controllers
{
    public class RisksController : ControllerBase
    {
        private IRiskService _riskService;
        private IRelationshipService _relationshipService;
        private IAuditTrailService _auditTrailService;
        private ITreatmentService _treatmentService;

        public RisksController(IRiskService riskService, IRelationshipService relationshipService, IAuditTrailService auditTrailService, ITreatmentService treatmentService)
        {
            _riskService = riskService;
            _relationshipService = relationshipService;
            _auditTrailService = auditTrailService;
            _treatmentService = treatmentService;
        }

        [NonAction]
        public Risk CreateRisk(CreateRiskCommand command)
        {
            var newValue = _riskService.Create(command).Result;
            _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Asset, FromId = command.AssetId, ToType = ObjectType.Risk, ToId = newValue.Id });

            foreach (var item in command.Vulnerabilities)
                _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Risk, FromId = newValue.Id, ToType = ObjectType.Vulnerabilitie, ToId = item });

            foreach (var item in command.Risks)
                _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Risk, FromId = newValue.Id, ToType = ObjectType.Risk, ToId = item });

            foreach (var item in command.Treatments)
            {
                var treatment = _treatmentService.Create(new CreateTreatmentCommand()
                {
                    RiskId = newValue.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Type = item.Type
                }).Result;
                _relationshipService.Create(new CreateRelationshipCommand()
                {
                    FromType = ObjectType.Asset,
                    ToType = ObjectType.TreatmentPayload,
                    FromId = command.AssetId,
                    ToId = treatment.Payload.Id
                });
            }

            _auditTrailService.LogAction(AuditTrailAction.CreateRisk, newValue.Id, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(command) });
            return newValue;
        }

        [NonAction]
        public bool UpdateRisk(UpdateRiskCommand command)
        {
            var newValue = _riskService.Update(command);

            _relationshipService.Delete(x => x.FromType == ObjectType.Risk && x.ToType == ObjectType.Vulnerabilitie && x.FromId == command.RootId);
            _relationshipService.Delete(x => x.FromType == ObjectType.Risk && x.ToType == ObjectType.Risk && x.FromId == command.RootId);
            _relationshipService.Delete(x => x.FromType == ObjectType.Risk && x.ToType == ObjectType.Treatment && x.FromId == command.RootId);
            foreach (var item in command.Vulnerabilities)
                _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Risk, FromId = command.RootId, ToType = ObjectType.Vulnerabilitie, ToId = item });

            foreach (var item in command.Risks)
                _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Risk, FromId = command.RootId, ToType = ObjectType.Risk, ToId = item });

            foreach (var item in command.Treatments)
                _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Risk, FromId = command.RootId, ToType = ObjectType.Treatment, ToId = item.Id });

            return true;
        }

        [NonAction]
        public bool Delete(Guid assetId, Guid id)
        {
            _relationshipService.Delete(x => (x.FromType == ObjectType.Asset || x.FromType == ObjectType.AssetBpmn) && x.ToType == ObjectType.Risk && x.FromId == assetId && x.ToId == id);
            return true;
        }
    }
}