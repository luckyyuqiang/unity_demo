using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Context : MonoBehaviour
{
    public bool initSDK { get; set; }

    private List<string> currentPath;

    public long StartTs = 0; // second

    private void Awake()
    {
        StartTs = Tools.ToTimeStamp(DateTime.Now);
    }

    public void SavePath(List<string> path)
    {
        currentPath = path.ToList();
        if (null == currentPath || currentPath.Count == 0)
        {
            Debug.LogError($"Invalid currentPath.");
        }
    }

    public List<string> GetCurrentPath()
    {
        return currentPath;
    }

    public bool IsCurrentPath(List<string> path)
    {
        return Tools.IsSamePath(currentPath, path, path.Count);
    }
}
