using System;
using System.Linq;
using ThreeInOne.Games.Driving;
using ThreeInOne.Games.Flying;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThreeInOne.EditorTools
{
    public static class GameplayFixMigration
    {
        private const string DrivingScenePath = "Assets/Scenes/DrivingGame.unity";
        private const string FlyingScenePath = "Assets/Scenes/FlyingGame.unity";
        private const float OriginalReferenceFrameRate = 60.0f;

        public static void MigrateAndAudit()
        {
            MigrateDrivingTrafficSpeeds();
            AuditFlyingController();
            AssetDatabase.SaveAssets();
            Debug.Log("GAMEPLAY_FIX_AUDIT Traffic speeds migrated and flying controller settings validated.");
        }

        private static void MigrateDrivingTrafficSpeeds()
        {
            var scene = EditorSceneManager.OpenScene(DrivingScenePath, OpenSceneMode.Single);
            var traffic = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<TrafficMove>(true))
                .ToArray();

            if (traffic.Length == 0)
            {
                throw new InvalidOperationException("No TrafficMove components were found in the driving scene.");
            }

            foreach (var vehicle in traffic)
            {
                // The original values represented units per rendered frame. At the
                // course project's 60 FPS reference, units/second = units/frame * 60.
                // Values above 1 are already migrated, making this operation idempotent.
                if (vehicle.speed <= 1.0f)
                {
                    vehicle.speed *= OriginalReferenceFrameRate;
                    EditorUtility.SetDirty(vehicle);
                }

                if (vehicle.speed <= 1.0f)
                {
                    throw new InvalidOperationException($"Traffic speed migration failed for {vehicle.name}.");
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"GAMEPLAY_FIX_AUDIT Migrated {traffic.Length} traffic vehicles to units per second.");
        }

        private static void AuditFlyingController()
        {
            var scene = EditorSceneManager.OpenScene(FlyingScenePath, OpenSceneMode.Single);
            var controllers = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<PlayerControllerX>(true))
                .ToArray();

            if (controllers.Length != 1)
            {
                throw new InvalidOperationException($"Expected one flying controller but found {controllers.Length}.");
            }

            var serializedController = new SerializedObject(controllers[0]);
            var maxPitch = serializedController.FindProperty("maxPitch").floatValue;
            var autoLevelSpeed = serializedController.FindProperty("autoLevelSpeed").floatValue;
            var inputDeadZone = serializedController.FindProperty("inputDeadZone").floatValue;

            if (maxPitch <= 0.0f || autoLevelSpeed <= 0.0f || inputDeadZone < 0.0f)
            {
                throw new InvalidOperationException("Flying auto-level settings are invalid.");
            }

            Debug.Log($"GAMEPLAY_FIX_AUDIT Flying controller: maxPitch={maxPitch}, autoLevelSpeed={autoLevelSpeed}, deadZone={inputDeadZone}.");
        }
    }
}
