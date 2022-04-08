using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;


public class vrGameLoader : MonoBehaviour
{
    [SerializeField]
    private string gameFolder = @"D:\steam\steamapps\common\";
    private string isVrFile, isNotVrFile;
    // Start is called before the first frame update
    public GameObject gameCube;
    async void Start()
    {
        isVrFile = Path.Combine(Application.persistentDataPath, "vrGames.txt");
        isNotVrFile = Path.Combine(Application.persistentDataPath, "notVrGame.txt");
        List<string> rememberedVrGames = new List<string>();
        List<string> rememberedNonVrGames = new List<string>();
        if(File.Exists(isVrFile)){
            try
            {
                using (StreamReader sr = new StreamReader(isVrFile))
                {
                    Task<string> line = sr.ReadLineAsync();
                    string taskResult = line.GetAwaiter().GetResult();;
                    while (taskResult != null)
                    {
                        while (!line.IsCompleted)
                        {
                            await Task.Yield();
                        }
                        rememberedVrGames.Add(taskResult);
                        taskResult = line.GetAwaiter().GetResult();
                        
                        line = sr.ReadLineAsync();
                    }
                }
                using (StreamReader sr = new StreamReader(isNotVrFile))
                {
                    Task<string> line = sr.ReadLineAsync();
                    string taskResult = line.GetAwaiter().GetResult();;
                    while (taskResult != null)
                    {
                        while (!line.IsCompleted)
                        {
                            await Task.Yield();
                        }
                        rememberedNonVrGames.Add(taskResult);
                        taskResult = line.GetAwaiter().GetResult();
                        
                        line = sr.ReadLineAsync();
                    }
                }
            }catch(System.Exception e){
                Debug.LogError(e);
            }
        }
        List<string> allGames = new List<string>(Directory.GetDirectories(gameFolder));
        allGames.RemoveAll(entry => rememberedNonVrGames.Contains(entry));
        foreach(string vrGames in rememberedVrGames){
            char[] splitChar = {'|'};
            print(vrGames);
            string[] splitString = vrGames.Split(splitChar);
            if(File.Exists(splitString[1]))
                allGames.Remove(splitString[0]);
        }
        float index = 0;
        StreamWriter sw = new StreamWriter(isVrFile,append:true);
        StreamWriter notVr = new StreamWriter(isNotVrFile,append:true);
        foreach(string game in allGames){
            Debug.Log($"searching {game}");
            string pathToOpenVr = await findVrGames(game);
            if(pathToOpenVr != null){
                print($"game: {game} is VR");
                string exePath = await findExe(game);
                if(exePath == null)
                    throw new Exception($"{game} exe not found");
                string newGameToAdd = game + "|" + pathToOpenVr + "|" + exePath;
                await sw.WriteLineAsync(newGameToAdd);
                rememberedVrGames.Add(newGameToAdd);
                sw.Flush();
            }else{
                await notVr.WriteLineAsync(game);
                notVr.Flush();
            }
            index++;
            Debug.LogWarning($"search is {index / allGames.Count * 100}% complete");
        }
        sw.Close();
        notVr.Close();
        foreach(string s in rememberedVrGames){
            GameObject g = Instantiate(gameCube);
            g.GetComponent<vrGame>().setup(s.Split('|')[2]);
        }
        print("done searching");
    }

    async Task<String> findVrGames(string folderToCheck){
        await Task.Yield();
        if(File.Exists(Path.Combine(folderToCheck,"openvr_api.dll")))
            return Path.Combine(folderToCheck,"openvr_api.dll");

        foreach(string folder in Directory.GetDirectories(folderToCheck)){
            string result = await findVrGames(folder);
            if(result != null)
                return result;
        }
        return null;
    }

    async Task<string> findExe(string folderToCheck){
        await Task.Yield();
        foreach(string file in Directory.GetFiles(folderToCheck)){
            if(Path.GetExtension(file).ToLower() == ".exe")
                return file;
        }

        foreach(string folder in Directory.GetDirectories(folderToCheck)){
            string result = await findVrGames(folder);
            if(result != null)
                return result;
        }
        return null;
    }
    // public static void ExtractIconFromFilePath(string executablePath)
    // {
    //     Icon result = (Icon) null;

    //     try
    //     {
    //         result = Icon.ExtractAssociatedIcon(executablePath);
    //         using (FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath,"test.ico"), FileMode.CreateNew))
    //         {
    //             result.ToBitmap().Save(stream, result.ToBitmap().RawFormat);
    //         }
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogWarning( e + "Unable to extract the icon from the binary");
    //     }
    // }

}
