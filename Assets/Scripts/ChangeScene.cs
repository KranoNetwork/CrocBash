using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
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
        //SceneManager.LoadScene("ArtAssetsTestScene", LoadSceneMode.Additive);
        SceneManager.LoadScene("ArtAssetsTestScene");
        Debug.Log("CONTACT");
    }
}
