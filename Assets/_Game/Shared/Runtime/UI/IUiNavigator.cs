public interface IUiNavigator
{
    bool TryOpenRootPanel(IUiPanel panel);

    bool TryCloseTopPanel();
}
