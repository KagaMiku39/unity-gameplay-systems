using System;
using UnityEngine;

public sealed class ShopController : MonoBehaviour
{
    private ICommandSender commandSender;
    private bool isInitialized;

    public void Initialize(ICommandSender commandSender)
    {
        if (isInitialized)
        {
            throw new InvalidOperationException(
                "ShopController 不能重复初始化。"
            );
        }

        this.commandSender = commandSender
            ?? throw new ArgumentNullException(nameof(commandSender));
        isInitialized = true;
    }

    public ShopTransactionResult Execute(
        ICommand<ShopTransactionResult> command
    )
    {
        EnsureInitialized();
        return commandSender.Send(command);
    }

    private void EnsureInitialized()
    {
        if (isInitialized == false)
        {
            throw new InvalidOperationException(
                "ShopController 尚未初始化。"
            );
        }
    }
}
