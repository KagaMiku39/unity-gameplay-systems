using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class QuestUiSetupTool
{
    private const string ScenePath =
        "Assets/_Game/Scenes/Development/InventoryStudy.unity";

    [MenuItem("Tools/Inventory Study/Build Quest UI")]
    public static void BuildQuestUi()
    {
        Scene scene = EditorSceneManager.OpenScene(
            ScenePath,
            OpenSceneMode.Single
        );

        Canvas canvas = UnityEngine.Object.FindObjectOfType<Canvas>(
            true
        );

        if (canvas == null)
        {
            throw new InvalidOperationException(
                $"Cannot find Canvas in scene {ScenePath}."
            );
        }

        GameBootstrapper bootstrapper =
            UnityEngine.Object.FindObjectOfType<GameBootstrapper>(
                true
            );

        if (bootstrapper == null)
        {
            throw new InvalidOperationException(
                $"Cannot find GameBootstrapper in scene {ScenePath}."
            );
        }

        GameObject panelObject = GetOrCreatePanel(
            canvas.transform
        );

        QuestView questView =
            panelObject.GetComponent<QuestView>();

        TMP_Text titleText = GetOrCreateText(
            panelObject.transform,
            "TitleText",
            "Quest Title",
            24,
            new Vector2(0f, -14f),
            new Vector2(-28f, 34f),
            FontStyles.Bold
        );

        TMP_Text descriptionText = GetOrCreateText(
            panelObject.transform,
            "DescriptionText",
            "Quest Description",
            18,
            new Vector2(0f, -54f),
            new Vector2(-28f, 56f),
            FontStyles.Normal
        );

        TMP_Text progressText = GetOrCreateText(
            panelObject.transform,
            "ProgressText",
            "Progress: 0/0",
            20,
            new Vector2(0f, -116f),
            new Vector2(-28f, 28f),
            FontStyles.Normal
        );

        TMP_Text statusText = GetOrCreateText(
            panelObject.transform,
            "StatusText",
            "In Progress",
            20,
            new Vector2(0f, -150f),
            new Vector2(-28f, 28f),
            FontStyles.Bold
        );

        statusText.color = new Color(0.65f, 0.9f, 1f, 1f);

        SerializedObject questViewObject =
            new SerializedObject(questView);

        questViewObject.FindProperty("titleText")
            .objectReferenceValue = titleText;
        questViewObject.FindProperty("descriptionText")
            .objectReferenceValue = descriptionText;
        questViewObject.FindProperty("progressText")
            .objectReferenceValue = progressText;
        questViewObject.FindProperty("statusText")
            .objectReferenceValue = statusText;
        questViewObject.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject bootstrapperObject =
            new SerializedObject(bootstrapper);

        bootstrapperObject.FindProperty("questView")
            .objectReferenceValue = questView;
        bootstrapperObject.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(questView);
        EditorUtility.SetDirty(bootstrapper);
        EditorSceneManager.MarkSceneDirty(scene);

        if (EditorSceneManager.SaveScene(scene) == false)
        {
            throw new InvalidOperationException(
                $"Failed to save scene: {ScenePath}"
            );
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Quest UI created and references configured.");
    }

    private static GameObject GetOrCreatePanel(
        Transform canvasTransform
    )
    {
        Transform existingTransform = canvasTransform.Find(
            "QuestPanel"
        );

        GameObject panelObject;

        if (existingTransform == null)
        {
            panelObject = new GameObject(
                "QuestPanel",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(QuestView)
            );

            panelObject.transform.SetParent(
                canvasTransform,
                false
            );
        }
        else
        {
            panelObject = existingTransform.gameObject;

            if (panelObject.GetComponent<Image>() == null)
            {
                panelObject.AddComponent<Image>();
            }

            if (panelObject.GetComponent<QuestView>() == null)
            {
                panelObject.AddComponent<QuestView>();
            }
        }

        RectTransform panelRect =
            panelObject.GetComponent<RectTransform>();

        panelRect.anchorMin = new Vector2(0f, 1f);
        panelRect.anchorMax = new Vector2(0f, 1f);
        panelRect.pivot = new Vector2(0f, 1f);
        panelRect.anchoredPosition = new Vector2(20f, -20f);
        panelRect.sizeDelta = new Vector2(420f, 190f);

        Image background = panelObject.GetComponent<Image>();
        background.color = new Color(0.08f, 0.1f, 0.14f, 0.88f);
        background.raycastTarget = false;

        return panelObject;
    }

    private static TMP_Text GetOrCreateText(
        Transform parent,
        string objectName,
        string initialText,
        float fontSize,
        Vector2 anchoredPosition,
        Vector2 sizeDelta,
        FontStyles fontStyle
    )
    {
        Transform existingTransform = parent.Find(objectName);
        TextMeshProUGUI text;

        if (existingTransform == null)
        {
            GameObject textObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI)
            );

            textObject.transform.SetParent(parent, false);
            text = textObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            text = existingTransform.GetComponent<TextMeshProUGUI>();

            if (text == null)
            {
                throw new InvalidOperationException(
                    $"{objectName} exists without TextMeshProUGUI."
                );
            }
        }

        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;

        text.text = initialText;
        text.font = TMP_Settings.defaultFontAsset;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.color = Color.white;
        text.enableWordWrapping = true;
        text.raycastTarget = false;

        return text;
    }
}
