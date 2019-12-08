using UnityEngine;

public class EditorUI : MonoBehaviour
{
    public InputBox id;
    public InputBox prompt;
    public DropdownBox type;
    public InputBox reprompt; // TODO: Convert to Input List when supported by the game.
    public InputList answers;

    public Prompt buffer = new Prompt();

    private void Start()
    {

    }

    private void Update() {
        // Read(); // For debugging
    }

    public void Clear()
    {
        id.data.data = "";
        prompt.data.data = "";
        reprompt.data.data = "";
        answers.Clear();
    }

    public void Display(Prompt source)
    {
        id.data.data = source.id.ToString();
        prompt.data.data = source.prompt;
        reprompt.data.data = source.reprompt;
        foreach (var answer in source.acceptedResponses)
        {
            answers.Add(answer);
        }
    }

    public Prompt Read()
    {
        // NOTE: Clear
        buffer.id = 0;
        buffer.prompt = "";
        buffer.reprompt = "";
        buffer.type = "aiNormal";
        buffer.acceptedResponses.Clear();

        int.TryParse(id.data.data, out buffer.id);
        buffer.prompt = prompt.data.data;
        buffer.reprompt = reprompt.data.data;
        foreach (var answer in answers.data)
        {
            buffer.acceptedResponses.Add(answer.data.data);
        }

        return buffer;
    }
}
