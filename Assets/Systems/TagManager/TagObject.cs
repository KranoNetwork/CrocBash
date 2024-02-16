using UnityEngine;

public class TagObject : MonoBehaviour
{
    public string[] Tags;

    private void Awake()
    {
        TagManager.Add(this);
    }

    private void OnDestroy()
    {
        TagManager.Remove(this);
    }
}