using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader: MonoBehaviour
{
    public int sceneToLoad = 0;
   
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad); 
    }
}
