using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.Assertions;
using System;


public enum RANDOM_SOUND_TYPE { LOOSE };

[System.Serializable]
public class RandomSound
{
    [SerializeField]
    RANDOM_SOUND_TYPE m_type;
    [SerializeField]
    List<AudioClip> m_listSounds;
    List<AudioClip> m_everUsed = new List<AudioClip>();
    int m_audioSourceKey = (int)AUDIOSOURCE_KEY.CREATE_KEY;

    #region GetterSetter
    public List<AudioClip> ListSounds
    {
        get
        {
            return m_listSounds;
        }

        private set
        {
            m_listSounds = value;
        }
    }

    public RANDOM_SOUND_TYPE Type
    {
        get
        {
            return m_type;
        }

        private set
        {
            m_type = value;
        }
    }

    public List<AudioClip> EverUsed
    {
        get
        {
            return m_everUsed;
        }

        private set
        {
            m_everUsed = value;
        }
    }

    public int AudioSourceKey
    {
        get
        {
            return m_audioSourceKey;
        }

        set
        {
            m_audioSourceKey = value;
        }
    }
    #endregion
}

public enum MIXER_GROUP_TYPE { AMBIANT, SFX_MENU, SFX_GOOD, SFX_BAD , SFX, MASTER};

[System.Serializable]
public class MixerGroupLink
{
    [SerializeField]
    MIXER_GROUP_TYPE m_mixerType;
    [SerializeField]
    AudioMixerGroup m_mixerGroup;

    [SerializeField]
    string m_volumeVariable;

    public MIXER_GROUP_TYPE MixerType
    {
        get
        {
            return m_mixerType;
        }

        private set
        {
            m_mixerType = value;
        }
    }

    public AudioMixerGroup MixerGroup
    {
        get
        {
            return m_mixerGroup;
        }

        private set
        {
            m_mixerGroup = value;
        }
    }

    public string VolumeVariable
    {
        get
        {
            return m_volumeVariable;
        }
    }
}

[System.Serializable]
public class AudioClipLink
{
    [SerializeField]
    AUDIOCLIP_KEY m_key;
    [SerializeField]
    AudioClip m_audioClip;

    public AUDIOCLIP_KEY Key
    {
        get
        {
            return m_key;
        }
    }

    public AudioClip AudioClip
    {
        get
        {
            return m_audioClip;
        }
    }
}


public class AudioSourceExtend
{
    AudioSource m_audioSource;
    bool m_autoDestroy;
    float m_speed;
    int m_key;
    float m_currentTime;

#region GetterSetter
    public bool IsDestroyed
    {
        get
        {
            return m_audioSource == null;
        }
    }

    public AudioSource AudioSource
    {
        get
        {
            return m_audioSource;
        }

        set
        {
            m_audioSource = value;
        }
    }

    public bool AutoDestroy
    {
        get
        {
            return m_autoDestroy;
        }

        set
        {
            m_autoDestroy = value;
        }
    }

    public float Speed
    {
        get
        {
            return m_speed;
        }

        set
        {
            m_currentTime = 0;
            m_speed = value;
        }
    }

    public int Key
    {
        get
        {
            return m_key;
        }

        set
        {
            m_key = value;
        }
    }

    #endregion


    public AudioSourceExtend(AudioSource a_audioSource)
    {
        m_audioSource = a_audioSource;
    }

    public void Update()
    {
        if(Time.deltaTime == 0)
        {
            m_currentTime += Mathf.Abs(Speed) *  1 / 60f;
        }
        else
        {
            m_currentTime += Mathf.Abs(Speed) * Time.deltaTime;
        }

        m_currentTime += Mathf.Abs(Speed) * Time.deltaTime;
        AudioSource.volume = ConcreteEaseMethods.ExpoEaseOut(m_currentTime, Speed > 0 ? 0 : 1, Speed > 0 ? 1 : -1, 1);
        TryToDestroy();
    }

