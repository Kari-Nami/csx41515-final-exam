using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ThreeInOne.EditorTools
{
    public static class IntegrationAudit
    {
        public static void Run()
        {
            var scenePaths = new[]
            {
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/DrivingGame.unity",
                "Assets/Scenes/FlyingGame.unity",
                "Assets/Scenes/SumoGame.unity"
            };

            var failures = 0;

            foreach (var scenePath in scenePaths)
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                var gameObjectCount = 0;
                var missingScriptCount = 0;
                var missingPrefabCount = 0;
                var allObjects = new System.Collections.Generic.List<GameObject>();

                foreach (var root in scene.GetRootGameObjects())
                {
                    foreach (var gameObject in root.GetComponentsInChildren<Transform>(true))
                    {
                        gameObjectCount++;
                        allObjects.Add(gameObject.gameObject);
                        missingScriptCount += GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject.gameObject);
                        if (PrefabUtility.GetPrefabAssetType(gameObject.gameObject) == PrefabAssetType.MissingAsset)
                        {
                            missingPrefabCount++;
                        }
                    }
                }

                if (scenePath == "Assets/Scenes/MainMenu.unity")
                {
                    AuditMainMenu(allObjects, ref failures);
                }
                else
                {
                    AuditPauseMenu(scenePath, allObjects, ref failures);
                }

                if (missingScriptCount > 0 || missingPrefabCount > 0)
                {
                    failures += missingScriptCount + missingPrefabCount;
                    Debug.LogError($"INTEGRATION_AUDIT {scenePath}: {missingScriptCount} missing scripts and {missingPrefabCount} missing prefabs across {gameObjectCount} objects.");
                }
                else
                {
                    Debug.Log($"INTEGRATION_AUDIT {scenePath}: OK, {gameObjectCount} objects, no missing scripts.");
                }
            }

            var expectedBuildScenes = scenePaths;
            var actualBuildScenes = EditorBuildSettings.scenes;
            if (actualBuildScenes.Length != expectedBuildScenes.Length)
            {
                failures++;
                Debug.LogError($"INTEGRATION_AUDIT Build scene count is {actualBuildScenes.Length}; expected {expectedBuildScenes.Length}.");
            }
            else
            {
                for (var index = 0; index < expectedBuildScenes.Length; index++)
                {
                    if (!actualBuildScenes[index].enabled || actualBuildScenes[index].path != expectedBuildScenes[index])
                    {
                        failures++;
                        Debug.LogError($"INTEGRATION_AUDIT Build scene {index} is incorrect: {actualBuildScenes[index].path}.");
                    }
                }
            }

            if (failures > 0)
            {
                throw new InvalidOperationException($"Integration audit failed with {failures} missing scripts.");
            }

            Debug.Log("INTEGRATION_AUDIT All imported gameplay scenes passed.");
        }

        private static void AuditMainMenu(System.Collections.Generic.List<GameObject> allObjects, ref int failures)
        {
            var controllers = FindComponents<MainMenuController>(allObjects);
            var buttons = FindComponents<Button>(allObjects);
            var eventSystems = FindComponents<EventSystem>(allObjects);

            Require(controllers.Length == 1, $"Main Menu has {controllers.Length} controllers; expected 1.", ref failures);
            Require(buttons.Length == 4, $"Main Menu has {buttons.Length} buttons; expected 4.", ref failures);
            Require(eventSystems.Length == 1, $"Main Menu has {eventSystems.Length} EventSystems; expected 1.", ref failures);
            AuditButtonMethods(buttons, new[] { "PlayDrivingGame", "PlayFlyingGame", "PlaySumoGame", "ExitGame" }, "Main Menu", ref failures);
        }

        private static void AuditPauseMenu(string scenePath, System.Collections.Generic.List<GameObject> allObjects, ref int failures)
        {
            var controllers = FindComponents<PauseMenuController>(allObjects);
            var buttons = FindComponents<Button>(allObjects);
            var eventSystems = FindComponents<EventSystem>(allObjects);

            Require(controllers.Length == 1, $"{scenePath} has {controllers.Length} pause controllers; expected 1.", ref failures);
            Require(buttons.Length == 3, $"{scenePath} has {buttons.Length} pause buttons; expected 3.", ref failures);
            Require(eventSystems.Length == 1, $"{scenePath} has {eventSystems.Length} EventSystems; expected 1.", ref failures);
            AuditButtonMethods(buttons, new[] { "ResumeGame", "RestartGame", "BackToMainMenu" }, scenePath, ref failures);

            if (controllers.Length == 1)
            {
                var serializedController = new SerializedObject(controllers[0]);
                var panel = serializedController.FindProperty("pausePanel").objectReferenceValue as GameObject;
                var resume = serializedController.FindProperty("resumeButton").objectReferenceValue as Button;
                Require(panel != null, $"{scenePath} has no serialized pause panel.", ref failures);
                Require(resume != null, $"{scenePath} has no serialized resume button.", ref failures);
                if (panel != null)
                {
                    Require(!panel.activeSelf, $"{scenePath} pause panel should be inactive by default.", ref failures);
                }
            }
        }

        private static T[] FindComponents<T>(System.Collections.Generic.List<GameObject> allObjects) where T : Component
        {
            return allObjects.SelectMany(gameObject => gameObject.GetComponents<T>()).ToArray();
        }

        private static void AuditButtonMethods(Button[] buttons, string[] expectedMethods, string context, ref int failures)
        {
            var methods = buttons
                .SelectMany(button => Enumerable.Range(0, button.onClick.GetPersistentEventCount())
                    .Select(index => button.onClick.GetPersistentMethodName(index)))
                .Where(method => !string.IsNullOrEmpty(method))
                .ToArray();

            foreach (var expectedMethod in expectedMethods)
            {
                Require(methods.Count(method => method == expectedMethod) == 1,
                    $"{context} requires exactly one {expectedMethod} button callback.", ref failures);
            }
        }

        private static void Require(bool condition, string message, ref int failures)
        {
            if (condition)
            {
                return;
            }

            failures++;
            Debug.LogError($"INTEGRATION_AUDIT {message}");
        }
    }
}
