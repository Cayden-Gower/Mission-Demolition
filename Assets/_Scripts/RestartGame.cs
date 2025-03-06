using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class RestartGame : MonoBehaviour
{
    static public void Restart() {
        SceneManager.LoadScene("_Scene_0");

    }
}