    void TryToDestroy()
    {
        if (AutoDestroy && AudioSource  && AudioSource.clip != null && (( AudioSource.time >= AudioSource.clip.length - 0.001 && !AudioSource.loop) || Mathf.Approximately(AudioSource.volume, 0)))
        {
            GameObject.DestroyImmediate(AudioSource);
        }
    }

    public void Stop()
    {
        if (m_audioSource)
        {
            m_audioSource.Stop();
        }
    }
}


public enum AUDIOSOURCE_KEY { BACKGROUND, BUTTON_MENU, NO_KEY_AUTODESTROY, CREATE_KEY };

public enum AUDIOCLIP_KEY{ BONUS_PICKED, BONUS_USED, ENEMY_DIE, ENEMY_FIRE, HITTED , LOOSE, WIN, ENNEMY_HITTED, BUTTON_MENU, RELOADED, COUNTDOWN_CLASSIC, COUNTDOWN_FINAL };


public class SoundManager : Singleton<SoundManager>
{

    /// <summary>
    /// Store mixers of game
    /// </summary>
    [SerializeField]
    List<MixerGroupLink> m_listMixerGroup;

    /// <summary>
    /// Store audioClip
    /// </summary>
    [SerializeField]
    List<AudioClipLink> m_listAudioClip;


    /// <summary>
    /// Store set of random sounds
    /// </summary>
    [SerializeField]
    List<RandomSound> m_listRandomSound;

    /// <summary>
    /// Store Audio Source Extend with their keys
    /// </summary>
    Dictionary<int, AudioSourceExtend> m_audioSourcesExtendWithKey = new Dictionary<int, AudioSourceExtend>();
    /// <summary>
    /// Store all AudioSource Extend know
    /// </summary>
    List<AudioSourceExtend> m_audioSourcesExtend = new List<AudioSourceExtend>();


    /// <summary>
    /// Key to allocate
    /// </summary>
    int m_maxAllocatedKey = (int)AUDIOSOURCE_KEY.CREATE_KEY;

    /// <summary>
    /// Step to fade sounds
    /// </summary>
    [SerializeField]
    float m_speedFade = 0.1f;


    void Update()
    {


        foreach (AudioSourceExtend audioSourceExtend in m_audioSourcesExtend)
        {
            audioSourceExtend.Update();
            if (audioSourceExtend.IsDestroyed)
            {
                if (m_audioSourcesExtendWithKey.ContainsKey(audioSourceExtend.Key) && m_audioSourcesExtendWithKey[audioSourceExtend.Key] == audioSourceExtend)
                {
                    m_audioSourcesExtendWithKey.Remove(audioSourceExtend.Key);
                }
            }
        }
        m_audioSourcesExtend.RemoveAll(x => x.IsDestroyed);
    }

    public void SetMixerVolume(MIXER_GROUP_TYPE a_mixerGroupType, float a_volume)
    {
        MixerGroupLink mixer = m_listMixerGroup.Find(x => x.MixerType == a_mixerGroupType);
        Assert.AreNotEqual(mixer, null, "MixerGroup don't exist");

        mixer.MixerGroup.audioMixer.SetFloat(mixer.VolumeVariable, a_volume);

    }

    public float GetMixerVolume(MIXER_GROUP_TYPE a_mixerGroupType)
    {
        float res = 0;
        MixerGroupLink mixer = m_listMixerGroup.Find(x => x.MixerType == a_mixerGroupType);
        Assert.AreNotEqual(mixer, null, "MixerGroup don't exist");

         mixer.MixerGroup.audioMixer.GetFloat(mixer.VolumeVariable, out res);
        return res;
    }

    /// <summary>
    /// Check if an Audio is actually playing
    /// </summary>
    /// <param name="a_key">Key of the audio</param>
    /// <returns>Do the audio playing</returns>
    public bool IsAudioPlaying(int a_key)
    {
        bool res = false;
        AudioSourceExtend output;
        if(m_audioSourcesExtendWithKey.TryGetValue(a_key, out output))
        {
            res = output.AudioSource.isPlaying;
        }

        return res;
    }

