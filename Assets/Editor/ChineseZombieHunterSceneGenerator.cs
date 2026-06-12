using System;
using System.IO;
using System.Reflection;
using ChineseZombieHunter;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ChineseZombieHunter.EditorTools
{
    public static class ChineseZombieHunterSceneGenerator
    {
        private const string SceneFolder = "Assets/Generated/Scenes";
        private const string PrefabFolder = "Assets/Generated/Prefabs";
        private const string DataFolder = "Assets/Generated/Data";
        private const string ScenePath = "Assets/Generated/Scenes/ChineseZombieHunterStarter.unity";
        private const string CatalogPath = "Assets/Generated/Data/ResourceCatalog.asset";
        private const string ResourceItemPrefabPath = "Assets/Generated/Prefabs/ResourceItemView.prefab";

        [MenuItem("Tools/Chinese Zombie Hunter/Create Starter Scene")]
        public static void CreateStarterScene()
        {
            EnsureFolder("Assets", "Generated");
            EnsureFolder("Assets/Generated", "Scenes");
            EnsureFolder("Assets/Generated", "Prefabs");
            EnsureFolder("Assets/Generated", "Data");

            if (File.Exists(ScenePath))
            {
                bool overwrite = EditorUtility.DisplayDialog(
                    "Overwrite starter scene?",
                    "A generated starter scene already exists. Do you want to recreate it?",
                    "Recreate",
                    "Cancel");

                if (!overwrite)
                {
                    return;
                }
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.transform.position = new Vector3(0f, 1.5f, -8f);
            camera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            camera.clearFlags = CameraClearFlags.Skybox;
            cameraObject.tag = "MainCamera";

            GameObject lightObject = new GameObject("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            CreateEventSystem();

            GameObject canvasObject = new GameObject("Canvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject resourcesPanel = CreatePanel(canvasObject.transform, "ResourcesPanel", new Color(0.12f, 0.14f, 0.10f, 0.96f));
            GameObject stage1Panel = CreatePanel(canvasObject.transform, "Stage1Panel", new Color(0.12f, 0.10f, 0.14f, 0.00f));

            GameObject gameFlow = new GameObject("GameFlow");
            JjGamesLauncher launcher = gameFlow.AddComponent<JjGamesLauncher>();

            GameObject resourceHub = new GameObject("ResourceHub");
            ResourceHubController resourceHubController = resourceHub.AddComponent<ResourceHubController>();

            GameObject stage1Root = new GameObject("Stage1Root");
            Stage1Manager stage1Manager = stage1Root.AddComponent<Stage1Manager>();

            GameObject worldStageRoot = new GameObject("WorldStageRoot");

            BuildResourcesPanel(resourcesPanel.transform, out Transform resourceContentRoot, out Text emptyStateText, out Button stage1Button, out Button backButton, out ResourceItemView resourceItemView);
            BuildStage1Panel(stage1Panel.transform, out CanvasGroup lessonGroup, out CanvasGroup challengeGroup, out CanvasGroup resultGroup, out CharacterLessonPanel lessonPanel, out BarrelChallengePanel challengePanel, out HandwritingPracticePanel handwritingPanel, out LifeDisplay lifeDisplay, out StageResultPanel resultPanel, out Stage1Barrel barrelView);
            barrelView.transform.SetParent(worldStageRoot.transform, true);

            ResourceCatalog catalog = CreateDefaultCatalog();
            AssignSerializedReference(resourceHubController, "catalog", catalog);
            AssignSerializedReference(resourceHubController, "contentRoot", resourceContentRoot);
            AssignSerializedReference(resourceHubController, "itemPrefab", resourceItemView);
            AssignSerializedReference(resourceHubController, "emptyStateText", emptyStateText);

            AssignSerializedReference(launcher, "resourcesPanel", resourcesPanel.GetComponent<CanvasGroup>());
            AssignSerializedReference(launcher, "stage1Panel", stage1Panel.GetComponent<CanvasGroup>());
            AssignSerializedReference(launcher, "stage1Button", stage1Button);
            AssignSerializedReference(launcher, "backToResourcesButton", backButton);
            AssignSerializedReference(launcher, "stage1Manager", stage1Manager);

            AssignSerializedReference(stage1Manager, "lessonPanelGroup", lessonGroup);
            AssignSerializedReference(stage1Manager, "challengePanelGroup", challengeGroup);
            AssignSerializedReference(stage1Manager, "resultPanelGroup", resultGroup);
            AssignSerializedReference(stage1Manager, "lessonPanel", lessonPanel);
            AssignSerializedReference(stage1Manager, "challengePanel", challengePanel);
            AssignSerializedReference(stage1Manager, "handwritingPanel", handwritingPanel);
            AssignSerializedReference(stage1Manager, "lifeDisplay", lifeDisplay);
            AssignSerializedReference(stage1Manager, "resultPanel", resultPanel);
            AssignSerializedReference(stage1Manager, "barrelView", barrelView);

            EditorUtility.SetDirty(catalog);
            EditorSceneManager.MarkSceneDirty(scene);
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();

            EditorSceneManager.OpenScene(ScenePath);
            Selection.activeObject = gameFlow;
        }

        private static void BuildResourcesPanel(Transform parent, out Transform contentRoot, out Text emptyStateText, out Button stage1Button, out Button backButton, out ResourceItemView resourceItemView)
        {
            RectTransform panelRect = parent.GetComponent<RectTransform>();
            Stretch(panelRect);

            CreateText(parent, "Header", "Resource Hub", 44, FontStyle.Bold, TextAnchor.UpperCenter, Color.white).rectTransform
                .SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 48f, 100f);

            emptyStateText = CreateText(parent, "EmptyStateText", "No goodies unlocked yet.", 28, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.95f, 0.95f, 0.95f, 0.9f));
            SetAnchoredRect(emptyStateText.rectTransform, new Vector2(0.5f, 0.44f), new Vector2(0.5f, 0.44f), new Vector2(700f, 100f), Vector2.zero);

            GameObject scrollView = new GameObject("ScrollView");
            scrollView.transform.SetParent(parent, false);
            RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
            SetAnchoredRect(scrollRect, new Vector2(0.08f, 0.16f), new Vector2(0.92f, 0.38f), Vector2.zero, Vector2.zero);
            Image scrollBg = scrollView.AddComponent<Image>();
            scrollBg.color = new Color(0f, 0f, 0f, 0.20f);
            Mask mask = scrollView.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            GameObject content = new GameObject("Content");
            content.transform.SetParent(scrollView.transform, false);
            contentRoot = content.transform;
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.sizeDelta = new Vector2(0f, 0f);
            VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(20, 20, 20, 20);
            layout.spacing = 18f;
            layout.childForceExpandHeight = false;
            layout.childControlHeight = true;
            ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            stage1Button = CreateButton(parent, "Stage1Button", "Open Stage 1");
            SetAnchoredRect(stage1Button.GetComponent<RectTransform>(), new Vector2(0.25f, 0.06f), new Vector2(0.50f, 0.06f), new Vector2(320f, 88f), Vector2.zero);

            backButton = CreateButton(parent, "BackButton", "Back to Resources");
            SetAnchoredRect(backButton.GetComponent<RectTransform>(), new Vector2(0.56f, 0.06f), new Vector2(0.81f, 0.06f), new Vector2(320f, 88f), Vector2.zero);

            resourceItemView = CreateResourceItemPrefab();
        }

        private static void BuildStage1Panel(Transform parent, out CanvasGroup lessonGroup, out CanvasGroup challengeGroup, out CanvasGroup resultGroup, out CharacterLessonPanel lessonPanel, out BarrelChallengePanel challengePanel, out HandwritingPracticePanel handwritingPanel, out LifeDisplay lifeDisplay, out StageResultPanel resultPanel, out Stage1Barrel barrelView)
        {
            RectTransform panelRect = parent.GetComponent<RectTransform>();
            Stretch(panelRect);

            GameObject lessonPanelObject = CreatePanel(parent, "LessonPanel", new Color(0.16f, 0.13f, 0.08f, 0.00f));
            lessonGroup = lessonPanelObject.GetComponent<CanvasGroup>();
            lessonPanel = lessonPanelObject.AddComponent<CharacterLessonPanel>();
            BuildLessonPanel(
                lessonPanelObject.transform,
                out Text lessonCharacterText,
                out Text lessonMeaningText,
                out Text lessonPromptText,
                out Button lessonContinueButton,
                out Text lessonContinueButtonText);
            AssignSerializedReference(lessonPanel, "characterText", lessonCharacterText);
            AssignSerializedReference(lessonPanel, "meaningText", lessonMeaningText);
            AssignSerializedReference(lessonPanel, "promptText", lessonPromptText);
            AssignSerializedReference(lessonPanel, "continueButton", lessonContinueButton);
            AssignSerializedReference(lessonPanel, "continueButtonText", lessonContinueButtonText);

            GameObject challengePanelObject = CreatePanel(parent, "ChallengePanel", new Color(0.08f, 0.14f, 0.16f, 0.00f));
            challengeGroup = challengePanelObject.GetComponent<CanvasGroup>();
            challengePanel = challengePanelObject.AddComponent<BarrelChallengePanel>();
            BuildChallengePanel(
                challengePanelObject.transform,
                out Text challengePromptText,
                out Text barrelLabelText,
                out Text feedbackText,
                out Button[] answerButtons,
                out Text[] answerTexts);
            AssignSerializedReference(challengePanel, "promptText", challengePromptText);
            AssignSerializedReference(challengePanel, "barrelLabelText", barrelLabelText);
            AssignSerializedReference(challengePanel, "feedbackText", feedbackText);
            AssignSerializedReference(challengePanel, "answerButtons", answerButtons);
            AssignSerializedReference(challengePanel, "answerButtonTexts", answerTexts);

            GameObject handwritingPanelObject = CreatePanel(parent, "HandwritingPanel", new Color(0.98f, 0.97f, 0.94f, 0.96f));
            handwritingPanelObject.GetComponent<Image>().raycastTarget = false;
            RectTransform handwritingRect = handwritingPanelObject.GetComponent<RectTransform>();
            handwritingRect.anchorMin = new Vector2(0.66f, 0.20f);
            handwritingRect.anchorMax = new Vector2(0.94f, 0.74f);
            handwritingRect.offsetMin = Vector2.zero;
            handwritingRect.offsetMax = Vector2.zero;
            handwritingRect.pivot = new Vector2(0.5f, 0.5f);

            handwritingPanel = handwritingPanelObject.AddComponent<HandwritingPracticePanel>();
            BuildHandwritingPanel(
                handwritingPanelObject.transform,
                out Text handwritingTitleText,
                out Text handwritingInstructionText,
                out Text handwritingCharacterText,
                out Button handwritingClearButton,
                out Text handwritingClearButtonText,
                out HandwritingPad handwritingPad);
            AssignSerializedReference(handwritingPanel, "titleText", handwritingTitleText);
            AssignSerializedReference(handwritingPanel, "instructionText", handwritingInstructionText);
            AssignSerializedReference(handwritingPanel, "characterText", handwritingCharacterText);
            AssignSerializedReference(handwritingPanel, "clearButton", handwritingClearButton);
            AssignSerializedReference(handwritingPanel, "clearButtonText", handwritingClearButtonText);
            AssignSerializedReference(handwritingPanel, "handwritingPad", handwritingPad);

            GameObject resultPanelObject = CreatePanel(parent, "ResultPanel", new Color(0.14f, 0.08f, 0.10f, 0.00f));
            resultGroup = resultPanelObject.GetComponent<CanvasGroup>();
            resultPanel = resultPanelObject.AddComponent<StageResultPanel>();
            BuildResultPanel(
                resultPanelObject.transform,
                out Text resultTitleText,
                out Text resultMessageText,
                out Button resultPrimaryButton,
                out Text resultPrimaryButtonText);
            AssignSerializedReference(resultPanel, "titleText", resultTitleText);
            AssignSerializedReference(resultPanel, "messageText", resultMessageText);
            AssignSerializedReference(resultPanel, "primaryButton", resultPrimaryButton);
            AssignSerializedReference(resultPanel, "primaryButtonText", resultPrimaryButtonText);

            GameObject lifeObject = new GameObject("LifeDisplay");
            lifeObject.transform.SetParent(parent, false);
            lifeObject.AddComponent<RectTransform>();
            lifeDisplay = lifeObject.AddComponent<LifeDisplay>();
            BuildLifeDisplay(lifeObject.transform, out Image[] lifeIcons);
            AssignSerializedReference(lifeDisplay, "lifeIcons", lifeIcons);

            barrelView = CreateBarrelWorldObject(parent);
        }

        private static void BuildLessonPanel(Transform parent, out Text characterText, out Text meaningText, out Text promptText, out Button continueButton, out Text continueButtonText)
        {
            characterText = CreateText(parent, "CharacterText", "一", 96, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            characterText.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 100f, 180f);
            meaningText = CreateText(parent, "MeaningText", "一 = 1", 54, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(1f, 0.9f, 0.4f, 1f));
            meaningText.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 300f, 120f);
            promptText = CreateText(parent, "PromptText", "Tap to see the next one.", 28, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.98f, 0.98f, 0.98f, 0.95f));
            promptText.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 430f, 80f);

            continueButton = CreateButton(parent, "ContinueButton", "Next");
            SetAnchoredRect(continueButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0.10f), new Vector2(0.5f, 0.10f), new Vector2(320f, 88f), Vector2.zero);
            continueButtonText = continueButton.GetComponentInChildren<Text>();
        }

        private static void BuildChallengePanel(Transform parent, out Text promptText, out Text barrelLabelText, out Text feedbackText, out Button[] answerButtons, out Text[] answerTexts)
        {
            promptText = CreateText(parent, "PromptText", "Pick the matching Chinese character.", 30, FontStyle.Bold, TextAnchor.UpperCenter, Color.white);
            promptText.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 36f, 100f);
            barrelLabelText = CreateText(parent, "BarrelLabelText", "1", 78, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(1f, 0.95f, 0.4f, 1f));
            barrelLabelText.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 120f, 160f);
            feedbackText = CreateText(parent, "FeedbackText", "Choose wisely.", 28, FontStyle.Normal, TextAnchor.MiddleCenter, Color.white);
            feedbackText.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 280f, 70f);

            GameObject buttonsRoot = new GameObject("ButtonsRoot");
            buttonsRoot.transform.SetParent(parent, false);
            RectTransform buttonsRect = buttonsRoot.AddComponent<RectTransform>();
            SetAnchoredRect(buttonsRect, new Vector2(0.15f, 0.08f), new Vector2(0.85f, 0.25f), Vector2.zero, Vector2.zero);
            HorizontalLayoutGroup layout = buttonsRoot.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 22f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.padding = new RectOffset(10, 10, 10, 10);

            Button answerA = CreateButton(buttonsRoot.transform, "AnswerA", "一");
            Button answerB = CreateButton(buttonsRoot.transform, "AnswerB", "二");
            Button answerC = CreateButton(buttonsRoot.transform, "AnswerC", "三");
            answerButtons = new[] { answerA, answerB, answerC };
            answerTexts = new[]
            {
                answerA.GetComponentInChildren<Text>(),
                answerB.GetComponentInChildren<Text>(),
                answerC.GetComponentInChildren<Text>()
            };
        }

        private static void BuildResultPanel(Transform parent, out Text titleText, out Text messageText, out Button primaryButton, out Text primaryButtonText)
        {
            titleText = CreateText(parent, "TitleText", "Stage Clear!", 60, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            titleText.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 140f, 120f);
            messageText = CreateText(parent, "MessageText", "Nice work. Want to play it again?", 30, FontStyle.Normal, TextAnchor.MiddleCenter, Color.white);
            messageText.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 280f, 100f);

            primaryButton = CreateButton(parent, "PrimaryButton", "Play Again");
            SetAnchoredRect(primaryButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0.10f), new Vector2(0.5f, 0.10f), new Vector2(360f, 90f), Vector2.zero);
            primaryButtonText = primaryButton.GetComponentInChildren<Text>();
        }

        private static void BuildHandwritingPanel(Transform parent, out Text titleText, out Text instructionText, out Text characterText, out Button clearButton, out Text clearButtonText, out HandwritingPad handwritingPad)
        {
            titleText = CreateText(parent, "TitleText", "Trace It", 34, FontStyle.Bold, TextAnchor.UpperCenter, new Color(0.12f, 0.12f, 0.12f, 1f));
            titleText.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 18f, 60f);

            instructionText = CreateText(parent, "InstructionText", "Use your finger or mouse to write here.", 22, FontStyle.Normal, TextAnchor.UpperCenter, new Color(0.16f, 0.16f, 0.16f, 1f));
            instructionText.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 68f, 88f);

            characterText = CreateText(parent, "CharacterText", "一", 64, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.12f, 0.12f, 0.12f, 1f));
            characterText.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 114f, 80f);

            GameObject padObject = new GameObject("Pad");
            padObject.transform.SetParent(parent, false);
            RectTransform padRect = padObject.AddComponent<RectTransform>();
            padRect.anchorMin = new Vector2(0.06f, 0.20f);
            padRect.anchorMax = new Vector2(0.94f, 0.70f);
            padRect.offsetMin = Vector2.zero;
            padRect.offsetMax = Vector2.zero;
            handwritingPad = padObject.AddComponent<HandwritingPad>();
            RawImage rawImage = padObject.AddComponent<RawImage>();
            AssignSerializedReference(handwritingPad, "padImage", rawImage);

            clearButton = CreateButton(parent, "ClearButton", "Clear");
            SetAnchoredRect(clearButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0.08f), new Vector2(0.5f, 0.08f), new Vector2(220f, 70f), Vector2.zero);
            clearButtonText = clearButton.GetComponentInChildren<Text>();
        }

        private static void BuildLifeDisplay(Transform parent, out Image[] lifeIcons)
        {
            RectTransform rt = parent.GetComponent<RectTransform>();
            SetAnchoredRect(rt, new Vector2(0.5f, 0.94f), new Vector2(0.5f, 0.94f), new Vector2(320f, 60f), Vector2.zero);
            HorizontalLayoutGroup layout = parent.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 10f;

            lifeIcons = new Image[3];
            for (int i = 0; i < 3; i++)
            {
                GameObject icon = new GameObject($"Life{i + 1}");
                icon.transform.SetParent(parent, false);
                Image image = icon.AddComponent<Image>();
                image.color = new Color(0.95f, 0.28f, 0.35f, 1f);
                RectTransform iconRect = icon.GetComponent<RectTransform>();
                iconRect.sizeDelta = new Vector2(42f, 42f);
                lifeIcons[i] = image;
            }
        }

        private static Stage1Barrel CreateBarrelWorldObject(Transform parent)
        {
            GameObject barrelRoot = new GameObject("Stage1Barrel");
            barrelRoot.transform.SetParent(parent, false);
            barrelRoot.transform.position = new Vector3(0f, 0f, 4f);

            GameObject travelRoot = new GameObject("TravelRoot");
            travelRoot.transform.SetParent(barrelRoot.transform, false);

            GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            barrel.name = "BarrelMesh";
            barrel.transform.SetParent(travelRoot.transform, false);
            barrel.transform.localPosition = Vector3.zero;
            barrel.transform.localScale = new Vector3(1.2f, 0.8f, 1.2f);

            GameObject zombies = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            zombies.name = "ZombieGroup";
            zombies.transform.SetParent(travelRoot.transform, false);
            zombies.transform.localPosition = new Vector3(0f, 0f, -1.5f);
            zombies.transform.localScale = new Vector3(0.7f, 1.2f, 0.7f);

            GameObject labelCanvas = new GameObject("LabelCanvas");
            labelCanvas.transform.SetParent(travelRoot.transform, false);
            Canvas worldCanvas = labelCanvas.AddComponent<Canvas>();
            worldCanvas.renderMode = RenderMode.WorldSpace;
            labelCanvas.AddComponent<CanvasScaler>();
            labelCanvas.AddComponent<GraphicRaycaster>();
            RectTransform labelCanvasRect = labelCanvas.GetComponent<RectTransform>();
            labelCanvasRect.sizeDelta = new Vector2(180f, 80f);
            labelCanvasRect.localScale = Vector3.one * 0.01f;
            labelCanvasRect.localPosition = new Vector3(0f, 1.3f, 0f);

            Text labelText = CreateText(labelCanvas.transform, "LabelText", "1", 42, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            Stretch(labelText.rectTransform);

            Stage1Barrel barrelView = barrelRoot.AddComponent<Stage1Barrel>();
            AssignSerializedReference(barrelView, "travelRoot", travelRoot.transform);
            AssignSerializedReference(barrelView, "barrelRoot", barrel);
            AssignSerializedReference(barrelView, "zombieGroupRoot", zombies);
            AssignSerializedReference(barrelView, "barrelLabelText", labelText);
            AssignSerializedReference(barrelView, "moveSpeed", 0.7f);
            return barrelView;
        }

        private static ResourceItemView CreateResourceItemPrefab()
        {
            GameObject root = new GameObject("ResourceItemView");
            RectTransform rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(0f, 220f);
            Image background = root.AddComponent<Image>();
            background.color = new Color(1f, 1f, 1f, 0.10f);
            HorizontalLayoutGroup layout = root.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 18, 18);
            layout.spacing = 16f;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            GameObject icon = new GameObject("Icon");
            icon.transform.SetParent(root.transform, false);
            Image iconImage = icon.AddComponent<Image>();
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(96f, 96f);

            GameObject body = new GameObject("Body");
            body.transform.SetParent(root.transform, false);
            RectTransform bodyRect = body.AddComponent<RectTransform>();
            VerticalLayoutGroup bodyLayout = body.AddComponent<VerticalLayoutGroup>();
            bodyLayout.spacing = 8f;
            bodyLayout.childForceExpandHeight = false;
            bodyLayout.childForceExpandWidth = true;
            ContentSizeFitter bodyFitter = body.AddComponent<ContentSizeFitter>();
            bodyFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            Text title = CreateText(body.transform, "TitleText", "Character Sheet", 30, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
            Text description = CreateText(body.transform, "DescriptionText", "Practice stroke order and meaning.", 22, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.95f, 0.95f, 0.95f, 0.95f));

            Button actionButton = CreateButton(root.transform, "ActionButton", "Download");
            RectTransform actionRect = actionButton.GetComponent<RectTransform>();
            actionRect.sizeDelta = new Vector2(220f, 72f);

            ResourceItemView itemView = root.AddComponent<ResourceItemView>();
            AssignSerializedReference(itemView, "iconImage", iconImage);
            AssignSerializedReference(itemView, "titleText", title);
            AssignSerializedReference(itemView, "descriptionText", description);
            AssignSerializedReference(itemView, "actionButton", actionButton);
            AssignSerializedReference(itemView, "actionButtonText", actionButton.GetComponentInChildren<Text>());

            string prefabDirectory = Path.GetDirectoryName(ResourceItemPrefabPath);
            if (!AssetDatabase.IsValidFolder(prefabDirectory))
            {
                EnsureNestedFolder(ResourceItemPrefabPath);
            }

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, ResourceItemPrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab.GetComponent<ResourceItemView>();
        }

        private static ResourceCatalog CreateDefaultCatalog()
        {
            ResourceCatalog catalog = AssetDatabase.LoadAssetAtPath<ResourceCatalog>(CatalogPath);
            if (catalog != null)
            {
                return catalog;
            }

            ResourceCatalog createdCatalog = ScriptableObject.CreateInstance<ResourceCatalog>();
            AssetDatabase.CreateAsset(createdCatalog, CatalogPath);
            AssetDatabase.SaveAssets();
            return createdCatalog;
        }

        private static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.AddComponent<RectTransform>();
            Stretch(rect);
            Image image = panel.AddComponent<Image>();
            image.color = color;
            CanvasGroup group = panel.AddComponent<CanvasGroup>();
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
            return panel;
        }

        private static Text CreateText(Transform parent, string name, string text, int fontSize, FontStyle style, TextAnchor anchor, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Text uiText = go.AddComponent<Text>();
            uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            uiText.text = text;
            uiText.fontSize = fontSize;
            uiText.fontStyle = style;
            uiText.alignment = anchor;
            uiText.color = color;
            uiText.horizontalOverflow = HorizontalWrapMode.Wrap;
            uiText.verticalOverflow = VerticalWrapMode.Overflow;
            RectTransform rect = uiText.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(600f, 120f);
            return uiText;
        }

        private static Button CreateButton(Transform parent, string name, string label)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Image image = go.AddComponent<Image>();
            image.color = new Color(0.18f, 0.22f, 0.32f, 0.96f);
            Button button = go.AddComponent<Button>();

            ColorBlock colors = button.colors;
            colors.normalColor = image.color;
            colors.highlightedColor = new Color(0.26f, 0.31f, 0.42f, 1f);
            colors.pressedColor = new Color(0.12f, 0.16f, 0.24f, 1f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;

            Text buttonText = CreateText(go.transform, "Text", label, 28, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            Stretch(buttonText.rectTransform);
            buttonText.raycastTarget = false;
            RectTransform rect = button.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(280f, 76f);
            return button;
        }

        private static void CreateEventSystem()
        {
            if (UnityEngine.Object.FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private static void SetAnchoredRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Vector2 anchoredPosition)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.sizeDelta = sizeDelta;
            rect.anchoredPosition = anchoredPosition;
            rect.pivot = new Vector2(0.5f, 0.5f);
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void AssignSerializedReference(object target, string fieldName, object value)
        {
            if (target == null)
            {
                return;
            }

            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(target, value);
                return;
            }

            Debug.LogWarning($"Could not assign field '{fieldName}' on {target.GetType().Name}.");
        }

        private static void EnsureFolder(string parent, string folderName)
        {
            string path = $"{parent}/{folderName}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }

        private static void EnsureNestedFolder(string assetPath)
        {
            string directory = Path.GetDirectoryName(assetPath)?.Replace('\\', '/');
            if (string.IsNullOrEmpty(directory))
            {
                return;
            }

            string[] segments = directory.Split('/');
            string current = segments[0];
            for (int i = 1; i < segments.Length; i++)
            {
                string next = $"{current}/{segments[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, segments[i]);
                }

                current = next;
            }
        }
    }
}
