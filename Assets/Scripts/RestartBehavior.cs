using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartBehavior : MonoBehaviour
{
    // P R O P E R T I E S
    [SerializeField] private string playerTagName;
    [SerializeField] GameManager GM;

    // M E T H O D S
    public void OnCollisionEnter(Collision collision)
    {
        // used for making sure the things are hit from above and not from the side
        Vector3 _tempRelativePosition;

        if (TagManager.CompareTags(collision.gameObject, playerTagName))
        {
            GM.RestartGame();
            this.gameObject.SetActive(false);
        }
    }

}
