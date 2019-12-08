using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    public GameObject editorOutput, inputObjectTemplate;

    public Dropdown languageDropdown;
    public Button prevPromptButton, nextPromptButton;

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
        prevPromptButton.onClick.AddListener(LoadPrevPrompt);
        nextPromptButton.onClick.AddListener(LoadNextPrompt);

        // NOTE: Serialize data to memory
        LoadLocale(gameLang.ToString(), "prompts");
        LoadPrompt(0);
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
        //     currentPrompt = ReadUI();
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
