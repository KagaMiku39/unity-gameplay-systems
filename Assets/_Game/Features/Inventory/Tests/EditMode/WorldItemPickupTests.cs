using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public sealed class WorldItemPickupTests
{
    [Test]
    public void TryCollect_WhenOnlyPartFits_LeavesRemainderAndPublishesEvent()
    {
        GameObject pickupObject = new GameObject("TestPickup");

        try
        {
            WorldItemPickup pickup =
                pickupObject.AddComponent<WorldItemPickup>();

            SetPrivateInt(pickup, "itemId", 1);
            SetPrivateInt(pickup, "amount", 5);

            EventBus eventBus = new EventBus();
            WorldItemPickedUpEvent receivedEvent = null;
            eventBus.Subscribe<WorldItemPickedUpEvent>(
                eventData => receivedEvent = eventData
            );

            pickup.Initialize(
                new FixedRemainingCommandSender(2),
                eventBus
            );

            int collectedAmount = pickup.TryCollect();

            Assert.That(collectedAmount, Is.EqualTo(3));
            Assert.That(pickup.RemainingAmount, Is.EqualTo(2));
            Assert.That(receivedEvent, Is.Not.Null);
            Assert.That(receivedEvent.ItemId, Is.EqualTo(1));
            Assert.That(receivedEvent.CollectedAmount, Is.EqualTo(3));
            Assert.That(receivedEvent.RemainingWorldAmount, Is.EqualTo(2));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(pickupObject);
        }
    }

    private static void SetPrivateInt(
        WorldItemPickup pickup,
        string fieldName,
        int value
    )
    {
        FieldInfo field = typeof(WorldItemPickup).GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        if (field == null)
        {
            throw new InvalidOperationException(
                $"找不到 WorldItemPickup.{fieldName} 字段。"
            );
        }

        field.SetValue(pickup, value);
    }

    private sealed class FixedRemainingCommandSender : ICommandSender
    {
        private readonly int remaining;

        public FixedRemainingCommandSender(int remaining)
        {
            this.remaining = remaining;
        }

        public TResult Send<TResult>(ICommand<TResult> command)
        {
            if (command is not AddItemCommand)
            {
                throw new InvalidOperationException(
                    "测试发送器只接受 AddItemCommand。"
                );
            }

            return (TResult)(object)remaining;
        }
    }
}
