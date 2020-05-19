using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Assets.Implementation.Commands.Vulnerabilities;
using Core.Assets.Interfaces.Services;
using Core.AuditTrail.Interfaces.Services;
using Core.AuditTrail.Models;
using Core.Database.Enums;
using Core.Database.Tables;
using Core.Relationships.Implementation.Commands;
using Core.Relationships.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Core.Api.Components.Controllers
{
    public class VulnerabilitiesController : ControllerBase
    {
        private IVulnerabilityService _vulnerabilityService;
        private IRelationshipService _relationshipService;
        private IAuditTrailService _auditTrailService;

        public VulnerabilitiesController(IVulnerabilityService vulnerabilityService, IRelationshipService relationshipService, IAuditTrailService auditTrailService)
        {
            _vulnerabilityService = vulnerabilityService;
            _relationshipService = relationshipService;
            _auditTrailService = auditTrailService;
        }

        [NonAction]
        public Vulnerability CreateVulnerability(CreateVulnerabilityCommand command)
        {
            var newValue = _vulnerabilityService.Create(command).Result;
            _relationshipService.Create(new CreateRelationshipCommand() { FromType = ObjectType.Asset, FromId = command.AssetId, ToType = ObjectType.Vulnerabilitie, ToId = newValue.Id, CreateByUserId = HttpContext.Request.UserId() });
            _auditTrailService.LogAction(AuditTrailAction.CreateVulnerabilities, newValue.Id, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(command) });
            return newValue;
        }

        [NonAction]
        public bool Update(UpdateVulnerabilityCommand command) => _vulnerabilityService.Update(command);

        [NonAction]
        public bool Delete(Guid assetId, Guid id)
        {
            _relationshipService.Delete(x => x.FromType == ObjectType.Asset && x.ToType == ObjectType.Vulnerabilitie && x.FromId == assetId && x.ToId == id);
            _vulnerabilityService.Delete(id);
            return true;
        }
    }
}