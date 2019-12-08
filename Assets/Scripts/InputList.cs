using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputList : MonoBehaviour
{
    public Text label;
    public List<ListInputBox> data;

    public Button buttonAdd;

    public GameObject inputTemplate;
    public RectTransform content;

    void Start()
    {
        buttonAdd.onClick.AddListener(AddItem);
    }

    public ListInputBox Add(string input = "")
    {
        var inputBox = Instantiate(inputTemplate, content);
        var inputItem = inputBox.GetComponent<ListInputBox>();
        if (inputItem)
        {
            inputItem.parent = this;
            inputItem.SetData(input);
            data.Add(inputItem);
        }
        else
        {
            Destroy(inputBox);
            Debug.Log("ListInputBox script not found in the template instance.");
        }

        return inputItem;
    }

    public void AddItem()
    {
        Add();
    }

    public void Remove(ListInputBox item)
    {
        data.Remove(item);
    }

    public void Clear()
    {
        if (data.Count > 0)
        {
            for (int i = data.Count - 1; i >= 0; i--)
            {
                data[i].Remove();
            }
        }
    }
}
