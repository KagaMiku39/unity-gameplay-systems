using System;
using System.Collections.Generic;

public sealed class GameplayInputLock : IGameplayInputLock
{
    private readonly HashSet<object> owners =
        new HashSet<object>();

    public bool IsBlocked => owners.Count > 0;

    public int OwnerCount => owners.Count;

    public void Acquire(object owner)
    {
        if (owner == null)
        {
            throw new ArgumentNullException(nameof(owner));
        }

        owners.Add(owner);
    }

    public void Release(object owner)
    {
        if (owner == null)
        {
            throw new ArgumentNullException(nameof(owner));
        }

        owners.Remove(owner);
    }
}
