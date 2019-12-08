using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugSceneManager : MonoBehaviour
{
    public string sceneName;
    Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        if (button)
        {
            button.onClick.AddListener(LoadScene);
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
