using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Newtonsoft.Json;
using QualisysRealTime.Unity;

public class Utils {

    public static byte[] Trans2byte<T>(T data) {
        byte[] dataBytes;
        using (MemoryStream ms = new MemoryStream()) {
            BinaryFormatter bf1 = new BinaryFormatter();
            bf1.Serialize(ms, data);
            dataBytes = ms.ToArray();
        }
        return dataBytes;
    }

    public static T byte2Origin<T>(byte[] data) {
        MemoryStream ms = new MemoryStream(data);
        BinaryFormatter bf = new BinaryFormatter();
        ms.Position = 0;
        return (T)bf.Deserialize(ms);
    }

    //where ==> 限制 T 繼承自object
    public static T resLoad<T>(string name) where T : UnityEngine.Object {
        T res = Resources.Load<T>(name);
        if (res == null) {
            Debug.LogError("Resources.Load [ " + name + " ] is Null !!");
            return default(T);
        }
        return res;
    }

    public static void Save<T>(T input, string filename = "1.txt") {
        File.WriteAllText(filename, JsonConvert.SerializeObject(input,Formatting.Indented, new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore//忽略實體中實體，不再序列化里面包含的實體
        }));
        //File.WriteAllBytes(filename, Trans2byte(input));

    }

    public static T load<T>(string filename = "1.txt") {
        return JsonConvert.DeserializeObject<T>(File.ReadAllText(filename));
        //return byte2Origin<T>(File.ReadAllBytes(filename));
    }


}