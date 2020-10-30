using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextScene : MonoBehaviour
{
    // Start is called before the first frame update        
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.name == "Player")
        {
            Invoke("GoNextScene", 2f);
        }
    }

    void GoNextScene()
    {
        ScenesManager.GetInstance().LoadScene("GameOver", callback);
    }

    void callback()
    {
        Invoke("Quit", 2f);
    }

    void Quit()
    {
        Application.Quit();
    }
}
