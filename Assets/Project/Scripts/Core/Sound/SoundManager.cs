using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    [Serializable]
    public class SoundManager : ISoundService
    {
        public AudioSource _sfxSource { get; set; }
        public AudioSource _musicSource { get; set; }
        public SoundLibrary _soundLibrary { get; set; }
        private Dictionary<string, AudioSource> _activeSfx = new();

        public SoundManager(AudioSource sfxSource, AudioSource musicSource,SoundLibrary soundLibrary)
        {
            _sfxSource = sfxSource;
            _musicSource = musicSource;
            _soundLibrary = soundLibrary;
        }


        public AudioSource PlaySfx(string id,bool loop=false)
        {
            var clip = _soundLibrary.GetClip(id);

            if (!_activeSfx.TryGetValue(id, out var source) || source == null)
            {
                source = _sfxSource.gameObject.AddComponent<AudioSource>();
                _activeSfx[id] = source;
            }

            source.clip = clip;
            source.loop = loop;
            source.Play();
            return source;
        }

        public void StopSfx(string id)
        {
            if (_activeSfx.TryGetValue(id, out var source))
            {
                source.Stop();
            }
        }
        public void PlayMusic(string musicId, bool loop = true)
        {
            var clip = _soundLibrary.GetClip(musicId);
            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            _musicSource.Stop();
        }
    }
}
