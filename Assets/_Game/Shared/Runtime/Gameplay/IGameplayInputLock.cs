public interface IGameplayInputLock : IGameplayInputGate
{
    void Acquire(object owner);

    void Release(object owner);
}
