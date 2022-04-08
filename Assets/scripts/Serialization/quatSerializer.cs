using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class quatSerializer : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context){
        Quaternion quat = (Quaternion)obj;
        info.AddValue("x", quat.x);
        info.AddValue("y", quat.y);
        info.AddValue("z", quat.z);
        info.AddValue("w", quat.w);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector){
        Quaternion quat = (Quaternion)obj;
        quat.x = (float)info.GetValue("x", typeof(float));
        quat.y = (float)info.GetValue("y", typeof(float));
        quat.z = (float)info.GetValue("z", typeof(float));
        quat.w = (float)info.GetValue("w", typeof(float));
        return quat;
    }
}
