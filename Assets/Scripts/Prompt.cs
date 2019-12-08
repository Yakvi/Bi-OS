using System.Collections.Generic;

[System.Serializable]
public class Prompt
{
    public int id;
    public string type;
    public string prompt;
    public string reprompt;
    public List<string> acceptedResponses;
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
