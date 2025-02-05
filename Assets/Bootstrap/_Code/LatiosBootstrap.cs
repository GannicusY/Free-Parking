using System;
using System.Collections.Generic;
using Latios;
using Latios.Authoring;
using Unity.Entities;

namespace FreeParking.Bootstrap
{
    [UnityEngine.Scripting.Preserve]
    public class LatiosBakingBootstrap : ICustomBakingBootstrap
    {
        public void InitializeBakingForAllWorlds(ref CustomBakingBootstrapContext context)
        {
            Latios.Transforms.Authoring.TransformsBakingBootstrap.InstallLatiosTransformsBakers(ref context);
            Latios.Psyshock.Authoring.PsyshockBakingBootstrap.InstallUnityColliderBakers(ref context);
            Latios.Kinemation.Authoring.KinemationBakingBootstrap.InstallKinemationBakersAndSystems(ref context);
            Latios.Kinemation.Authoring.KinemationBakingBootstrap.InstallMecanimBakersAndSystems(ref context);
        }
    }

    [UnityEngine.Scripting.Preserve]
    public class LatiosEditorBootstrap : ICustomEditorBootstrap
    {
        public World InitializeOrModify(World defaultEditorWorld)
        {
            var world                        = new LatiosWorld(defaultEditorWorld.Name);
            world.zeroToleranceForExceptions = true;

            var systems = DefaultWorldInitialization.GetAllSystemTypeIndices(WorldSystemFilterFlags.Default, true);
            BootstrapTools.InjectSystems(systems, world, world.simulationSystemGroup);

            Latios.Transforms.TransformsBootstrap.InstallTransforms(world, world.simulationSystemGroup);
            Latios.Kinemation.KinemationBootstrap.InstallKinemation(world);
            Latios.Calligraphics.CalligraphicsBootstrap.InstallCalligraphics(world);

            return world;
        }
    }

    [UnityEngine.Scripting.Preserve]
    public class LatiosBootstrap : ICustomBootstrap
    {
        public bool Initialize(string defaultWorldName)
        {
            var world                             = new LatiosWorld(defaultWorldName);
            World.DefaultGameObjectInjectionWorld = world;
            world.useExplicitSystemOrdering       = true;
            world.zeroToleranceForExceptions      = true;

            var systems = DefaultWorldInitialization.GetAllSystemTypeIndices(WorldSystemFilterFlags.Default);

            BootstrapTools.InjectUnitySystems(systems, world, world.simulationSystemGroup);

            CoreBootstrap.InstallSceneManager(world);
            Latios.Transforms.TransformsBootstrap.InstallTransforms(world, world.simulationSystemGroup);
            Latios.Myri.MyriBootstrap.InstallMyri(world);
            Latios.Kinemation.KinemationBootstrap.InstallKinemation(world);
            Latios.Kinemation.KinemationBootstrap.InstallMecanimFeatures(world);
            Latios.Calligraphics.CalligraphicsBootstrap.InstallCalligraphics(world);

            BootstrapTools.InjectRootSuperSystems(systems, world, world.simulationSystemGroup);

            world.initializationSystemGroup.SortSystems();
            world.simulationSystemGroup.SortSystems();
            world.presentationSystemGroup.SortSystems();

            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(world);
            return true;
        }
    }
}

