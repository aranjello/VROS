using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class saveData
{
    private static saveData _current;
    public static saveData current{
        get{
            if(_current == null){
                _current = new saveData();
            }
            return _current;
        }set{
            _current = value;
        }
    }

    public List<vrObjData> objs = new List<vrObjData>();
    

}

