public interface ICommandSender
{
    TResult Send<TResult>(ICommand<TResult> command);
}
