using UnityEngine;
using System.Collections;
/// <summary>
/// 武器类
/// </summary>
public class Task : Item
{
    public int Damage { get; set; }//伤害

    public Task(int id, string name, ItemType type, ItemQuality quality, string description, int capaticy, string sprite)
        : base(id, name, type, quality, description, capaticy, sprite)
    {
    }
    /// <summary>
    /// 任务用品
    /// </summary>

    //对父方法Item.GetToolTipText()进行重写
    public override string GetToolTipText()
    {
        string text = base.GetToolTipText();//调用父类的GetToolTipText()方法
        string newText = string.Format("{0}",text);
        return newText;
    }
}
