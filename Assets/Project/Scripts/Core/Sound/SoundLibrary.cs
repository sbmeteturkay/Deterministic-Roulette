using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Core
{
    [System.Serializable]
    public class SoundEntry
    {
        public string id;
        public AudioClip clip;
    }
    [CreateAssetMenu(menuName = "Audio/SoundLibrary")]
    public class SoundLibrary : ScriptableObject
    {
        public List<SoundEntry> sounds=new();

        private Dictionary<string, AudioClip> _soundMap=new();

        public AudioClip GetClip(string id) => _soundMap[id];

        private void OnEnable()
        {
            _soundMap = sounds.ToDictionary(s => s.id, s => s.clip);
        }
    }
}