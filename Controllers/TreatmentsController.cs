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
        private readonly IAuditTrailService _auditTrailService;
        private readonly IRelationshipService _relationshipService;
        private readonly ITreatmentService _treatmentService;
        private IAssetService _assetService;

        public TreatmentsController(ITreatmentService treatmentService, IRelationshipService relationshipService, IAuditTrailService auditTrailService, IAssetService assetService)
        {
            _treatmentService = treatmentService;
            _relationshipService = relationshipService;
            _auditTrailService = auditTrailService;
            _assetService = assetService;
        }

        [NonAction]
        public Treatment CreateTreatment(CreateTreatmentCommand command)
        {
            var newValue = _treatmentService.Create(command).Result;
            _relationshipService.Create(new CreateRelationshipCommand()
            {
                FromType = _assetService.GetSingle(x => x.Id == command.AssetId) != null ? ObjectType.Asset : ObjectType.AssetEdge,
                FromId = command.AssetId,
                ToType = ObjectType.TreatmentPayload,
                ToId = newValue.Payload.Id,
                CreateByUserId = command.CreateByUserId
            });

            _auditTrailService.LogAction(AuditTrailAction.CreateTreatment, newValue.Id, new AuditTrailPayloadModel() {Data = JsonConvert.SerializeObject(command)});
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