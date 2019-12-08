using UnityEngine;
using UnityEngine.UI;

public class InputBox : MonoBehaviour
{
    public Text label; // NOTE: May be null!
    public InputField input;
    public DataString data;
    
    private string dataBuffer = "";

    public virtual void Awake()
    {
        data = ScriptableObject.CreateInstance<DataString>();
    }

    internal void SetData(string inputData)
    {
        dataBuffer = inputData;
    }

    private void Update()
    {
        if (input.isFocused && input.text != "")
        {
            SetData(input.text);
        }
        else
        {
            input.text = data.data; 
        }

        if(dataBuffer != "")
        {
            data.data = dataBuffer;
            dataBuffer = "";
        }
    }
}
