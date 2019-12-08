using System;
using UnityEngine.UI;

public class ListInputBox : InputBox
{
    public Button buttonRemove; 
    public InputList parent;

    public override void Awake() {
        base.Awake();
        buttonRemove.onClick.AddListener(Remove);
    }

    public void Remove()
    {
        parent.Remove(this);
        Destroy(gameObject);
    }

}