using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class RayInteractionSetupTool
{
    private const string ScenePath =
        "Assets/_Game/Scenes/Development/InventoryStudy.unity";

    [MenuItem("Tools/Inventory Study/Configure Ray Interaction")]
    public static void ConfigureRayInteraction()
    {
        Scene scene = EditorSceneManager.OpenScene(
            ScenePath,
            OpenSceneMode.Single
        );

        GameObject player = GameObject.Find("Player");
        Canvas canvas = UnityEngine.Object.FindObjectOfType<Canvas>(true);
        ShopView shopView =
            UnityEngine.Object.FindObjectOfType<ShopView>(true);

        if (player == null || canvas == null || shopView == null)
        {
            throw new InvalidOperationException(
                "Build the pickup demo and shop UI before ray interaction."
            );
        }

        Transform rayOrigin = GetOrCreateRayOrigin(player.transform);
        InteractionPromptView promptView =
            GetOrCreatePromptView(canvas.transform);
        PlayerRayInteractor interactor =
            GetOrAdd<PlayerRayInteractor>(player);

        SerializedObject interactorObject =
            new SerializedObject(interactor);
        SetReference(interactorObject, "rayOrigin", rayOrigin);
        SetReference(interactorObject, "promptView", promptView);
        SetReference(interactorObject, "worldCamera", Camera.main);
        interactorObject.FindProperty("interactionDistance").floatValue =
            2.5f;
        interactorObject.FindProperty("interactableLayers").intValue = -1;
        interactorObject.ApplyModifiedPropertiesWithoutUndo();

        CreateOrUpdateShopKeeper();
        UpdateControlHint(canvas.transform);

        EditorUtility.SetDirty(interactor);
        EditorUtility.SetDirty(promptView);
        EditorSceneManager.MarkSceneDirty(scene);

        if (EditorSceneManager.SaveScene(scene) == false)
        {
            throw new InvalidOperationException(
                $"Failed to save scene: {ScenePath}"
            );
        }

        AssetDatabase.SaveAssets();
        Debug.Log(
            "Ray interaction configured: face a pickup or shop keeper and press F."
        );
    }

    private static Transform GetOrCreateRayOrigin(Transform player)
    {
        Transform origin = player.Find("InteractionOrigin");

        if (origin == null)
        {
            GameObject originObject = new GameObject("InteractionOrigin");
            originObject.transform.SetParent(player, false);
            origin = originObject.transform;
        }

        origin.localPosition = new Vector3(0f, -0.35f, 0.65f);
        origin.localRotation = Quaternion.identity;
        return origin;
    }

    private static InteractionPromptView GetOrCreatePromptView(
        Transform canvas
    )
    {
        Transform existingRoot = canvas.Find("InteractionPromptRoot");
        GameObject root;

        if (existingRoot == null)
        {
            root = new GameObject(
                "InteractionPromptRoot",
                typeof(RectTransform),
                typeof(InteractionPromptView)
            );
            root.transform.SetParent(canvas, false);
        }
        else
        {
            root = existingRoot.gameObject;

            if (root.GetComponent<InteractionPromptView>() == null)
            {
                root.AddComponent<InteractionPromptView>();
            }
        }

        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.anchoredPosition = Vector2.zero;
        rootRect.sizeDelta = Vector2.zero;

        GameObject promptBackground =
            GetOrCreatePromptBackground(root.transform);
        TextMeshProUGUI text = GetOrCreatePromptText(root.transform);
        TextMeshProUGUI targetMarker =
            GetOrCreateTargetMarkerText(root.transform);

        promptBackground.transform.SetAsFirstSibling();
        text.transform.SetAsLastSibling();
        targetMarker.transform.SetAsLastSibling();

        InteractionPromptView promptView =
            root.GetComponent<InteractionPromptView>();
        SerializedObject promptObject = new SerializedObject(promptView);
        SetReference(promptObject, "promptText", text);
        SetReference(
            promptObject,
            "promptBackground",
            promptBackground
        );
        SetReference(promptObject, "targetMarkerText", targetMarker);
        promptObject.ApplyModifiedPropertiesWithoutUndo();
        return promptView;
    }

    private static GameObject GetOrCreatePromptBackground(
        Transform parent
    )
    {
        Transform existing = parent.Find("PromptBackground");
        GameObject background;

        if (existing == null)
        {
            background = new GameObject(
                "PromptBackground",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );
            background.transform.SetParent(parent, false);
        }
        else
        {
            background = existing.gameObject;

            if (background.GetComponent<Image>() == null)
            {
                background.AddComponent<Image>();
            }
        }

        RectTransform rect = background.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, 88f);
        rect.sizeDelta = new Vector2(780f, 68f);

        Image image = background.GetComponent<Image>();
        image.color = new Color(0.03f, 0.04f, 0.06f, 0.82f);
        image.raycastTarget = false;
        background.SetActive(false);
        return background;
    }

    private static TextMeshProUGUI GetOrCreatePromptText(
        Transform parent
    )
    {
        Transform existing = parent.Find("PromptText");
        TextMeshProUGUI text;

        if (existing == null)
        {
            GameObject textObject = new GameObject(
                "PromptText",
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
        }

        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, 88f);
        rect.sizeDelta = new Vector2(740f, 60f);

        text.text = string.Empty;
        text.font = TMP_Settings.defaultFontAsset;
        text.fontSize = 30f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private static TextMeshProUGUI GetOrCreateTargetMarkerText(
        Transform parent
    )
    {
        Transform existing = parent.Find("TargetMarkerText");
        TextMeshProUGUI text;

        if (existing == null)
        {
            GameObject textObject = new GameObject(
                "TargetMarkerText",
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
        }

        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(48f, 48f);

        text.text = "+";
        text.font = TMP_Settings.defaultFontAsset;
        text.fontSize = 34f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(1f, 0.82f, 0.2f, 1f);
        text.raycastTarget = false;
        text.gameObject.SetActive(false);
        return text;
    }

    private static void CreateOrUpdateShopKeeper()
    {
        GameObject shopKeeper = GameObject.Find("ShopKeeper");

        if (shopKeeper == null)
        {
            shopKeeper = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shopKeeper.name = "ShopKeeper";
        }

        shopKeeper.transform.position = new Vector3(4f, 1f, 0f);
        shopKeeper.transform.localScale = new Vector3(1f, 2f, 1f);

        GetOrAdd<ShopInteractable>(shopKeeper);
        EditorUtility.SetDirty(shopKeeper);
    }

    private static void UpdateControlHint(Transform canvas)
    {
        Transform hint = canvas.Find("UiControlHint");

        if (hint == null)
        {
            return;
        }

        TMP_Text text = hint.GetComponent<TMP_Text>();

        if (text != null)
        {
            text.text =
                "WASD: Move    E: Inventory    F: Interact    Esc: Close";
            text.rectTransform.sizeDelta = new Vector2(720f, 36f);
            EditorUtility.SetDirty(text);
        }
    }

    private static T GetOrAdd<T>(GameObject gameObject)
        where T : Component
    {
        T component = gameObject.GetComponent<T>();

        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }

        return component;
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
                $"Cannot find serialized field {propertyName}."
            );
        }

        property.objectReferenceValue = value;
    }
}
