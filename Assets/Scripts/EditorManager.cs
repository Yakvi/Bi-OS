using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    public GameObject consolePanel, textObject;

    public Dropdown languageDropdown;
    public InputField consoleInput, IDField;

    public GameLocale gameLang;

    Locale currentLocale;
    [SerializeField]
    Prompt currentPrompt;

    List<Message> messageList = new List<Message>();

    void Start()
    {
        // NOTE: Load data
        LoadLocale(gameLang.ToString(), "prompts");
        currentPrompt = LoadPrompt(0);

        // NOTE: Load UI
        var languageArray = Enum.GetNames(typeof(GameLocale));
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(new List<string>(languageArray));

        // NOTE: Populate UI
        ShowFields();
    }

    private void ShowFields()
    {
        IDField.text = currentPrompt.id.ToString();
    }

    private void Update()
    {
        if (languageDropdown.value != (int) gameLang)
        {
            gameLang = (GameLocale) languageDropdown.value;
            LoadLocale(gameLang.ToString(), "prompts");
            currentPrompt = ReloadPrompt();
        }
        // // NOTE: State check
        // consoleInput.gameObject.SetActive(awaitingPrompt);
        // if (!consoleInput.isFocused) consoleInput.ActivateInputField();
        // if (gameLang.ToString() != currentLocale.language) 

        // // NOTE: Input processing
        // if (field.text != "" &&
        //     (Input.GetKeyDown(KeyCode.Return) || 
        //      Input.GetKeyDown(KeyCode.KeypadEnter) || 
        //      !field.isFocused)))
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

    Prompt ReloadPrompt() { return LoadPrompt(currentPrompt.id); }
    Prompt LoadNextPrompt() { return LoadPrompt(currentPrompt.id + 1); }
    Prompt LoadPreviousPrompt() { return LoadPrompt(currentPrompt.id - 1); }
    Prompt LoadPrompt(int id)
    {
        Prompt result = currentPrompt;

        if (id >= 0 && id <= currentLocale.prompts.Length)
        {
            result = currentLocale.prompts[id];
            if (result.id != id) Debug.LogError("Wrong prompt loaded! Expected prompt ${id}, loaded prompt ${result.id}.");
        }

        if (result != null)
        {
            // TODO: decipher and load the whole prompt
            Enum.TryParse(result.type, out Message.MessageType type);
            // PopulateFields(result.prompt, type);
        }

        return (result);
    }

    void AddField(string text, Message.MessageType messageType)
    {
        var message = new Message();
        var newText = Instantiate(textObject, consolePanel.transform);

        message.textObject = newText.GetComponentInChildren<Text>();

        // TODO: Switch behavior depending on the field type
        switch (messageType)
        {
            default : break;
        }

        // NOTE: Output the message
        messageList.Add(message);
        message.textObject.text = message.text = text;
    }
}
