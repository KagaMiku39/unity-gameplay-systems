using NUnit.Framework;

public sealed class GameplayInputLockTests
{
    [Test]
    public void Acquire_WithOneOwner_BlocksGameplayInput()
    {
        GameplayInputLock inputLock = new GameplayInputLock();

        inputLock.Acquire(new object());

        Assert.That(inputLock.IsBlocked, Is.True);
        Assert.That(inputLock.OwnerCount, Is.EqualTo(1));
    }

    [Test]
    public void Release_OnlyUnlocksAfterEveryOwnerHasReleased()
    {
        GameplayInputLock inputLock = new GameplayInputLock();
        object inventoryOwner = new object();
        object cutsceneOwner = new object();

        inputLock.Acquire(inventoryOwner);
        inputLock.Acquire(cutsceneOwner);
        inputLock.Release(inventoryOwner);

        Assert.That(inputLock.IsBlocked, Is.True);

        inputLock.Release(cutsceneOwner);

        Assert.That(inputLock.IsBlocked, Is.False);
    }

    [Test]
    public void Acquire_SameOwnerTwice_DoesNotCreateDuplicateLock()
    {
        GameplayInputLock inputLock = new GameplayInputLock();
        object owner = new object();

        inputLock.Acquire(owner);
        inputLock.Acquire(owner);

        Assert.That(inputLock.OwnerCount, Is.EqualTo(1));

        inputLock.Release(owner);

        Assert.That(inputLock.IsBlocked, Is.False);
    }
}
