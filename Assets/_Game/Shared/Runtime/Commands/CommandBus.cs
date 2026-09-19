using System;
using System.Collections.Generic;

public sealed class CommandBus : ICommandSender
{
    private readonly Dictionary<Type, Delegate> handlers =
        new Dictionary<Type, Delegate>();

    public void Register<TCommand, TResult>(
        Func<TCommand, TResult> handler
    ) where TCommand : class, ICommand<TResult>
    {
        if (handler == null)
        {
            throw new ArgumentNullException
            (
                nameof(handler),
                "处理方法不能为空"
            );
        }
        Type commandType = typeof(TCommand);
        if (handlers.ContainsKey(commandType))
        {
            throw new InvalidOperationException("命令已经注册处理器");
        }
        Func<ICommand<TResult>, TResult> adaptedHandler =
        command =>
        {
            if (command is not TCommand typedCommand)
            {
                throw new InvalidOperationException(
                    $"命令类型不是 {typeof(TCommand).Name}"
                );
            }

            return handler(typedCommand);
        };
        handlers.Add(commandType, adaptedHandler);
    }

    public TResult Send<TResult>(
        ICommand<TResult> command
    )
    {
        if (command == null)
        {
            throw new ArgumentNullException
            (
                nameof(command),
                "命令不能为空"
            );
        }
        Type commandType = command.GetType();
        Delegate registeredHandler;
        if (handlers.TryGetValue(commandType, out registeredHandler) == false)
        {
            throw new InvalidOperationException($"命令没有注册处理器 {commandType.Name}");
        }
        if (registeredHandler is not Func<ICommand<TResult>, TResult> handler)
        {
            throw new InvalidOperationException("处理器返回类型不匹配");
        }
        return handler(command);
    }
}
