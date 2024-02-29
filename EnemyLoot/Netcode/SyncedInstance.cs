using LethalLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Unity.Netcode;

namespace EnemyLoot.Netcode
{
    [Serializable]
    public class SyncedInstance<T>
    {
        internal static CustomMessagingManager MessageManager => NetworkManager.Singleton.CustomMessagingManager;
        internal static bool IsClient => NetworkManager.Singleton.IsClient;
        internal static bool IsHost => NetworkManager.Singleton.IsHost;

        [NonSerialized]
        protected static int IntSize = 4;

        public static T Default { get; private set; }
        public static T Instance { get; private set; }

        public static bool Synced { get; internal set; }

        protected void InitInstance(T instance)
        {
            Default = instance;
            Instance = instance;

            IntSize = sizeof(int);
        }

        internal static void SyncInstance(byte[] data)
        {
            Instance = DeserializeFromBytes(data);
            Synced = true;
        }

        internal static void RevertSync()
        {
            Instance = Default;
            Synced = false;
        }


        public static byte[] SerializeToBytes(T val)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
          
            try
            {
                bf.Serialize(stream, val);
                return stream.ToArray();
            }
            catch (Exception e)
            {
                Plugin.logger.LogError($"Error serializing instance: {e}");
                return null;
            }
            
        }


        //public static byte[] SerializeToBytes(T val)
        //{



        //    try
        //    {
        //        return JsonSerializer.SerializeToUtf8Bytes(val);
        //    }
        //    catch (Exception e)
        //    {
        //        Plugin.logger.LogError($"Error serializing instance: {e}");
        //        return null;
        //    }
        //}

        public static T DeserializeFromBytes(byte[] data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(data);

            try
            {
                return (T)bf.Deserialize(stream);
            }
            catch (Exception e)
            {
                Plugin.logger.LogError($"Error deserializing instance: {e}");
                return default;
            }
        }



        //public static T DeserializeFromBytes(byte[] data)
        //{
   

        //    try
        //    {
        //        return JsonSerializer.Deserialize<T>(data);
        //    }
        //    catch (Exception e)
        //    {
        //        Plugin.logger.LogError($"Error deserializing instance: {e}");
        //        return default;
        //    }
        //}
    }
}
