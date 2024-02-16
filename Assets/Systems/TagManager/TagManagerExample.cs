using UnityEngine;

[RequireComponent(typeof(TagObject))]
public class TagManagerExample : MonoBehaviour
{
    public string TestTag = "Ground";

    [ContextMenu("Print")]
    public void Print()
    {
        //UNITY: var gos = GameObject.FindGameObjectsWithTag(TestTag);
        var gos = TagManager.FindGameObjectsWithTag(TestTag);

        //UNITY: var match = gameObject.CompareTag(TestTag)
        var match = gameObject.CompareTags(TestTag);

        Debug.Log($"Match: {match}");

        if(gos != null)
            Debug.Log($"Count: {gos.Length}");
    }
}