﻿using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using SolveIntersection.DB;
using SolveIntersection.DB.Entities;
using System;

namespace SolveIntersection.EndPoint
{
    internal class CreateRightTurnCorridors<T> where T : RoadComponent
    {
        public CreateRightTurnCorridors(Transaction trans, CivilDocument civilDoc, T road, Assembly assembly)
        {
            // Create a new Corridor
            ObjectId newCorridorId = civilDoc.CorridorCollection.Add("Corridor " + Guid.NewGuid());

            Corridor corridor = trans.GetObject(newCorridorId, OpenMode.ForWrite) as Corridor;

            // Get the first alignment of this drawing
            Alignment alignment = road.alignment;

            // Get the first profile of this alignment
            ObjectId profileId = alignment.GetProfileIds()[0];

            // Create the baseline
            Baseline baseline = corridor.Baselines.Add("New Baseline", alignment.ObjectId, profileId);

            //Add region
            BaselineRegion baselineRegion = baseline.BaselineRegions.Add("Rgeion", assembly.Id);

            //Edit Frequence
            baselineRegion.AppliedAssemblySetting.FrequencyAlongCurves = 0.1;
            baselineRegion.AppliedAssemblySetting.FrequencyAlongTangents = 0.2;

            //Add target to corridor
            SubassemblyTargetInfoCollection corridorTargets = baselineRegion.GetTargets();
            var ids = new ObjectIdCollection() { 
                IntersectionDB.getInstance().road_Secondary.alignment.Id,
                IntersectionDB.getInstance().road_Main.alignment.Id,
                IntersectionDB.getInstance().data.featureLineTarget.Id
            };
            foreach (SubassemblyTargetInfo target in corridorTargets)
                if (target.SubassemblyName.Equals("LaneSuperelevationAOR"))
                    if (target.TargetType == SubassemblyLogicalNameType.Offset)
                        target.TargetIds = ids;
            baselineRegion.SetTargets(corridorTargets);
        }
    }
}