    /// <summary>
    /// Get audiosourceassociate to a key if it not exist will create a new depend of parameter
    /// </summary>
    /// <param name="a_key">key of the audio</param>
    /// <param name="out_res">out</param>
    /// <param name="a_createIfNotPresent">Do we create a new source if not exist</param>
    /// <returns>True if the key ever exist, false if not</returns>
    bool GetAudioSource(int a_key, out AudioSourceExtend out_res, bool a_createIfNotPresent = true)
    {
        Assert.IsFalse(a_key < 0 || a_key > m_maxAllocatedKey, "Bad sound key");

        bool res = true;
        if (!m_audioSourcesExtendWithKey.TryGetValue(a_key, out out_res))
        {
            res = false;
            if (a_createIfNotPresent)
            {
                out_res = new AudioSourceExtend(CreateAudioSource());
                if (a_key != (int)AUDIOSOURCE_KEY.NO_KEY_AUTODESTROY)
                {
                    m_audioSourcesExtendWithKey.Add(a_key, out_res);
                    out_res.Speed = -m_speedFade;
                    out_res.AutoDestroy = true;
                }
                else
                {
                    out_res.AutoDestroy = true;
                }
                out_res.Key = a_key;
                m_audioSourcesExtend.Add(out_res);
            }

        }

  
        return res;
    }

    /// <summary>
    /// Create an audio source with looping true and not playing
    /// </summary>
    /// <returns>an Audios ource</returns>
    AudioSource CreateAudioSource()
    {
        AudioSource res;
        res = gameObject.AddComponent<AudioSource>();
        res.loop = true;
        res.Stop();
        return res;
    }


    /// <summary>
    /// depend of the key create a new one or just return this key.
    /// </summary>
    /// <param name="a_key">the key</param>
    /// <returns>key created</returns>
    int GetKey(int a_key)
    {
        Assert.IsFalse(a_key < 0 || a_key > m_maxAllocatedKey, "Bad sound key");
  
        if (a_key == (int)AUDIOSOURCE_KEY.CREATE_KEY)
        {
            a_key = ++m_maxAllocatedKey;
        }
        return a_key;
    }

    /// <summary>
    /// Get a key to keep tracking of audiosource
    /// </summary>
    /// <returns>a key</returns>
    public int GenerateKey()
    {
        return GetKey((int)AUDIOSOURCE_KEY.CREATE_KEY);
    }


    public int StartAudio(AUDIOCLIP_KEY a_clipKey, MIXER_GROUP_TYPE a_mixerGroupType = MIXER_GROUP_TYPE.AMBIANT, bool a_isFading = true, bool a_isLooping = true, AUDIOSOURCE_KEY a_key = AUDIOSOURCE_KEY.CREATE_KEY, ulong a_delay = 0)
    {
        AudioClipLink audioClip = m_listAudioClip.Find(o => o.Key == a_clipKey);

        Assert.IsFalse(audioClip == null, "Bad audioclip key");

        return StartAudio(audioClip.AudioClip,a_mixerGroupType, a_isFading,a_isLooping,a_key,a_delay);

    }

    //TODO see to move initialisation of audiosource elsewhere - store key inside audio extends instead of dictionnary?
    //TODO : Store a part of audioclips here and associate key

