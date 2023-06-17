using System.Collections.Generic;
using UnityEngine;

public class CopyManager : MonoBehaviour
{
    public List<ObjectCopy> copyList;

    GameObject[] cards;

    void Start()
    {
        cards = GameObject.FindGameObjectsWithTag("specialCard");
    }

    void Update()
    {
        for (int i = 0; i < copyList.Count; i++)
        {
            copyList[i].SetCopyTransform(cards[i].transform);
        }
    }

    public void SetEnabledObjects(int index, bool enabled)
    {
        copyList[index].Object.enabled = enabled;
    }
}
