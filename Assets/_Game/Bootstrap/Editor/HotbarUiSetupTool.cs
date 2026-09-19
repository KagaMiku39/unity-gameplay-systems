using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class HotbarUiSetupTool
{
    private const string ScenePath =
        "Assets/_Game/Scenes/Development/InventoryStudy.unity";
    private const string SlotPrefabPath =
        "Assets/_Game/Features/Inventory/Content/Prefabs/Slot.prefab";
    private const int DefaultSlotCount = 5;

    [MenuItem("Tools/Inventory Study/Build Hotbar UI")]
    public static void BuildHotbarUi()
    {
        ConfigureSlotPrefab();

        Scene scene = EditorSceneManager.OpenScene(
            ScenePath,
            OpenSceneMode.Single
        );

        Canvas canvas = UnityEngine.Object.FindObjectOfType<Canvas>(true);
        GameBootstrapper bootstrapper =
            UnityEngine.Object.FindObjectOfType<GameBootstrapper>(true);
        InventorySlotView slotPrefab =
            AssetDatabase.LoadAssetAtPath<InventorySlotView>(
                SlotPrefabPath
            );

        if (canvas == null || bootstrapper == null || slotPrefab == null)
        {
            throw new InvalidOperationException(
                "Hotbar setup requires Canvas, GameBootstrapper and Slot prefab."
            );
        }

        GameObject hotbarRoot = GetOrCreateHotbarRoot(
            canvas.transform
        );
        HotbarController hotbarController =
            GetOrAdd<HotbarController>(hotbarRoot);
        HotbarView hotbarView = GetOrAdd<HotbarView>(hotbarRoot);

        SerializedObject controllerObject =
            new SerializedObject(hotbarController);
        controllerObject.FindProperty("visibleSlotCount").intValue =
            DefaultSlotCount;
        controllerObject.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject viewObject = new SerializedObject(hotbarView);
        SetReference(
            viewObject,
            "content",
            hotbarRoot.GetComponent<RectTransform>()
        );
        SetReference(viewObject, "slotPrefab", slotPrefab);
        viewObject.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject bootstrapperObject =
            new SerializedObject(bootstrapper);
        SetReference(
            bootstrapperObject,
            "hotbarController",
            hotbarController
        );
        SetReference(
            bootstrapperObject,
            "hotbarView",
            hotbarView
        );
        bootstrapperObject.ApplyModifiedPropertiesWithoutUndo();

        RepositionHudElements(canvas.transform);

        EditorUtility.SetDirty(hotbarController);
        EditorUtility.SetDirty(hotbarView);
        EditorUtility.SetDirty(bootstrapper);
        EditorSceneManager.MarkSceneDirty(scene);

        if (EditorSceneManager.SaveScene(scene) == false)
        {
            throw new InvalidOperationException(
                $"Failed to save scene: {ScenePath}"
            );
        }

        AssetDatabase.SaveAssets();
        Debug.Log(
            "Hotbar configured: slots 1-5 select the first five inventory slots."
        );
    }

    private static GameObject GetOrCreateHotbarRoot(Transform canvas)
    {
        Transform existing = canvas.Find("HotbarRoot");
        GameObject root;

        if (existing == null)
        {
            root = new GameObject(
                "HotbarRoot",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(HorizontalLayoutGroup),
                typeof(HotbarController),
                typeof(HotbarView)
            );
            root.transform.SetParent(canvas, false);
        }
        else
        {
            root = existing.gameObject;
            GetOrAdd<Image>(root);
            GetOrAdd<HorizontalLayoutGroup>(root);
            GetOrAdd<HotbarController>(root);
            GetOrAdd<HotbarView>(root);
        }

        RectTransform rect = root.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 16f);
        rect.sizeDelta = new Vector2(
            DefaultSlotCount * 100f +
            (DefaultSlotCount - 1) * 8f + 24f,
            124f
        );

        Image background = root.GetComponent<Image>();
        background.color = new Color(0.03f, 0.04f, 0.06f, 0.82f);
        background.raycastTarget = false;

        HorizontalLayoutGroup layout =
            root.GetComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 12, 12);
        layout.spacing = 8f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        root.SetActive(true);
        return root;
    }

    private static void ConfigureSlotPrefab()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(
            SlotPrefabPath
        );

        try
        {
            InventorySlotView slotView =
                root.GetComponent<InventorySlotView>();

            if (slotView == null)
            {
                throw new InvalidOperationException(
                    "Slot prefab requires InventorySlotView."
                );
            }

            TextMeshProUGUI shortcutText =
                GetOrCreateShortcutText(root.transform);
            SerializedObject slotObject = new SerializedObject(slotView);
            SetReference(slotObject, "shortcutText", shortcutText);
            slotObject.ApplyModifiedPropertiesWithoutUndo();

            foreach (
                TextMeshProUGUI text in
                root.GetComponentsInChildren<TextMeshProUGUI>(true)
            )
            {
                text.raycastTarget = false;
            }

            Image background = root.GetComponent<Image>();

            if (background != null)
            {
                background.raycastTarget = true;
            }

            PrefabUtility.SaveAsPrefabAsset(root, SlotPrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static TextMeshProUGUI GetOrCreateShortcutText(
        Transform parent
    )
    {
        Transform existing = parent.Find("ShortcutText");
        TextMeshProUGUI text;

        if (existing == null)
        {
            GameObject textObject = new GameObject(
                "ShortcutText",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI)
            );
            textObject.transform.SetParent(parent, false);
            text = textObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            text = existing.GetComponent<TextMeshProUGUI>();

            if (text == null)
            {
                throw new InvalidOperationException(
                    "ShortcutText exists without TextMeshProUGUI."
                );
            }
        }

        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(7f, -5f);
        rect.sizeDelta = new Vector2(26f, 26f);

        text.text = string.Empty;
        text.font = TMP_Settings.defaultFontAsset;
        text.fontSize = 18f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.color = Color.black;
        text.raycastTarget = false;
        return text;
    }

    private static void RepositionHudElements(Transform canvas)
    {
        Transform hintTransform = canvas.Find("UiControlHint");

        if (hintTransform != null)
        {
            TextMeshProUGUI hint =
                hintTransform.GetComponent<TextMeshProUGUI>();

            if (hint != null)
            {
                hint.rectTransform.anchoredPosition =
                    new Vector2(0f, 148f);
                hint.text =
                    "WASD: Move    1-5: Hotbar    E: Inventory    " +
                    "F: Interact    Esc: Close";
            }
        }

        Transform promptRoot = canvas.Find("InteractionPromptRoot");

        if (promptRoot == null)
        {
            return;
        }

        RepositionBottomElement(
            promptRoot.Find("PromptBackground"),
            198f
        );
        RepositionBottomElement(
            promptRoot.Find("PromptText"),
            198f
        );
    }

    private static void RepositionBottomElement(
        Transform element,
        float y
    )
    {
        if (element == null)
        {
            return;
        }

        RectTransform rect = element.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchoredPosition = new Vector2(
                rect.anchoredPosition.x,
                y
            );
        }
    }

    private static T GetOrAdd<T>(GameObject target)
        where T : Component
    {
        T component = target.GetComponent<T>();
        return component != null
            ? component
            : target.AddComponent<T>();
    }

    private static void SetReference(
        SerializedObject serializedObject,
        string propertyName,
        UnityEngine.Object value
    )
    {
        SerializedProperty property =
            serializedObject.FindProperty(propertyName);

        if (property == null)
        {
            throw new InvalidOperationException(
                $"Cannot find serialized property {propertyName}."
            );
        }

        property.objectReferenceValue = value;
    }
}
