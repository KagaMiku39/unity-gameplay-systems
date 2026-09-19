using NUnit.Framework;

public sealed class UiPanelStackTests
{
    [Test]
    public void Push_SecondPanel_HidesFirstPanel()
    {
        UiPanelStack stack = new UiPanelStack();
        FakePanel inventory = new FakePanel();
        FakePanel shop = new FakePanel();

        stack.Push(inventory);
        stack.Push(shop);

        Assert.That(inventory.IsVisible, Is.False);
        Assert.That(shop.IsVisible, Is.True);
        Assert.That(stack.IsTop(shop), Is.True);
        Assert.That(stack.Count, Is.EqualTo(2));
    }

    [Test]
    public void TryPop_RevealsPreviousPanel()
    {
        UiPanelStack stack = new UiPanelStack();
        FakePanel inventory = new FakePanel();
        FakePanel shop = new FakePanel();

        stack.Push(inventory);
        stack.Push(shop);

        bool closed = stack.TryPop();

        Assert.That(closed, Is.True);
        Assert.That(shop.IsVisible, Is.False);
        Assert.That(inventory.IsVisible, Is.True);
        Assert.That(stack.IsTop(inventory), Is.True);
        Assert.That(stack.Count, Is.EqualTo(1));
    }

    [Test]
    public void Push_ExistingPanel_MovesItToTopWithoutDuplicate()
    {
        UiPanelStack stack = new UiPanelStack();
        FakePanel inventory = new FakePanel();
        FakePanel shop = new FakePanel();

        stack.Push(inventory);
        stack.Push(shop);
        stack.Push(inventory);

        Assert.That(stack.Count, Is.EqualTo(2));
        Assert.That(stack.IsTop(inventory), Is.True);
        Assert.That(inventory.IsVisible, Is.True);
        Assert.That(shop.IsVisible, Is.False);
    }

    private sealed class FakePanel : IUiPanel
    {
        public bool IsVisible { get; private set; }

        public void Show()
        {
            IsVisible = true;
        }

        public void Hide()
        {
            IsVisible = false;
        }
    }
}
