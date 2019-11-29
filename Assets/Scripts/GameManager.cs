using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int maxMessages = 25;

    public GameObject consolePanel, textObject;
    public InputField consoleInput;

    [SerializeField]
    private List<Message> messageList = new List<Message>();

    private void Update()
    {

        if (consoleInput.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SendMessageToChat(consoleInput.text, Message.MessageType.playerMessage);
                consoleInput.text = "";
            }

        }

        if (!consoleInput.isFocused)
        {
            consoleInput.ActivateInputField();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SendMessageToChat("This is a Triumph", Message.MessageType.aiMessageNormal);
                Debug.Log("Space");
            }
        }
    }

    public void SendMessageToChat(string text, Message.MessageType messageType)
    {
        if (messageList.Count >= maxMessages)
        {
            var Index = 0; //messageList.Count - 1;

            Destroy(messageList[Index].textObject.gameObject);
            messageList.Remove(messageList[Index]);
        }

        var newMessage = new Message();
        var newText = Instantiate(textObject, consolePanel.transform);

        newMessage.textObject = newText.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text = text;

        switch (messageType)
        {
            case Message.MessageType.aiMessageNormal:
                {
                    newMessage.textObject.alignment = TextAnchor.LowerRight;
                }
                break;
            default:
                {
                    newMessage.textObject.alignment = TextAnchor.LowerLeft;
                }
                break;
        }

        messageList.Add(newMessage);
    }
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
