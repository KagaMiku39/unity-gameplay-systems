using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ShopUiSetupTool
{
    private const string ScenePath =
        "Assets/_Game/Scenes/Development/InventoryStudy.unity";

    [MenuItem("Tools/Inventory Study/Build Shop UI")]
    public static void BuildShopUi()
    {
        Scene scene = EditorSceneManager.OpenScene(
            ScenePath,
            OpenSceneMode.Single
        );

        Canvas canvas = UnityEngine.Object.FindObjectOfType<Canvas>(
            true
        );
        GameBootstrapper bootstrapper =
            UnityEngine.Object.FindObjectOfType<GameBootstrapper>(true);

        if (canvas == null || bootstrapper == null)
        {
            throw new InvalidOperationException(
                "The scene requires Canvas and GameBootstrapper."
            );
        }

        ShopController shopController =
            bootstrapper.GetComponent<ShopController>();

        if (shopController == null)
        {
            shopController =
                bootstrapper.gameObject.AddComponent<ShopController>();
        }

        GameObject panelObject = GetOrCreatePanel(canvas.transform);
        ShopView shopView = panelObject.GetComponent<ShopView>();

        TMP_Text titleText = CreateText(
            panelObject.transform,
            "ShopTitleText",
            "Farm Shop",
            new Vector2(0f, -12f),
            new Vector2(-28f, 34f),
            24f,
            FontStyles.Bold
        );

        Image itemIcon = CreateImage(
            panelObject.transform,
            "ItemIcon",
            new Vector2(24f, -58f),
            new Vector2(78f, 78f)
        );

        TMP_Text productNameText = CreateText(
            panelObject.transform,
            "ProductNameText",
            "Apple",
            new Vector2(48f, -58f),
            new Vector2(-130f, 34f),
            22f,
            FontStyles.Bold
        );

        TMP_Text priceText = CreateText(
            panelObject.transform,
            "PriceText",
            "Buy: 5  Sell: 2",
            new Vector2(48f, -94f),
            new Vector2(-130f, 30f),
            18f,
            FontStyles.Normal
        );

        TMP_Text balanceText = CreateText(
            panelObject.transform,
            "BalanceText",
            "Gold: 200",
            new Vector2(0f, -142f),
            new Vector2(-28f, 30f),
            20f,
            FontStyles.Bold
        );

        TMP_Text statusText = CreateText(
            panelObject.transform,
            "ShopStatusText",
            "Choose Buy or Sell",
            new Vector2(0f, -174f),
            new Vector2(-28f, 30f),
            17f,
            FontStyles.Normal
        );

        Button previousButton = CreateButton(
            panelObject.transform,
            "PreviousButton",
            "<",
            new Vector2(20f, -218f),
            new Vector2(52f, 36f)
        );
        Button buyButton = CreateButton(
            panelObject.transform,
            "BuyButton",
            "Buy",
            new Vector2(84f, -218f),
            new Vector2(100f, 36f)
        );
        Button sellButton = CreateButton(
            panelObject.transform,
            "SellButton",
            "Sell",
            new Vector2(196f, -218f),
            new Vector2(100f, 36f)
        );
        Button nextButton = CreateButton(
            panelObject.transform,
            "NextButton",
            ">",
            new Vector2(308f, -218f),
            new Vector2(52f, 36f)
        );

        SerializedObject viewObject = new SerializedObject(shopView);
        SetReference(viewObject, "itemIcon", itemIcon);
        SetReference(viewObject, "productNameText", productNameText);
        SetReference(viewObject, "priceText", priceText);
        SetReference(viewObject, "balanceText", balanceText);
        SetReference(viewObject, "statusText", statusText);
        SetReference(viewObject, "previousButton", previousButton);
        SetReference(viewObject, "nextButton", nextButton);
        SetReference(viewObject, "buyButton", buyButton);
        SetReference(viewObject, "sellButton", sellButton);
        viewObject.FindProperty("shopId").intValue = 1;
        viewObject.FindProperty("transactionAmount").intValue = 1;
        viewObject.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject bootstrapperObject =
            new SerializedObject(bootstrapper);
        SetReference(
            bootstrapperObject,
            "shopController",
            shopController
        );
        SetReference(bootstrapperObject, "shopView", shopView);
        bootstrapperObject.FindProperty("initialGold").intValue = 200;
        bootstrapperObject.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(shopController);
        EditorUtility.SetDirty(shopView);
        EditorUtility.SetDirty(bootstrapper);
        EditorSceneManager.MarkSceneDirty(scene);

        if (EditorSceneManager.SaveScene(scene) == false)
        {
            throw new InvalidOperationException(
                $"Failed to save scene: {ScenePath}"
            );
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Shop UI created and references configured.");
    }

    private static GameObject GetOrCreatePanel(Transform parent)
    {
        Transform existing = parent.Find("ShopPanel");
        GameObject panel;

        if (existing == null)
        {
            panel = new GameObject(
                "ShopPanel",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(ShopView)
            );
            panel.transform.SetParent(parent, false);
        }
        else
        {
            panel = existing.gameObject;

            if (panel.GetComponent<Image>() == null)
            {
                panel.AddComponent<Image>();
            }

            if (panel.GetComponent<ShopView>() == null)
            {
                panel.AddComponent<ShopView>();
            }
        }

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-20f, -20f);
        rect.sizeDelta = new Vector2(380f, 270f);

        Image background = panel.GetComponent<Image>();
        background.color = new Color(0.12f, 0.09f, 0.05f, 0.9f);
        background.raycastTarget = true;

        return panel;
    }

    private static TMP_Text CreateText(
        Transform parent,
        string name,
        string content,
        Vector2 anchoredPosition,
        Vector2 sizeDelta,
        float fontSize,
        FontStyles style
    )
    {
        Transform existing = parent.Find(name);
        TextMeshProUGUI text;

        if (existing == null)
        {
            GameObject textObject = new GameObject(
                name,
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
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;

        text.text = content;
        text.font = TMP_Settings.defaultFontAsset;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private static Image CreateImage(
        Transform parent,
        string name,
        Vector2 anchoredPosition,
        Vector2 sizeDelta
    )
    {
        Transform existing = parent.Find(name);
        Image image;

        if (existing == null)
        {
            GameObject imageObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );
            imageObject.transform.SetParent(parent, false);
            image = imageObject.GetComponent<Image>();
        }
        else
        {
            image = existing.GetComponent<Image>();
        }

        RectTransform rect = image.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
        image.color = Color.white;
        image.raycastTarget = false;
        return image;
    }

    private static Button CreateButton(
        Transform parent,
        string name,
        string label,
        Vector2 anchoredPosition,
        Vector2 sizeDelta
    )
    {
        Transform existing = parent.Find(name);
        Button button;

        if (existing == null)
        {
            GameObject buttonObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(Button)
            );
            buttonObject.transform.SetParent(parent, false);
            button = buttonObject.GetComponent<Button>();
        }
        else
        {
            button = existing.GetComponent<Button>();
        }

        RectTransform rect = button.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;

        Image background = button.GetComponent<Image>();
        background.color = new Color(0.78f, 0.55f, 0.22f, 1f);

        TMP_Text buttonText = CreateText(
            button.transform,
            "Label",
            label,
            Vector2.zero,
            Vector2.zero,
            18f,
            FontStyles.Bold
        );
        RectTransform textRect = buttonText.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = Vector2.zero;
        buttonText.alignment = TextAlignmentOptions.Center;

        return button;
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
