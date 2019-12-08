using System;
using UnityEngine.UI;

public class ListInputBox : InputBox
{
    public Button buttonRemove; 
    public InputList parent;

    void Awake() {
        base.Start();
        buttonRemove.onClick.AddListener(Remove);
    }

    public void Remove()
    {
        parent.Remove(this);
        Destroy(gameObject);
    }

}