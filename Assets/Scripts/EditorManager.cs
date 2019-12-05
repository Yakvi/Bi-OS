using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    public GameObject editorOutput, inputObjectTemplate;

    public Dropdown languageDropdown;
    public InputField IDField;
    public Button prevPromptButton, nextPromptButton;

    public GameLocale gameLang;

    Locale currentLocale;
    Prompt currentPrompt;

    [SerializeField]
    List<Field> fieldList = new List<Field>();

    void Start()
    {
        // NOTE: Set language
        var languageArray = Enum.GetNames(typeof(GameLocale));
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(new List<string>(languageArray));
        
        // NOTE: Load data
        LoadLocale(gameLang.ToString(), "prompts");
        LoadPrompt(0);

        prevPromptButton.onClick.AddListener(LoadPrevPrompt);
        nextPromptButton.onClick.AddListener(LoadNextPrompt);
    }

    void ClearFields()
    {
        if (fieldList.Count > 0)
        {
            foreach (var item in fieldList)
            {
                Destroy(item.inst.gameObject);
            }
            fieldList.Clear();
        }
    }

    void AddField(string name, string value)
    {
        var field = new Field();
        field.inst = Instantiate(inputObjectTemplate, editorOutput.transform);
        field.inst.GetComponentInChildren<Text>().text = name; // Input field title
        field.inst.GetComponentInChildren<InputField>().SetTextWithoutNotify(value);
        field.inst.gameObject.name = name + currentPrompt.id; // NOTE: debug
        
        field.source = name; 
        field.value = value;
        fieldList.Add(field);
    }

    void ShowFields()
    {
        ClearFields();
        AddField("prompt", currentPrompt.prompt);
        IDField.text = currentPrompt.id.ToString();

    }

    private void Update()
    {
        if (languageDropdown.value != (int) gameLang)
        {
            gameLang = (GameLocale) languageDropdown.value;
            LoadLocale(gameLang.ToString(), "prompts");
            ReloadPrompt();
        }

        // // NOTE: State check
        // consoleInput.gameObject.SetActive(awaitingPrompt);
        // if (!consoleInput.isFocused) consoleInput.ActivateInputField();
        // if (gameLang.ToString() != currentLocale.language) 

        // // NOTE: Input processing
        // if (field.text != "" &&
        //     (Input.GetKeyDown(KeyCode.Return) || 
        //      Input.GetKeyDown(KeyCode.KeypadEnter) || 
        //      !field.isFocused)
        //      && field.text != currentPrompt.somefield.value )
        // {
        //     SaveInput(Field.text, TODO: enum???  );
        // }
    }

    void LoadLocale(string lang, string source)
    {
        var promptsJson = Resources.Load<TextAsset>(source);
        var collection = JsonUtility.FromJson<PromptCollection>(promptsJson.text);

        if (collection != null)
        {
            // NOTE: We could have just a simple index lookup but we want to 
            // potentially allow location of different locales in different files
            foreach (var loc in collection.locales)
            {
                if (loc.language == lang)
                {
                    currentLocale = loc;
                    return;
                }
            }
        }
    }

    void ReloadPrompt() { LoadPrompt(currentPrompt.id); }
    void LoadNextPrompt() { LoadPrompt(currentPrompt.id + 1); }
    void LoadPrevPrompt() { LoadPrompt(currentPrompt.id - 1); }
    void LoadPrompt(int id)
    {
        Prompt result = currentPrompt;

        if (id >= 0 && id < currentLocale.prompts.Length)
        {
            result = currentLocale.prompts[id];
            if (result.id != id) Debug.LogError("Wrong prompt loaded! Expected prompt ${id}, loaded prompt ${result.id}.");
        }

        if (result != null)
        {
            // TODO: decipher and load the whole prompt
            Enum.TryParse(result.type, out Message.MessageType type);
        }

        currentPrompt = result;
        ShowFields();
    }
}

[System.Serializable]
public class Field
{
    public GameObject inst;
    public FieldType type;
    public string value;
    public string source;

    public enum FieldType
    {
        textInput,
    }
}
