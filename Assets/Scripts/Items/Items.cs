using System;

[Serializable]
public class Items
{
    public ItemData item;
    public int count;

    public Items(ItemData newItem, int newCount)
    {
        item = newItem;
        count = newCount;
    }
}
