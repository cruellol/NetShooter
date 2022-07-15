using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Net
{
    public static class Debugger
    {
        private static Text _console;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void OnStart()
        {
            FindConsole();
        }

        private static void FindConsole()
        {
            var texts = GameObject.FindObjectsOfType<Text>();
            if (texts != null)
                _console = texts.FirstOrDefault(t => t.name == "console");
#if UNITY_EDITOR
            if (_console == null)
            {
                Debug.Log("Console not found");
            }
#endif
        }

        public static void Log(object message)
        {
            FindConsole();
#if UNITY_EDITOR
            Debug.Log(message);
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
            _console.text += message;            
#endif

        }
    }

    public struct PlayerData
    {
        public float PositionX;
        public float PositionZ;
        public float RotationY;
        public float Hp;

        public static PlayerData Create(PlayerControls player)
        {
            return new PlayerData
            {
                PositionX = player.transform.position.x,
                PositionZ = player.transform.position.z,
                RotationY = player.transform.eulerAngles.y,
                Hp = player.Health
            };
        }

        public void Set(PlayerControls player)
        {
            var vector = player.transform.position;
            vector.x = PositionX;
            vector.z = PositionZ;
            player.transform.position = vector;
            vector = player.transform.eulerAngles;
            vector.y = RotationY;
            player.transform.eulerAngles = vector;
            player.Health = Hp;
        }

        public static byte[] SerializePlayerData(object data)
        {
            var player = (PlayerData)data;
            List<byte> arr = new List<byte>();
            arr.AddRange(BitConverter.GetBytes(player.PositionX));
            arr.AddRange(BitConverter.GetBytes(player.PositionZ));
            arr.AddRange(BitConverter.GetBytes(player.RotationY));
            arr.AddRange(BitConverter.GetBytes(player.Hp));
            return arr.ToArray();
        }


        public static object DeserializePlayerData(byte[] data)
        {            
            return new PlayerData
            {
                PositionX=BitConverter.ToSingle(data,0),
                PositionZ=BitConverter.ToSingle(data,4),
                RotationY=BitConverter.ToSingle(data,8),
                Hp=BitConverter.ToSingle(data,12),
            };
        }
    }
}
