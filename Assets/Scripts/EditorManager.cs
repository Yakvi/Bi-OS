using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    public GameObject editorOutput, inputObjectTemplate;

    public Dropdown languageDropdown;
    public Button prevButton, nextButton, saveButton, resetButton, deleteButton;

    public GameLocale gameLang;

    Locale currentLocale; // TODO: Multiple language editor
    Prompt currentPrompt;

    [SerializeField]
    public EditorUI promptEditor; // id, type, prompt, reprompt, responses // TODO: multiple reprompts

    void Start()
    {
        // NOTE: Set language
        var languageArray = Enum.GetNames(typeof(GameLocale));
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(new List<string>(languageArray));
        gameLang = (GameLocale) languageDropdown.value;

        // NOTE: Config UI
        prevButton.onClick.AddListener(LoadPrevPrompt);
        nextButton.onClick.AddListener(LoadNextPrompt);
        resetButton.onClick.AddListener(ReloadPrompt);
        saveButton.onClick.AddListener(SavePrompt);
        // deleteButton.onClick.AddListener(DeletePrompt);

        // NOTE: Serialize data to memory
        DeserializeLocale(gameLang.ToString(), "prompts");
        LoadPrompt(0);
    }

    private void SavePrompt()
    {
        currentPrompt = promptEditor.Read();
        currentLocale.prompts[currentPrompt.id] = currentPrompt;
        SerializeLocale(gameLang.ToString(), "prompts");
    }


    private void Update()
    {
        if (languageDropdown.value != (int) gameLang)
        {
            gameLang = (GameLocale) languageDropdown.value;
            DeserializeLocale(gameLang.ToString(), "prompts");
            ReloadPrompt();
        }
    }

    PromptCollection LoadCollection(string source)
    {
        var promptsJson = Resources.Load<TextAsset>(source);
        var collection = JsonUtility.FromJson<PromptCollection>(promptsJson.text);
        Assert.IsNotNull(collection);

        return collection;
    }

    private void SerializeLocale(string lang, string source)
    {
        var collection = LoadCollection(source);
        for (var i = 0; i < collection.locales.Length; ++i)
        {
            if(collection.locales[i].language == lang)
            {
                collection.locales[i] = currentLocale;
            }
        }

        JsonUtility.ToJson(collection);
    }

    void DeserializeLocale(string lang, string source)
    {
        var collection = LoadCollection(source);
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

    void ReloadPrompt() { LoadPrompt(currentPrompt.id); }
    void LoadNextPrompt() { LoadPrompt(currentPrompt.id + 1); }
    void LoadPrevPrompt() { LoadPrompt(currentPrompt.id - 1); }
    void LoadPrompt(int id)
    {
        Prompt result = currentPrompt;

        if (id >= 0 && id < currentLocale.prompts.Count)
        {
            result = currentLocale.prompts[id];
            if (result.id != id) Debug.LogError(
                "Wrong prompt loaded! Expected prompt" + id + ", loaded prompt" + result.id);
        }

        currentPrompt = result;
        WriteUI(currentPrompt);
    }

    private void WriteUI(Prompt currentPrompt)
    {
        promptEditor.Clear();
        promptEditor.Display(currentPrompt);
    }
}
