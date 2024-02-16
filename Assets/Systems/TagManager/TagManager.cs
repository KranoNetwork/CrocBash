using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TagManager
{
    public static Dictionary<GameObject, TagObject> GameObjectTags = new Dictionary<GameObject, TagObject>();
    public static Dictionary<string, List<GameObject>> TagGameObjects = new Dictionary<string, List<GameObject>>();

    public static void Add(TagObject tagObject)
    {
        if (GameObjectTags.ContainsKey(tagObject.gameObject))
            return;

        GameObjectTags.Add(tagObject.gameObject, tagObject);

        foreach (var tag in tagObject.Tags)
        {
            if (!TagGameObjects.ContainsKey(tag))
            {
                var list = new List<GameObject>();
                TagGameObjects.Add(tag, list);
            }

            var goList = TagGameObjects[tag];
            goList.Add(tagObject.gameObject);
        }
    }

    public static void Remove(TagObject tagObject)
    {
        GameObjectTags.Remove(tagObject.gameObject);

        foreach (var tag in tagObject.Tags)
        {
            var list = TagGameObjects[tag];
            list.Remove(tagObject.gameObject);
        }
    }

    public static bool CompareTags(this GameObject go, string tagToCompare)
    {
        var tagObject = GameObjectTags[go];
        foreach (var tag in tagObject.Tags)
        {
            if (tag == tagToCompare)
                return true;
        }

        return false;
    }

    public static string GetTag(this GameObject go)
    {
        return GameObjectTags[go].Tags[0];
    }

    public static string[] GetTags(this GameObject go)
    {
        return GameObjectTags[go].Tags;
    }

    public static GameObject FindGameObjectWithTag(string tag)
    {
        if (!TagGameObjects.ContainsKey(tag))
        {
            Debug.LogError($"Tag {tag} is not defined");
            return null;
        }

        var list = TagGameObjects[tag];
        return list[0];
    }

    public static GameObject[] FindGameObjectsWithTag(string tag)
    {
        if (!TagGameObjects.ContainsKey(tag))
        {
            Debug.LogError($"Tag {tag} is not defined");
            return null;
        }

        return TagGameObjects[tag].ToArray();
    }
}