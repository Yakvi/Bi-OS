using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int maxMessages = 60;

    public GameObject consolePanel, textObject;
    public InputField consoleInput;

    public GameLocale gameLang;

    Locale currentLocale;
    [SerializeField]
    Prompt currentPrompt;

    bool awaitingPrompt = true;

    List<Message> messageList = new List<Message>();

    void Start()
    {
        LoadLocale(gameLang.ToString(), "prompts");
        currentPrompt = LoadPrompt(0);
    }

    private void Update()
    {
        // NOTE: State check
        consoleInput.gameObject.SetActive(awaitingPrompt);
        if (!consoleInput.isFocused) consoleInput.ActivateInputField();
        if (gameLang.ToString() != currentLocale.language) LoadLocale(gameLang.ToString(), "prompts");

        // NOTE: Input processing
        if (consoleInput.text != "" &&
            (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            AddMessage(consoleInput.text, Message.MessageType.playerMessage);
            ReactToMessage(consoleInput.text);
            consoleInput.text = "";
        }
    }

    void LoadLocale(string lang, string source)
    {
        var promptsJson = Resources.Load<TextAsset>(source);
        var collection = JsonUtility.FromJson<PromptCollection>(promptsJson.text);

        foreach (var loc in collection.locales)
        {
            if (loc.language == lang)
            {
                currentLocale = loc;
                break;
            }
        }
    }

    Prompt LoadNextPrompt() { return LoadPrompt(currentPrompt.id + 1); }
    Prompt LoadPrompt(int id)
    {
        Prompt result = null;

        foreach (var prompt in currentLocale.prompts)
        {
            if (prompt.id == id)
            {
                result = prompt;
                break;
            }
        }

        if (result != null)
        {
            Enum.TryParse(result.type, out Message.MessageType type);
            AddMessage(result.prompt, type);
        }
        else
        {
            // TODO: elegant solution to no prompt found
        }

        return (result);
    }

    void ReactToMessage(string text)
    {
        var sanitizedText = text.ToLower(); // TODO: remove punctuation?

        if (currentPrompt?.acceptedResponses.Length > 0)
        {
            var found = false;

            foreach (var response in currentPrompt.acceptedResponses)
            {
                if (response.ToLower() == sanitizedText) found = true;
            }

            if (found)
            {
                currentPrompt = LoadNextPrompt();
            }
            else
            {
                Enum.TryParse(currentPrompt.type, out Message.MessageType type);
                AddMessage(currentPrompt.reprompt, type);
            }
        }
        else
        {
            // NOTE: No matter what response is, story moves on.
            currentPrompt = LoadNextPrompt();
        }

    }

    void AddMessage(string text, Message.MessageType messageType)
    {
        if (messageList.Count >= maxMessages)
        {
            var Index = 0; //messageList.Count - 1;

            Destroy(messageList[Index].textObject.gameObject);
            messageList.Remove(messageList[Index]);
        }

        var message = new Message();
        var newText = Instantiate(textObject, consolePanel.transform);

        message.textObject = newText.GetComponent<Text>();

        // NOTE: Setting formatting options for various types of messages
        var delay = 0;
        switch (messageType)
        {
            case Message.MessageType.aiNormal:
                {
                    message.textObject.alignment = TextAnchor.LowerLeft;
                    delay = UnityEngine.Random.Range(1, 3);
                    awaitingPrompt = false;
                }
                break;
            default:
                {
                    message.textObject.alignment = TextAnchor.LowerRight;
                }
                break;
        }

        // NOTE: Output the message
        StartCoroutine(DisplayMessage(message, text, delay));
    }

    IEnumerator DisplayMessage(Message message, string text, int delay)
    {
        yield return new WaitForSeconds(delay);

        message.textObject.text = message.text = text;
        messageList.Add(message);

        if (delay > 0) awaitingPrompt = true;
    }
}

public enum GameLocale
{
    EN,
    RU,
    IT
}

[System.Serializable]
public class Message
{
    public string text;
    public Text textObject;
    public MessageType messageType;

    public enum MessageType
    {
        aiNormal,
        aiImportant,
        aiAngry,
        playerMessage,
    }
}
