using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Asset
{
    public class ResourceExtensions
    {
        public static string _BaseExt = ".prefab";

        public static readonly Dictionary<Type, List<string>> TypeExtentions = new Dictionary<Type, List<string>>
        {
            [typeof(GameObject)] = new List<string>{ ".prefab" },
            [typeof(Material)] = new List<string> { ".mat" },
            [typeof(Texture)] = new List<string> { ".png", ".tga" },
            [typeof(Texture2D)] = new List<string> { ".png", ".tga" },
            [typeof(Sprite)] = new List<string> { ".png", ".tga", ".jpg" },
            [typeof(Shader)] = new List<string> { ".shader" },
            [typeof(RuntimeAnimatorController)] = new List<string> { ".controller" },
            [typeof(AnimationClip)] = new List<string> { ".anim" },
            [typeof(TextAsset)] = new List<string> { ".json", ".txt", ".bytes", },
            [typeof(AudioClip)] = new List<string> { ".wav", ".ogg", ".mp3" },
        };

        public static List<string> GetExtOfType(Type t)
        {
            if (TypeExtentions.ContainsKey(t))
                return TypeExtentions[t];

            return null;
        }
    }
}
