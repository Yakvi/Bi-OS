using UnityEngine;

public class EditorUI : MonoBehaviour {
    public InputBox id; 
    public InputBox prompt; 
    public DropdownBox type;
    public InputBox reprompt; // TODO: Convert to Input List when supported by the game.
    public InputList answers;
    
    private void Start() {

    }

    public void Clear()
    {
        id.data.data = "";
        prompt.data.data = "";
        reprompt.data.data = "";
        answers.Clear();
    }

    public void Display (Prompt source)
    {
        id.data.data = source.id.ToString();
        prompt.data.data = source.prompt;
        reprompt.data.data = source.reprompt;
        foreach (var answer in source.acceptedResponses)
        {
            answers.Add(answer);
        }
    }
}