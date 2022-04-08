using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
public static class filesystemUtils
{

    public static string getFirstFolder(string filesystemRoot)
    {
        if (Directory.Exists(filesystemRoot))
        {
            return filesystemRoot;
            // try
            // {
            //     string[] containedDirs = Directory.GetDirectories(filesystemRoot);
            //     if(containedDirs.Length > 0)
            //         return containedDirs[0];
            //     string[] containedFiles = Directory.GetFiles(filesystemRoot);
            //     if(containedFiles.Length > 0)
            //         return containedFiles[0];
            // }catch(System.UnauthorizedAccessException e){
            //     Debug.LogWarning("error caught: " + e.Message);
            // }  
        }
        return "";
    }
    public async static Task<List<string>> getAllFilesAndDirs(string filesystemRoot){

        List<string> dirList = await getAllDirectories(filesystemRoot);
        List<string> returnData = new List<string>();
        returnData.AddRange(dirList);
        foreach (string s in dirList)
        {
            foreach (string f in getAllFiles(s))
            {
                returnData.Add(f);
                await Task.Yield();
            }
        }
        return returnData;
    }
    //single thread 353235433
    //multi thread 
    public async static Task<List<string>> getAllDirectories(string filesystemRoot){
        Debug.Log("Directory:" + filesystemRoot + " exists:" + Directory.Exists(filesystemRoot));
        if (Directory.Exists(filesystemRoot))
        {
            //Debug.Log(filesystemRoot);
            try
            {
                await Task.Yield();
                string[] containedDirs = Directory.GetDirectories(filesystemRoot);
                List<string> returnDirs = new List<string>(containedDirs);
                foreach (string dir in containedDirs)
                {
                    returnDirs.AddRange(await getAllDirectories(dir));
                }
                return returnDirs;
            }catch(System.UnauthorizedAccessException e){
                Debug.LogWarning("error caught: " + e.Message);
            }
        }
        return new List<string>();
    }

    public static string[] getImmediateFolders(string filesystemRoot){
        List<string> returnArray = new List<string>();
        if (Directory.Exists(filesystemRoot))
        {
            try
            {
                string[] containedDirs = Directory.GetDirectories(filesystemRoot);
                if(containedDirs.Length > 0)
                    returnArray.AddRange(new List<string>(containedDirs));
                // string[] containedFiles = Directory.GetFiles(filesystemRoot);
                // if(containedFiles.Length > 0)
                //     returnArray.AddRange(new List<string>(containedFiles));
            }catch(System.UnauthorizedAccessException e){
                Debug.LogWarning("error caught: " + e.Message);
            }  
        }
        //returnArray.Sort((a, b) => (a.Length <= b.Length) ? (a.Length == b.Length) ? 0 : 1 : -1);
        return returnArray.ToArray();
    }

    public static string[] getAllFiles(string filesystemRoot){
        if (Directory.Exists(filesystemRoot))
        {
            try
            {
                string[] containedFiles = Directory.GetFiles(filesystemRoot);
                if(containedFiles.Length > 0)
                    return containedFiles;
            }catch(System.UnauthorizedAccessException e){
                Debug.LogWarning("error caught: " + e.Message);
            }  
        }
        string[] empty = { };
        return empty;
    }

    public static bool dirExists(string dir){
        return Directory.Exists(dir);
    }

}
