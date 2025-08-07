// Shared/ISoundService.cs

using UnityEngine;

public interface ISoundService
{
    AudioSource PlaySfx(string id,bool loop=false);
    void StopSfx(string id);
    void PlayMusic(string id, bool loop = true);
    void StopMusic();
}