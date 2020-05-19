using Core.Assets.Implementation.Commands.Treatments;
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

namespace Core.Api.Components.Controllers
{
    public class TreatmentsController : ControllerBase
    {
        private readonly ITreatmentService _treatmentService;
        private readonly IRelationshipService _relationshipService;
        private readonly IAuditTrailService _auditTrailService;

        public TreatmentsController(ITreatmentService treatmentService, IRelationshipService relationshipService, IAuditTrailService auditTrailService)
        {
            _treatmentService = treatmentService;
            _relationshipService = relationshipService;
            _auditTrailService = auditTrailService;
        }

        [NonAction]
        public Treatment CreateTreatment(CreateTreatmentCommand command)
        {
            var newValue = _treatmentService.Create(command).Result;
            _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Asset, FromId = command.AssetId, ToType = ObjectType.Treatment, ToId = newValue.Id, CreateByUserId = command.CreateByUserId });
            _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Risk, FromId = command.RiskId, ToType = ObjectType.Treatment, ToId = newValue.Id, CreateByUserId = command.CreateByUserId });
            _auditTrailService.LogAction(AuditTrailAction.CreateTreatment, newValue.Id, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(command) });
            return newValue;
        }

        [NonAction]
        public bool Update(UpdateTreatmentCommand command)
        {
            var value = _treatmentService.Update(command);
            //_relationshipService.Delete(x => x.FromType == ObjectType.Risk && x.ToType == ObjectType.Treatment && x.ToId == command.Id);
            //_relationshipService.Create(new CreateRelationshipCommand() { FromType = risk});
            return true;
        }

        [NonAction]
        public bool Delete(Guid assetId, Guid id)
        {
            _relationshipService.Delete(x => x.FromType == ObjectType.Asset && x.ToType == ObjectType.Treatment && x.FromId == assetId && x.ToId == id);
            return true;
        }
    }
}