using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class UiNavigationSetupTool
{
    private const string ScenePath =
        "Assets/_Game/Scenes/Development/InventoryStudy.unity";

    [MenuItem("Tools/Inventory Study/Configure UI Navigation")]
    public static void ConfigureUiNavigation()
    {
        Scene scene = EditorSceneManager.OpenScene(
            ScenePath,
            OpenSceneMode.Single
        );

        GameBootstrapper bootstrapper =
            UnityEngine.Object.FindObjectOfType<GameBootstrapper>(true);
        Canvas canvas = UnityEngine.Object.FindObjectOfType<Canvas>(true);

        if (bootstrapper == null || canvas == null)
        {
            throw new InvalidOperationException(
                "The scene requires GameBootstrapper and Canvas."
            );
        }

        UiNavigationInput navigationInput =
            bootstrapper.GetComponent<UiNavigationInput>();

        if (navigationInput == null)
        {
            navigationInput =
                bootstrapper.gameObject.AddComponent<UiNavigationInput>();
        }

        CreateOrUpdateHint(canvas.transform);

        EditorUtility.SetDirty(navigationInput);
        EditorUtility.SetDirty(bootstrapper);
        EditorSceneManager.MarkSceneDirty(scene);

        if (EditorSceneManager.SaveScene(scene) == false)
        {
            throw new InvalidOperationException(
                $"Failed to save scene: {ScenePath}"
            );
        }

        AssetDatabase.SaveAssets();
        Debug.Log("UI navigation configured: E opens inventory, Esc closes the top panel.");
    }

    private static void CreateOrUpdateHint(Transform canvasTransform)
    {
        Transform existing = canvasTransform.Find("UiControlHint");
        TextMeshProUGUI hint;

        if (existing == null)
        {
            GameObject hintObject = new GameObject(
                "UiControlHint",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI)
            );
            hintObject.transform.SetParent(canvasTransform, false);
            hint = hintObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            hint = existing.GetComponent<TextMeshProUGUI>();

            if (hint == null)
            {
                throw new InvalidOperationException(
                    "UiControlHint exists without TextMeshProUGUI."
                );
            }
        }

        RectTransform rect = hint.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 20f);
        rect.sizeDelta = new Vector2(720f, 36f);

        hint.text =
            "WASD: Move    E: Inventory    F: Interact    Esc: Close";
        hint.font = TMP_Settings.defaultFontAsset;
        hint.fontSize = 20f;
        hint.alignment = TextAlignmentOptions.Center;
        hint.color = Color.white;
        hint.raycastTarget = false;
    }
}
