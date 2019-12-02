using UnityEngine;

[System.Serializable]
public class Prompt
{
    public int id;
    public string type;
    public string prompt;
    public string reprompt;
    public string[] acceptedResponses;
}

[System.Serializable]
public class Locale
{
    public string language;
    public Prompt[] prompts;
}

[System.Serializable]
public class PromptCollection
{
    public Locale[] locales;
}
