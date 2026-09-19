using System;
using TMPro;
using UnityEngine;

public sealed class QuestView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text descriptionText;

    [SerializeField]
    private TMP_Text progressText;

    [SerializeField]
    private TMP_Text statusText;

    private IEventSubscriber eventSubscriber;
    private int questId;
    private bool isInitialized;

    public void Initialize(
        IEventSubscriber eventSubscriber
    )
    {
        if (isInitialized)
        {
            throw new InvalidOperationException(
                "QuestView cannot be initialized twice."
            );
        }

        if (eventSubscriber == null)
        {
            throw new ArgumentNullException(nameof(eventSubscriber));
        }

        ValidateReferences();

        this.eventSubscriber = eventSubscriber;

        titleText.text = "Waiting for Quest";
        descriptionText.text = string.Empty;
        progressText.text = "Progress: --";
        statusText.text = "Not Started";

        eventSubscriber.Subscribe<QuestStartedEvent>(
            HandleQuestStarted
        );

        eventSubscriber.Subscribe<QuestProgressChangedEvent>(
            HandleQuestProgressChanged
        );

        eventSubscriber.Subscribe<QuestCompletedEvent>(
            HandleQuestCompleted
        );

        eventSubscriber.Subscribe<QuestChainCompletedEvent>(
            HandleQuestChainCompleted
        );

        isInitialized = true;
    }

    private void HandleQuestStarted(
        QuestStartedEvent eventData
    )
    {
        questId = eventData.QuestId;
        titleText.text = eventData.Title;
        descriptionText.text = eventData.Description;
        progressText.text = "Synchronizing progress...";
        statusText.text = "In Progress";
    }

    private void HandleQuestProgressChanged(
        QuestProgressChangedEvent eventData
    )
    {
        if (eventData.QuestId != questId)
        {
            return;
        }

        RenderProgress(
            eventData.CurrentAmount,
            eventData.RequiredAmount,
            eventData.IsCompleted
        );
    }

    private void HandleQuestCompleted(
        QuestCompletedEvent eventData
    )
    {
        if (eventData.QuestId != questId)
        {
            return;
        }

        statusText.text = "Completed";
    }

    private void HandleQuestChainCompleted(
        QuestChainCompletedEvent eventData
    )
    {
        statusText.text = "Quest Chain Completed";
    }

    private void RenderProgress(
        int currentAmount,
        int requiredAmount,
        bool isCompleted
    )
    {
        progressText.text =
            $"Progress: {currentAmount}/{requiredAmount}";

        statusText.text = isCompleted
            ? "Completed"
            : "In Progress";
    }

    private void ValidateReferences()
    {
        if (titleText == null)
        {
            throw CreateMissingReferenceException(
                nameof(titleText)
            );
        }

        if (descriptionText == null)
        {
            throw CreateMissingReferenceException(
                nameof(descriptionText)
            );
        }

        if (progressText == null)
        {
            throw CreateMissingReferenceException(
                nameof(progressText)
            );
        }

        if (statusText == null)
        {
            throw CreateMissingReferenceException(
                nameof(statusText)
            );
        }
    }

    private InvalidOperationException CreateMissingReferenceException(
        string fieldName
    )
    {
        return new InvalidOperationException(
            $"{nameof(QuestView)} is missing {fieldName}."
        );
    }

    private void OnDestroy()
    {
        if (eventSubscriber == null)
        {
            return;
        }

        eventSubscriber.Unsubscribe<QuestStartedEvent>(
            HandleQuestStarted
        );

        eventSubscriber.Unsubscribe<QuestProgressChangedEvent>(
            HandleQuestProgressChanged
        );

        eventSubscriber.Unsubscribe<QuestCompletedEvent>(
            HandleQuestCompleted
        );

        eventSubscriber.Unsubscribe<QuestChainCompletedEvent>(
            HandleQuestChainCompleted
        );
    }
}
