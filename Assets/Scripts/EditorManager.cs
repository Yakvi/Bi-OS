using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    public GameObject editorOutput, inputObjectTemplate;

    public Dropdown languageDropdown;
    public Button prevButton, nextButton, saveButton, resetButton, deleteButton, newButton;

    public GameLocale gameLang;

    [SerializeField]
    Locale currentLocale; // TODO: Multiple language editor
    Prompt currentPrompt;

    public EditorUI promptEditor; // id, type, prompt, reprompt, responses // TODO: multiple reprompts

    void Start()
    {
        // NOTE: Set language
        var languageArray = Enum.GetNames(typeof(GameLocale));
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(new List<string>(languageArray));
        languageDropdown.value = 1; // TODO: multiple language support in game
        gameLang = (GameLocale) languageDropdown.value;

        // NOTE: Config UI
        prevButton.onClick.AddListener(LoadPrevPrompt);
        nextButton.onClick.AddListener(LoadNextPrompt);
        newButton.onClick.AddListener(NewPrompt);
        resetButton.onClick.AddListener(ReloadPrompt);
        saveButton.onClick.AddListener(SavePrompt);
        deleteButton.onClick.AddListener(DeletePrompt);

        // NOTE: Serialize data to memory
        DeserializeLocale(gameLang.ToString(), "prompts");
        LoadPrompt(0);
    }

    private void Update()
    {
        if (languageDropdown.value != (int) gameLang)
        {
            SavePrompt();
            gameLang = (GameLocale) languageDropdown.value;
            DeserializeLocale(gameLang.ToString(), "prompts");
            ReloadPrompt();
        }
    }

    private void NewPrompt()
    {
        var prompt = new Prompt();
        prompt.id = currentLocale.prompts.Count;
        prompt.acceptedResponses = new List<string>();
        prompt.prompt = "";
        prompt.reprompt = "";
        prompt.type = "aiNormal";
        currentLocale.prompts.Add(prompt);
        LoadPrompt(prompt.id);
    }

    private void DeletePrompt()
    {
        currentLocale.prompts.Remove(currentLocale.prompts[currentPrompt.id]);
        SerializeLocale(gameLang.ToString(), "prompts");
        ReloadPrompt();
    }

    private void SavePrompt()
    {
        currentPrompt = promptEditor.Read();
        if (currentPrompt.id < currentLocale.prompts.Count)
        {
            currentLocale.prompts[currentPrompt.id] = currentPrompt;
        }
        else
        {
            currentLocale.prompts.Add(currentPrompt);
        }
        SerializeLocale(gameLang.ToString(), "prompts");
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
        Assert.IsNotNull(currentLocale);
        var collection = LoadCollection(source);
        var found = false;
        for (var i = 0; i < collection.locales.Count; ++i)
        {
            if (collection.locales[i].language == lang)
            {
                collection.locales[i] = currentLocale;
                found = true;
            }
        }
        if (!found) collection.locales.Add(currentLocale);

        var output = JsonUtility.ToJson(collection);
        var writer = new StreamWriter("Assets/Resources/" + source + ".json"); // Does this work?
        writer.WriteLine(output);
        writer.Close();
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
        currentLocale = new Locale();
        currentLocale.language = lang;
        currentLocale.prompts = new List<Prompt>();
        NewPrompt();
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
        else if (id >= 0 && currentLocale.prompts.Count > 0)
        {
            result = currentLocale.prompts[currentLocale.prompts.Count - 1];
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
