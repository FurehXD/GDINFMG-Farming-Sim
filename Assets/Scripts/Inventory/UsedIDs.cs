using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UsedIDs : MonoBehaviour
{
    private List<int> usedCropIDs = new();
    public List<int> UsedCropIDs { get { return usedCropIDs; } }
}
