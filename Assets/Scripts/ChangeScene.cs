using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    [SerializeField] private GameObject MainMenuScene;
    [SerializeField] private GameObject GameScene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void OnTriggerEnter(Collider other)
	{
        MainMenuScene.SetActive(false);
        GameScene.SetActive(true);
        Debug.Log("CONTACT");
    }
}
