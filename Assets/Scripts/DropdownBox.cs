using UnityEngine;
using UnityEngine.UI;

public class DropdownBox : MonoBehaviour
{
    public Text label;
    public Dropdown input;
    public DataString data;

    private void Start()
    {
        data = ScriptableObject.CreateInstance<DataString>();
        
    }
}