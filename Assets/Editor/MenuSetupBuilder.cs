using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ThreeInOne.EditorTools
{
    public static class MenuSetupBuilder
    {
        private const string MainMenuPath = "Assets/Scenes/MainMenu.unity";
        private const string PausePrefabPath = "Assets/Prefabs/PauseMenu.prefab";

        private static readonly Color Background = Hex("081426");
        private static readonly Color Panel = Hex("10223D");
        private static readonly Color PanelEdge = Hex("244B72");
        private static readonly Color Cyan = Hex("3CE6E0");
        private static readonly Color Gold = Hex("FFC857");
        private static readonly Color Coral = Hex("FF6577");
        private static readonly Color White = Hex("F4F8FF");
        private static readonly Color Muted = Hex("8FA8C3");

        private static Font font;

        public static void Build()
        {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureFolders();
            CreateMainMenuScene();
            var pausePrefab = CreatePauseMenuPrefab();
            AddPauseMenuToGameScenes(pausePrefab);
            ConfigureBuildScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("MENU_SETUP Main menu, pause prefab, and all gameplay scene instances created successfully.");
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }
        }

        private static void CreateMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = SceneNames.MainMenu;

            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Background;
            camera.orthographic = true;
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);

            var controllerObject = new GameObject("Main Menu Controller", typeof(MainMenuController));
            var controller = controllerObject.GetComponent<MainMenuController>();

            var canvasObject = new GameObject("Main Menu Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            CreateEventSystem("Main Menu EventSystem");

            var background = CreateImage("Background", canvasObject.transform, Background);
            Stretch(background.rectTransform);

            var glowTop = CreateImage("Top Accent", background.transform, Cyan);
            SetRect(glowTop.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -6f), new Vector2(1920f, 12f));

            var glowBottom = CreateImage("Bottom Accent", background.transform, Gold);
            SetRect(glowBottom.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 6f), new Vector2(1920f, 12f));

            var card = CreateImage("Menu Card", background.transform, Panel);
            SetRect(card.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760f, 900f));
            AddOutline(card.gameObject, PanelEdge, new Vector2(3f, -3f));

            var eyebrow = CreateText("Eyebrow", card.transform, "WELCOME TO", 28, Cyan, FontStyle.Bold, TextAnchor.MiddleCenter);
            SetRect(eyebrow.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -90f), new Vector2(640f, 48f));

            var title = CreateText("Title", card.transform, "TRIPLE THREAT\nARCADE", 70, White, FontStyle.Bold, TextAnchor.MiddleCenter);
            SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -185f), new Vector2(680f, 150f));
            AddOutline(title.gameObject, new Color(0f, 0f, 0f, 0.55f), new Vector2(3f, -3f));

            var subtitle = CreateText("Subtitle", card.transform, "THREE GAMES. ONE CHALLENGE.", 23, Muted, FontStyle.Normal, TextAnchor.MiddleCenter);
            SetRect(subtitle.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -285f), new Vector2(640f, 42f));

            var driving = CreateButton("Driving Game Button", card.transform, "MAD DRIVER", Cyan, Background);
            SetRect(driving.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -390f), new Vector2(580f, 94f));
            UnityEventTools.AddPersistentListener(driving.onClick, controller.PlayDrivingGame);

            var flying = CreateButton("Flying Game Button", card.transform, "FLY LIKE A BIRD", Gold, Background);
            SetRect(flying.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -505f), new Vector2(580f, 94f));
            UnityEventTools.AddPersistentListener(flying.onClick, controller.PlayFlyingGame);

            var sumo = CreateButton("Sumo Game Button", card.transform, "I'M A SUMO AND A BALL", Coral, White);
            SetRect(sumo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -620f), new Vector2(580f, 94f));
            UnityEventTools.AddPersistentListener(sumo.onClick, controller.PlaySumoGame);

            var exit = CreateButton("Exit Button", card.transform, "EXIT", PanelEdge, White);
            SetRect(exit.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -735f), new Vector2(580f, 82f));
            UnityEventTools.AddPersistentListener(exit.onClick, controller.ExitGame);

            var byline = CreateText("Byline", card.transform, "By Kari Nami", 22, Muted, FontStyle.Italic, TextAnchor.MiddleCenter);
            SetRect(byline.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 48f), new Vector2(620f, 38f));

            EditorSceneManager.SaveScene(scene, MainMenuPath);
        }

        private static GameObject CreatePauseMenuPrefab()
        {
            var root = new GameObject("PauseMenu", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(PauseMenuController));
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var eventSystem = new GameObject("Pause EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.transform.SetParent(root.transform, false);

            var overlay = CreateImage("Pause Overlay", root.transform, new Color(0.015f, 0.035f, 0.07f, 0.9f));
            Stretch(overlay.rectTransform);

            var card = CreateImage("Pause Card", overlay.transform, Panel);
            SetRect(card.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(620f, 650f));
            AddOutline(card.gameObject, Cyan, new Vector2(3f, -3f));

            var label = CreateText("Pause Label", card.transform, "GAME PAUSED", 24, Cyan, FontStyle.Bold, TextAnchor.MiddleCenter);
            SetRect(label.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -80f), new Vector2(520f, 44f));

            var title = CreateText("Pause Title", card.transform, "PAUSED", 76, White, FontStyle.Bold, TextAnchor.MiddleCenter);
            SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -160f), new Vector2(540f, 100f));

            var hint = CreateText("Pause Hint", card.transform, "ESC TO RESUME", 20, Muted, FontStyle.Normal, TextAnchor.MiddleCenter);
            SetRect(hint.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -230f), new Vector2(500f, 36f));

            var resume = CreateButton("Resume Button", card.transform, "RESUME", Cyan, Background);
            SetRect(resume.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -330f), new Vector2(450f, 88f));

            var restart = CreateButton("Restart Button", card.transform, "RESTART", Gold, Background);
            SetRect(restart.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -435f), new Vector2(450f, 88f));

            var back = CreateButton("Back To Main Menu Button", card.transform, "BACK TO MAIN MENU", PanelEdge, White);
            SetRect(back.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -540f), new Vector2(450f, 88f));

            var controller = root.GetComponent<PauseMenuController>();
            var serializedController = new SerializedObject(controller);
            serializedController.FindProperty("pausePanel").objectReferenceValue = overlay.gameObject;
            serializedController.FindProperty("resumeButton").objectReferenceValue = resume;
            serializedController.ApplyModifiedPropertiesWithoutUndo();

            UnityEventTools.AddPersistentListener(resume.onClick, controller.ResumeGame);
            UnityEventTools.AddPersistentListener(restart.onClick, controller.RestartGame);
            UnityEventTools.AddPersistentListener(back.onClick, controller.BackToMainMenu);

            overlay.gameObject.SetActive(false);
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, PausePrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        private static void AddPauseMenuToGameScenes(GameObject pausePrefab)
        {
            var scenePaths = new[]
            {
                "Assets/Scenes/DrivingGame.unity",
                "Assets/Scenes/FlyingGame.unity",
                "Assets/Scenes/SumoGame.unity"
            };

            foreach (var scenePath in scenePaths)
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                foreach (var existing in scene.GetRootGameObjects().Where(root => root.name == "PauseMenu").ToArray())
                {
                    UnityEngine.Object.DestroyImmediate(existing);
                }

                var instance = (GameObject)PrefabUtility.InstantiatePrefab(pausePrefab, scene);
                instance.name = "PauseMenu";
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }

        private static void ConfigureBuildScenes()
        {
            var paths = new[]
            {
                MainMenuPath,
                "Assets/Scenes/DrivingGame.unity",
                "Assets/Scenes/FlyingGame.unity",
                "Assets/Scenes/SumoGame.unity"
            };
            EditorBuildSettings.scenes = paths.Select(path => new EditorBuildSettingsScene(path, true)).ToArray();
        }

        private static EventSystem CreateEventSystem(string name)
        {
            return new GameObject(name, typeof(EventSystem), typeof(StandaloneInputModule)).GetComponent<EventSystem>();
        }

        private static Image CreateImage(string name, Transform parent, Color color)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            gameObject.transform.SetParent(parent, false);
            var image = gameObject.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private static Text CreateText(string name, Transform parent, string content, int size, Color color, FontStyle style, TextAnchor alignment)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            gameObject.transform.SetParent(parent, false);
            var text = gameObject.GetComponent<Text>();
            text.font = font;
            text.text = content;
            text.fontSize = size;
            text.color = color;
            text.fontStyle = style;
            text.alignment = alignment;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Math.Max(14, size / 2);
            text.resizeTextMaxSize = size;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            return text;
        }

        private static Button CreateButton(string name, Transform parent, string label, Color normalColor, Color textColor)
        {
            var image = CreateImage(name, parent, normalColor);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.12f, 1.12f, 1.12f, 1f);
            colors.pressedColor = new Color(0.78f, 0.78f, 0.78f, 1f);
            colors.selectedColor = new Color(1.05f, 1.05f, 1.05f, 1f);
            colors.disabledColor = new Color(0.35f, 0.35f, 0.35f, 0.65f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.08f;
            button.colors = colors;

            AddOutline(button.gameObject, new Color(0f, 0f, 0f, 0.38f), new Vector2(3f, -3f));
            var text = CreateText("Label", button.transform, label, 30, textColor, FontStyle.Bold, TextAnchor.MiddleCenter);
            Stretch(text.rectTransform, 26f, 10f);
            return button;
        }

        private static void AddOutline(GameObject gameObject, Color color, Vector2 distance)
        {
            var outline = gameObject.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = distance;
            outline.useGraphicAlpha = true;
        }

        private static void Stretch(RectTransform rect, float horizontalInset = 0f, float verticalInset = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(horizontalInset, verticalInset);
            rect.offsetMax = new Vector2(-horizontalInset, -verticalInset);
        }

        private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 size)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
        }

        private static Color Hex(string value)
        {
            if (!ColorUtility.TryParseHtmlString($"#{value}", out var color))
            {
                throw new ArgumentException($"Invalid color: {value}");
            }
            return color;
        }
    }
}
