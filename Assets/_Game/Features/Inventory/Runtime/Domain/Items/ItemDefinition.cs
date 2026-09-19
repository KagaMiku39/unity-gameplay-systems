using System;

[Serializable]
public class ItemDefinition
{
    public int id;
    public string name;
    public string description;
    public string iconPath;
    public ItemType itemType;
    public bool isStackable;
    public int maxStack;
    public int buyPrice;
    public int sellPrice;

    public void ValidateOrThrow()
    {
        if (id <= 0)
        {
            throw new InvalidOperationException("非法ID");
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("非法名字");
        }
        if (string.IsNullOrWhiteSpace(iconPath))
        {
            throw new InvalidOperationException("非法图标路径");
        }
        if (itemType == ItemType.None)
        {
            throw new InvalidOperationException("非法物品类别");
        }
        if (maxStack <= 0)
        {
            throw new InvalidOperationException("非法最大堆叠");
        }
        if (
          (isStackable == false && maxStack != 1) ||
          (isStackable == true && maxStack <= 1)
        )
        {
            throw new InvalidOperationException("非法堆叠");
        }
        if (buyPrice <= 0)
        {
            throw new InvalidOperationException("非法购买价格");
        }
        if (sellPrice < 0 || sellPrice > buyPrice)
        {
            throw new InvalidOperationException("非法出售价格");
        }
    }
}
