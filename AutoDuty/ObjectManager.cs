﻿using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.GameFunctions;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using System;

namespace AutoDuty
{
    internal static class ObjectManager
    {
        internal static List<GameObject> GetObjectsByRadius(List<GameObject> gameObjects, float radius)
        {
            return gameObjects.Where(o => GetDistanceToPlayer(o) <= radius).ToList();
        }
        internal static List<GameObject> GetObjectsByName(List<GameObject> gameObjects, string name)
        {
            return gameObjects.Where(o => o.Name.ToString().ToUpper() == name.ToUpper()).ToList();
        }
        internal static GameObject GetClosestObjectByName(List<GameObject> gameObjects, string name)
        {
            return gameObjects.OrderBy(GetDistanceToPlayer).FirstOrDefault(p => p.Name.ToString().ToUpper().Equals(name.ToUpper()) && p.IsTargetable);
        }
        internal unsafe static float GetDistanceToPlayer(GameObject gameObject)
        {
            return Vector3.Distance(gameObject.Position, Player.GameObject->Position);
        }
        public static BNpcBase GetObjectNPC(GameObject obj)
        {
            if (obj == null) return null;
            return GetSheet<BNpcBase>().GetRow(obj.DataId);
        }

        internal static ExcelSheet<T> GetSheet<T>() where T : ExcelRow => Svc.Data.GetExcelSheet<T>();

        internal static bool IsBossFromIcon(BattleChara obj)
        {
            if (obj == null) return false;

            //Icon
            if (GetObjectNPC(obj)?.Rank is 1 or 2 /*or 4*/ or 6) return true;

            return false;
        }
        internal static unsafe bool IsValid => Svc.Condition.Any()
        && !Svc.Condition[ConditionFlag.BetweenAreas]
        && !Svc.Condition[ConditionFlag.BetweenAreas51]
        && Player.Available
        && Player.Interactable;

        internal static unsafe bool InCombat(this BattleChara obj)
        {
            return obj.Struct()->Character.InCombat;
        }
        internal static unsafe void InteractWithObject(GameObject baseObj)
        {
            try
            {
                var convObj = (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)baseObj.Address;
                TargetSystem.Instance()->InteractWithObject(convObj, true);
            }
            catch (Exception ex)
            {
                Svc.Log.Error(ex.ToString());
            }
        }

        internal static unsafe bool PlayerIsCasting => Player.Character->IsCasting;
    }
}