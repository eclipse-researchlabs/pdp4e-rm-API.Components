﻿// /********************************************************************************
//  * Copyright (c) 2020,2021 Beawre Digital SL
//  *
//  * This program and the accompanying materials are made available under the
//  * terms of the Eclipse Public License 2.0 which is available at
//  * http://www.eclipse.org/legal/epl-2.0.
//  *
//  * SPDX-License-Identifier: EPL-2.0 3
//  *
//  ********************************************************************************/

using Core.Assets.Implementation.Commands.Edges;
using Core.Assets.Interfaces.Services;
using Core.Database.Enums;
using Core.Database.Models;
using Core.Database.Tables;
using Core.Relationships.Implementation.Commands;
using Core.Relationships.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Core.Api.Components.Controllers.Assets
{
    public class AssetsEdgesController : ControllerBase
    {
        private IAssetEdgeService _assetEdgeService;
        private IRelationshipService _relationshipService;

        public AssetsEdgesController(IRelationshipService relationshipService, IAssetEdgeService assetEdgeService)
        {
            _relationshipService = relationshipService;
            _assetEdgeService = assetEdgeService;
        }

        [NonAction]
        public Relationship CreateEdge(CreateEdgeCommand command)
        {
            var newValue = _relationshipService.Create(new CreateRelationshipCommand()
            {
                FromType = ObjectType.Asset,
                FromId = command.Asset1Guid,
                ToType = ObjectType.Asset,
                ToId = command.Asset2Guid,
                CreateByUserId = command.CreateByUserId,
                Payload = JsonConvert.SerializeObject(new AssetEdgePayloadModel() {Name = command.Name, Asset1Anchor = command.Asset1Anchor, Asset2Anchor = command.Asset2Anchor})
            });
            if (command.ContainerRootId.HasValue)
                _relationshipService.Create(new CreateRelationshipCommand()
                    {FromType = ObjectType.Container, FromId = command.ContainerRootId.Value, ToType = ObjectType.AssetEdge, ToId = newValue.Id, CreateByUserId = command.CreateByUserId});
            return newValue;
        }

        [NonAction]
        public bool UpdateEdgeLabel(ChangeEdgeLabelCommand command)
        {
            _assetEdgeService.Update(command);
            //_auditTrailService.LogAction(AuditTrailAction.CreateAssetEdge, newValue.Id, new AuditTrailPayloadModel() { Data = JsonConvert.SerializeObject(command) });
            return true;
        }

        [NonAction]
        public bool DeleteEdges(string ids)
        {
            foreach (var id in ids.Split(',').ToList().ConvertAll(Guid.Parse))
                _assetEdgeService.Delete(id);
            return true;
        }
    }
}