    /// <summary>
    /// Start an audio depending of parameters - TODO simplify it and move elswhere some logic
    /// </summary>
    /// <param name="a_clip">Clip we want to play</param>
    /// <param name="a_mixerGroupType">Mixer we want to output</param>
    /// <param name="a_isFading">If the clip is fading</param>
    /// <param name="a_isLooping">If the clip it autolooping</param>
    /// <param name="a_key">the key we want</param>
    /// <param name="a_delay">if we play with a delay</param>
    /// <returns></returns>
     public int StartAudio(AudioClip a_clip, MIXER_GROUP_TYPE a_mixerGroupType = MIXER_GROUP_TYPE.AMBIANT, bool a_isFading = true, bool a_isLooping = true, AUDIOSOURCE_KEY a_key = AUDIOSOURCE_KEY.CREATE_KEY, ulong a_delay = 0)
     {
        int res = -1;
        int key = (int)a_key;
        res = GetKey(key);
        AudioMixerGroup mixer = null;
        try
        {
            mixer = m_listMixerGroup.Find(x => x.MixerType == a_mixerGroupType).MixerGroup;
            AudioSourceExtend source;

            if (GetAudioSource(key, out source))
            {
                if(source.AudioSource.clip == a_clip)
                {
                    Debug.Log("Clip ever play : " + a_clip.name);
                    return key;
                }
                AudioSourceExtend new_source = new AudioSourceExtend(CreateAudioSource());
                m_audioSourcesExtend.Add(new_source);
                m_audioSourcesExtendWithKey[key] = new_source;
                if (a_isFading)
                {
                    source.Speed = -m_speedFade;
                }
                else
                {
                    source.Stop();
                }
                source.AutoDestroy = true;
                AudioSourceExtend temp = source;
                source = new_source;
                new_source = temp;
            }

            if (a_isFading)
            {

                source.Speed = m_speedFade;
                source.AudioSource.volume = 0;
            }

            source.AudioSource.outputAudioMixerGroup = mixer;
            source.AudioSource.clip = a_clip;
            source.AudioSource.loop = a_isLooping;
            source.AudioSource.Play(a_delay);

            Debug.Log("StartAudio : " + a_clip.name);
        }
        catch
        {
            Debug.LogError("Error when try to launch sound");
            if(mixer == null)
            {
                Debug.LogError("Mixer doesn't exist");
            }
        }
        return res;
     }

    /// <summary>
    /// Stop the audio specify if exist
    /// </summary>
    /// <param name="a_key">key of the audio we want to stop</param>
    public void StopAudio(int a_key = (int)AUDIOSOURCE_KEY.CREATE_KEY)
    {
        AudioSourceExtend audioSourceExtend;
        if (Instance.m_audioSourcesExtendWithKey.TryGetValue(a_key, out audioSourceExtend))
        {
            audioSourceExtend.Stop();
            Debug.Log("StopAudio : " + a_key);
        }
    }

    /// <summary>
    /// Start an audio of a random bank
    /// </summary>
    /// <param name="a_randomSoundType">id of the random bank</param>
    /// <param name="a_mixerGroupType">id of the mixer we want to output through</param>
    public void StartRandom(RANDOM_SOUND_TYPE a_randomSoundType, MIXER_GROUP_TYPE a_mixerGroupType)
    {

        RandomSound randomSound = m_listRandomSound.Find(x => x.Type == a_randomSoundType);
        if (randomSound.ListSounds.Count > 0)
        {
            int rnd = (int)Utils.RandomFloat(0, randomSound.ListSounds.Count);
            AudioClip toPlay = randomSound.ListSounds[rnd];
            randomSound.AudioSourceKey = StartAudio(randomSound.ListSounds[rnd], a_mixerGroupType, false, false,(AUDIOSOURCE_KEY) randomSound.AudioSourceKey);
            randomSound.EverUsed.Add(toPlay);
            randomSound.ListSounds.Remove(toPlay);

            if (randomSound.ListSounds.Count == 0)
            {
                randomSound.ListSounds.AddRange(randomSound.EverUsed);
                randomSound.EverUsed.Clear();
            }
        }

    }



    /// <summary>
    /// Stop an audio with a key
    /// </summary>
    /// <param name="a_key">key of the audio we want to stop</param>
     public void StopAudioWithFadeOut(int a_key)
     {
        AudioSourceExtend audioSource;
        GetAudioSource(a_key, out audioSource,  false);
        audioSource.Speed = -m_speedFade;
        audioSource.AutoDestroy = true;
     }

}