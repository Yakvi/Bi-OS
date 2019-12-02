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

    public GameLocale locale;
    [SerializeField]
    Locale prompts;

    bool awaitingPrompt = true;

    private List<Message> messageList = new List<Message>();

    private void Start()
    {
        SetGameLanguage(locale.ToString(), "prompts");
    }

    private void SetGameLanguage(string lang, string source)
    {
        var promptsJson = Resources.Load<TextAsset>(source);
        var collection = JsonUtility.FromJson<PromptCollection>(promptsJson.text);

        foreach (var loc in collection.locales)
        {
            if (loc.language == lang)
            {
                prompts = loc;
                break;
            }
        }
    }

    private void Update()
    {
        // NOTE: State check
        consoleInput.gameObject.SetActive(awaitingPrompt);
        if (!consoleInput.isFocused) consoleInput.ActivateInputField();
        if (locale.ToString() != prompts.language) SetGameLanguage(locale.ToString(), "prompts");

        // NOTE: Input processing
        if (consoleInput.text != "" &&
            (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            AddMessage(consoleInput.text, Message.MessageType.playerMessage);
            ReactToMessage(consoleInput.text);
            consoleInput.text = "";
        }
    }

    void ReactToMessage(string text)
    {
        var sanitizedText = text.ToLower(); // TODO: remove punctuation
        if (sanitizedText == "this was a triumph")
        {
            AddMessage("I'm making a note here: huge success!", Message.MessageType.aiMessageNormal);
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
            case Message.MessageType.aiMessageNormal:
                {
                    message.textObject.alignment = TextAnchor.LowerRight;
                    delay = UnityEngine.Random.Range(1, 3);
                    awaitingPrompt = false;
                }
                break;
            default:
                {
                    message.textObject.alignment = TextAnchor.LowerLeft;
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
        playerMessage,
        aiMessageNormal,
        aiMessageImportant,
        aiMessageAngry,
    }
}
