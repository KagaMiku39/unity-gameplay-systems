using System;
using System.Collections.Generic;

public sealed class UiPanelStack
{
    private readonly List<IUiPanel> panels =
        new List<IUiPanel>();

    public int Count => panels.Count;

    public bool IsTop(IUiPanel panel)
    {
        if (panel == null)
        {
            throw new ArgumentNullException(nameof(panel));
        }

        return panels.Count > 0 &&
            ReferenceEquals(panels[panels.Count - 1], panel);
    }

    public void Push(IUiPanel panel)
    {
        if (panel == null)
        {
            throw new ArgumentNullException(nameof(panel));
        }

        if (IsTop(panel))
        {
            return;
        }

        if (panels.Count > 0)
        {
            panels[panels.Count - 1].Hide();
        }

        int existingIndex = FindIndex(panel);

        if (existingIndex >= 0)
        {
            panels.RemoveAt(existingIndex);
        }

        panels.Add(panel);
        panel.Show();
    }

    public bool TryPop()
    {
        if (panels.Count == 0)
        {
            return false;
        }

        int topIndex = panels.Count - 1;
        IUiPanel topPanel = panels[topIndex];
        panels.RemoveAt(topIndex);
        topPanel.Hide();

        if (panels.Count > 0)
        {
            panels[panels.Count - 1].Show();
        }

        return true;
    }

    private int FindIndex(IUiPanel panel)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            if (ReferenceEquals(panels[i], panel))
            {
                return i;
            }
        }

        return -1;
    }
}
