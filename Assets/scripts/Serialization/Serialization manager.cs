using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using UnityEngine;

public class Serializationmanager
{
   public static bool Save(string saveName, object saveData){
        BinaryFormatter formatter = getBinaryFormatter();
        if(!Directory.Exists(Application.persistentDataPath+"/userRoomData")){
            Directory.CreateDirectory(Application.persistentDataPath + "/userRoomData");
        }

        string path = Application.persistentDataPath + "/userRoomData/" + saveName+".mcn";
        FileStream f = File.Create(path);
        formatter.Serialize(f, saveData);
        f.Close();

        return true;
    }

    public static object load(string path){
        if(!File.Exists(path)){
            return null;
        }

        BinaryFormatter formatter = getBinaryFormatter();

        FileStream f = File.Open(path,FileMode.Open);
        try
        {
            object o = formatter.Deserialize(f);
            f.Close();
            return o;
        }catch{
            Debug.LogErrorFormat("failed to load object from save path {0}", path);
            f.Close();
            return null;
        }
    }

public static BinaryFormatter getBinaryFormatter(){
        BinaryFormatter formatter = new BinaryFormatter();

        SurrogateSelector selector = new SurrogateSelector();

        vec3Serilaizer vec3Surrogate = new vec3Serilaizer();
        quatSerializer quatSurrogate = new quatSerializer();

        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vec3Surrogate);
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quatSurrogate);

        formatter.SurrogateSelector = selector;

        return formatter;
    }
}
