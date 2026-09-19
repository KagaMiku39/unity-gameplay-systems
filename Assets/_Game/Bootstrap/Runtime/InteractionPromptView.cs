using System;
using TMPro;
using UnityEngine;

public sealed class InteractionPromptView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text promptText;

    [SerializeField]
    private GameObject promptBackground;

    [SerializeField]
    private TMP_Text targetMarkerText;

    private Canvas rootCanvas;

    private void Awake()
    {
        if (promptText == null)
        {
            throw new InvalidOperationException(
                "InteractionPromptView requires a TMP_Text reference."
            );
        }

        if (targetMarkerText == null)
        {
            throw new InvalidOperationException(
                "InteractionPromptView requires a target marker text."
            );
        }

        if (promptBackground == null)
        {
            throw new InvalidOperationException(
                "InteractionPromptView requires a prompt background."
            );
        }

        rootCanvas = GetComponentInParent<Canvas>();

        if (rootCanvas == null)
        {
            throw new InvalidOperationException(
                "InteractionPromptView must be placed under a Canvas."
            );
        }

        Clear();
    }

    public void Render(
        string prompt,
        Vector2 targetScreenPosition
    )
    {
        promptText.text = $"[F] {prompt}";
        promptBackground.SetActive(true);
        promptText.gameObject.SetActive(true);

        RectTransform markerParent =
            targetMarkerText.rectTransform.parent as RectTransform;
        Camera uiCamera = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : rootCanvas.worldCamera;

        Vector2 localPosition;
        bool converted = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            markerParent,
            targetScreenPosition,
            uiCamera,
            out localPosition
        );

        if (converted)
        {
            targetMarkerText.rectTransform.anchoredPosition = localPosition;
            targetMarkerText.gameObject.SetActive(true);
        }
        else
        {
            targetMarkerText.gameObject.SetActive(false);
        }
    }

    public void Clear()
    {
        promptText.text = string.Empty;
        promptBackground.SetActive(false);
        promptText.gameObject.SetActive(false);
        targetMarkerText.gameObject.SetActive(false);
    }
}
