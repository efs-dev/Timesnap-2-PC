using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
#endif

using UnityEngine.Audio;
using UnityEngine.Networking;
using System.Linq;

using UnityEngine.SceneManagement;


public class AudioSaveData
{
    public Dictionary<string, AudioSaveDataEntry> Entries = new Dictionary<string, AudioSaveDataEntry>();
}

public class AudioSaveDataEntry
{
    public string Id;
    public string Group;
    public string MixerId;
    public string Path;
    public double Volume;
    public int AudioType;
}


public partial class AudioManager : MonoBehaviour
{
    public static List<int> AudioIdsToKill = new List<int>();

    public static AudioManager GetInstance() { return _instance; }
    private static AudioManager _instance;

    public static bool debug = false;

    void Start()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        Init();
    }

    void OnSceneUnloaded(Scene oldScene)
    {
        Debug.Log("scene unloaded");

        AudioIdsToKill.ForEach(id => Stop(id));
        AudioIdsToKill.Clear();
    }

    public static void Init()
    {
        var data = LitJson.JsonMapper.ToObject<AudioSaveData>(Resources.Load<TextAsset>("AudioData").text);

        foreach (var entry in data.Entries.Values)
        {
            //Debug.Log("adding entry: " + entry.Id + " - " + entry.Path);
#if UNITY_EDITOR || UNITY_STANDALONE
            RegisterStreamingAudio(entry.Id, entry.Group, entry.MixerId, entry.Path.Replace(".mp3", "").Replace(".ogg", "") + ".mp3", 1); //RegisterStreamingAudio(entry.Id, entry.Group, entry.MixerId, entry.Path.Replace(".mp3", "").Replace(".ogg", "") + ".ogg", 1);
#else
            RegisterStreamingAudio(entry.Id, entry.Group, entry.MixerId, entry.Path.Replace(".mp3", "").Replace(".ogg", "") + ".mp3", 1);
#endif
        }
    }

#if UNITY_EDITOR
    [MenuItem("AudioManager/GenerateJSON")]
    static void GenerateJSON()
    {
        var data = new AudioSaveData();
        var searchPath = "Assets/StreamingAssets/Audio";

        Directory.GetDirectories(searchPath).ToList().ForEach(directory =>
        {
            var directoryInfo = new DirectoryInfo(directory);

            directoryInfo.GetFiles("*.mp3").ToList().ForEach(mp3 =>
            {
                var entry = new AudioSaveDataEntry();
                entry.Id = mp3.Name.Replace(".mp3", "");
                entry.Group = "SFX";
                entry.MixerId = "";
                entry.Path = directoryInfo.Name + "/" + mp3.Name;

                data.Entries.Add(entry.Id, entry);
            });
        });

        var json = LitJson.JsonMapper.ToJson(data);

        File.WriteAllText("Assets/Timesnap/Titles/Timesnap1/Audio/Resources/AudioData.txt", json);
    }

    public static void PreprocessStreamingDialogs()
    {
        var path = Application.dataPath + "/StreamingAssets/Audio/Dialogs";
        var files = Directory.GetFiles(path, "*.ogg", SearchOption.AllDirectories);

        for (var i = 0; i < files.Length; i++)
        {
            var file = files[i];

            var audioManager = FindObjectOfType<AudioManager>();
            audioManager.StartCoroutine(OnProcessDialogAudioFile(file));
        }
    }

    private static IEnumerator OnProcessDialogAudioFile(string file)
    {
        var audioManager = FindObjectOfType<AudioManager>();
        yield return RegisterStreamingAudio(file, "file:///" + file, false, AudioType.OGGVORBIS);

        var audioData = GetRegisteredAudio(file);
        while (audioData.Clip.length == 0)
        {
            yield return new WaitForEndOfFrame();
        }
        var volumes = audioData.GetSampleVolumes(400);


        // var filenameStart = file.LastIndexOf("\\");
        var filenameEnd = file.LastIndexOf(".");
        var filename = file.Substring(0, filenameEnd) + ".txt";
        // var sr = File.CreateText(Application.dataPath + "/Game/Data/DialogVolumes/" + filename);
        var bytes = new byte[volumes.Length];
        for (var i = 0; i < volumes.Length; i++)
        {
            var vol = volumes[i];
            byte val = byte.Parse((vol <= 0.0002f ? 0 : 1).ToString());
            bytes[i] = val;
        }

        File.WriteAllBytes(filename, bytes);
        //  sr.Close();
        UnregisterAudio(file);
    }
#endif

    public static string GetStreamingPath()
    {
        string path;
#if UNITY_EDITOR_WIN
        path = "file:" + Application.dataPath + "/StreamingAssets";
#elif UNITY_EDITOR_OSX
	    path = "file://" + Application.dataPath + "/StreamingAssets";
#elif UNITY_ANDROID
     path = "jar:file://"+ Application.dataPath + "!/assets/";
#elif UNITY_IOS
     path = "file://" + Application.streamingAssetsPath;
#elif UNITY_STANDALONE_WIN
     path = "file:" + Application.dataPath + "/StreamingAssets";
#elif UNITY_STANDALONE_OSX
     path = "file:" + Application.dataPath + "/Resources/Data/StreamingAssets";
#elif UNITY_WEBGL
     path = Application.streamingAssetsPath;
#else
        //Desktop (Mac OS or Windows)
        path = "StreamingAssets";
#endif
        return path;
    }

    IEnumerator TestStreaming()
    {
        yield return RegisterStreamingAudio("test", "file:///" + Application.streamingAssetsPath + "/Audio/Dialogs/p2_huf_01/p2_huf_001_n01_p0.ogg", true, AudioType.OGGVORBIS);
        Play("test", "test");
    }

    public delegate void OnVolumeChangeHandler();
    public static OnVolumeChangeHandler OnVolumeChange;

    private static float _volume = 1;
    public static float Volume
    {
        get
        {
            return _volume;
        }
        set
        {
            _volume = value;
            foreach (var group in _audioGroups.Values)
                group.RefreshVolume();

            if (OnVolumeChange != null)
                OnVolumeChange();
        }
    }
    private static bool _isMute;
    public static bool IsMute
    {
        get
        {
            return _isMute;
        }
        set
        {
            _isMute = value;
            foreach (var group in _audioGroups.Values)
                group.RefreshVolume();

            if (OnVolumeChange != null)
                OnVolumeChange();
        }
    }

    private static int _uniqueId;
    public static int GetUniqueId()
    {
        return _uniqueId++;
    }

    private static Dictionary<string, AudioData> _data = new Dictionary<string, AudioData>();
    private static Dictionary<string, AudioGroup> _audioGroups = new Dictionary<string, AudioGroup>();

    private void Awake()
    {
        _instance = this;
        Volume = 1;

        if (Application.isPlaying)
            DontDestroyOnLoad(gameObject);
    }

    public static void RegisterAudio(string id, AudioClip clip, bool removeOnComplete = false)
    {
        var audioData = new AudioData(id, clip, removeOnComplete);
        _data[audioData.Id] = audioData;
    }

    public static void RegisterAudio(string id, string group, string mixerId, float volume)
    {
        var audioData = new AudioData(id, group, mixerId, volume);
        _data[audioData.Id] = audioData;
    }

    public static void RegisterAudio(string id, string group, string mixerId, AudioClip clip, float volume)
    {
        var audioData = new AudioData(id, group, mixerId, clip, volume);
        _data[audioData.Id] = audioData;
    }

    public static void RegisterAudio(string id, string group, string mixerId, string path, float volume)
    {
        var audioData = new AudioData(id, group, mixerId, path, volume);
        _data[audioData.Id] = audioData;
    }

    public static IEnumerator RegisterStreamingAudio(string id, string path, bool useStream = true, AudioType type = AudioType.UNKNOWN)
    {
        var www = new WWW(path);
        yield return www;

        if (www.error != null)
            Debug.Log(www.error);

        AudioClip audioClip;
        if (type == AudioType.UNKNOWN)
            audioClip = www.GetAudioClip(false, useStream, type);
        else
            audioClip = www.GetAudioClip(false, useStream);

        audioClip.LoadAudioData();
        RegisterAudio(id, audioClip, true);
    }

    public static void RegisterStreamingAudio(string id, string group, string mixerId, string path, float volume, bool useStream = true, AudioType type = AudioType.UNKNOWN)
    {
        RegisterAudio(id, group, mixerId, path, volume);
    }

    public static void UnregisterAudio(string id)
    {
        id = id.ToUpper();
        if (_data.ContainsKey(id))
            _data.Remove(id);
    }

    public static AudioData GetRegisteredAudio(string id)
    {
        id = id.ToUpper();
        if (!_data.ContainsKey(id))
            return null;
        return _data[id];
    }

    public static Audio Play(string id, int loops = 0, float volume = 1, bool ignoreNonExplicitStops = false)
    {
        var data = GetRegisteredAudio(id);
        Audio audio = null;

        try
        {
            if (!_audioGroups.ContainsKey(data.Group.ToUpper()))
                _audioGroups.Add(data.Group.ToUpper(), new AudioGroup(data.Group.ToUpper()));

            var group = _audioGroups[data.Group.ToUpper()];

            _instance.StartCoroutine(group.Play(id, (value) => audio = value, loops, volume, data.MixerId));
        }
        catch (System.Exception ex)
        {
            if (debug) Debug.LogError("AudioManager.Play Caught NullRefEx for id: " + id + "\n" + ex);
        }

        audio.IgnoreNonExplicitStops = ignoreNonExplicitStops;
        return audio;
    }

    public static Audio Play(string id, string groupId, int loops = 0, float volume = 1, string mixerId = null)
    {
        groupId = groupId.ToUpper();
        if (!_audioGroups.ContainsKey(groupId))
            _audioGroups.Add(groupId, new AudioGroup(groupId));

        var group = _audioGroups[groupId];

        Audio audio = null;
        _instance.StartCoroutine(group.Play(id, (value) => audio = value, loops, volume, mixerId));
        return audio;
    }

    public static IEnumerator PlayAndWait(string id, int loops = 0, float volume = 1)
    {
        var data = GetRegisteredAudio(id);

        if (!_audioGroups.ContainsKey(data.Group.ToUpper()))
            _audioGroups.Add(data.Group.ToUpper(), new AudioGroup(data.Group.ToUpper()));

        var group = _audioGroups[data.Group.ToUpper()];
        yield return group.Play(id, (value) => { }, loops, volume, data.MixerId);
    }

    public static IEnumerator PlayAndWait(string id, string groupId, int loops = 0, float volume = 1, string mixerId = null)
    {
        groupId = groupId.ToUpper();
        if (!_audioGroups.ContainsKey(groupId))
            _audioGroups.Add(groupId, new AudioGroup(groupId));


        var group = _audioGroups[groupId];
        yield return group.Play(id, (value) => { }, loops, volume, mixerId);
    }

    public static void Pause(string id)
    {
        foreach (var group in _audioGroups.Values)
            group.Pause(id);
    }

    public static void Pause(int instanceId)
    {
        foreach (var group in _audioGroups.Values)
            group.Pause(instanceId);
    }

    public static void Unpause(string id)
    {
        foreach (var group in _audioGroups.Values)
            group.UnPause(id);
    }

    public static void Unpause(int instanceId)
    {
        foreach (var group in _audioGroups.Values)
            group.UnPause(instanceId);
    }

    public static void SetAudioVolume(string id, float volume)
    {
        foreach (var group in _audioGroups.Values)
            group.SetAudioVolume(id, volume);
    }

    public static void Stop(string id)
    {
        foreach (var group in _audioGroups.Values)
            group.Stop(id);
    }

    public static void Stop(int instanceId)
    {
        var groups = new AudioGroup[_audioGroups.Count];
        _audioGroups.Values.CopyTo(groups, 0);
        for (var i = 0; i < groups.Length; i++)
            groups[i].Stop(instanceId);
    }

    public static void PauseGroup(string groupId)
    {
        _audioGroups[groupId.ToUpper()].PauseAll();
    }

    public static void UnpauseGroup(string groupId)
    {
        _audioGroups[groupId.ToUpper()].UnpauseAll();
    }

    public static void StopGroup(string groupId, bool forceAll = false)
    {
        if (!_audioGroups.ContainsKey(groupId.ToUpper()))
            return;

        _audioGroups[groupId.ToUpper()].StopAll(forceAll);
    }

    public static AudioGroup GetGroup(string groupId)
    {
        if (!_audioGroups.ContainsKey(groupId.ToUpper()))
            _audioGroups.Add(groupId, new AudioGroup(groupId.ToUpper()));

        return _audioGroups[groupId.ToUpper()];
    }

    public static void PauseAll()
    {
        foreach (var group in _audioGroups.Values)
            group.PauseAll();
    }

    public static void UnpauseAll()
    {
        foreach (var group in _audioGroups.Values)
            group.UnpauseAll();
    }

    public static void StopAll(bool forceAll = false)
    {
        foreach (var group in _audioGroups.Values)
            group.StopAll(forceAll);
    }

}

public class AudioGroup
{
    public string Id { get; private set; }
    public Dictionary<string, List<Audio>> _audioById = new Dictionary<string, List<Audio>>();
    public Dictionary<int, Audio> _audioByInstanceId = new Dictionary<int, Audio>();

    public Audio GetById(string id)
    {
        id = id.ToUpper();
        if (!_audioById.ContainsKey(id) || _audioById[id].Count == 0)
            return null;

        return _audioById[id][0];
    }

    private float _volume = 1;
    public float Volume
    {
        get
        {
            return _volume;
        }
        set
        {
            _volume = value;
            RefreshVolume();

            if (AudioManager.OnVolumeChange != null)
                AudioManager.OnVolumeChange();
        }
    }

    private float _isMute;
    public float IsMute
    {
        get
        {
            return _isMute;
        }
        set
        {
            _isMute = value;
            RefreshVolume();

            if (AudioManager.OnVolumeChange != null)
                AudioManager.OnVolumeChange();
        }
    }

    public void RefreshVolume()
    {
        foreach (var audio in _audioByInstanceId.Values)
            audio.RefreshVolume();
    }

    public IEnumerator Play(string id, System.Action<Audio> setAudio, int loops = 0, float volume = 1, string mixerId = null)
    {
        var audioData = AudioManager.GetRegisteredAudio(id);
        if (audioData == null)
        {
            Debug.LogError("Invalid audio id: " + id);
            yield break;
        }
        var audio = new Audio(AudioManager.GetUniqueId(), this, audioData, loops, volume, mixerId);

        if (!_audioById.ContainsKey(audioData.Id))
            _audioById.Add(audioData.Id, new List<Audio>());

        _audioById[audioData.Id].Add(audio);
        _audioByInstanceId.Add(audio.InstanceId, audio);

        setAudio(audio);
        yield return audio.Play();
    }

    public void PauseAll()
    {
        foreach (var audio in _audioByInstanceId.Values)
            audio.Pause();
    }

    public void UnpauseAll()
    {
        foreach (var audio in _audioByInstanceId.Values)
            audio.Unpause();
    }

    public void Pause(string id)
    {
        id = id.ToUpper();
        if (!_audioById.ContainsKey(id))
            return;

        var list = _audioById[id];

        foreach (var audio in list)
            audio.Pause();
    }

    public void Pause(int instanceId)
    {
        if (!_audioByInstanceId.ContainsKey(instanceId))
            return;

        foreach (var audio in _audioByInstanceId.Values)
            audio.Pause();
    }

    public void UnPause(string id)
    {
        id = id.ToUpper();
        if (!_audioById.ContainsKey(id))
            return;

        var list = _audioById[id];

        foreach (var audio in list)
            audio.Unpause();
    }

    public void UnPause(int instanceId)
    {
        if (!_audioByInstanceId.ContainsKey(instanceId))
            return;

        foreach (var audio in _audioByInstanceId.Values)
            audio.Unpause();
    }

    public void SetAudioVolume(string id, float volume)
    {
        id = id.ToUpper();
        if (!_audioById.ContainsKey(id))
            return;

        var list = _audioById[id];

        foreach (var audio in list)
            audio.Volume = volume;
    }


    public void Stop(string id)
    {
        id = id.ToUpper();
        if (!_audioById.ContainsKey(id))
            return;

        var list = _audioById[id];

        for (var i = 0; i < list.Count; i++)
        {
            Stop(list[i]);
        }
    }


    public void Stop(Audio audio)
    {
        _audioById[audio.Data.Id].Remove(audio);
        _audioByInstanceId.Remove(audio.InstanceId);
        audio.Dispose();
    }

    public void Stop(int instanceId)
    {
        if (!_audioByInstanceId.ContainsKey(instanceId))
            return;

        var list = new Audio[_audioByInstanceId.Count];
        _audioByInstanceId.Values.CopyTo(list, 0);
        foreach (var audio in list)
            Stop(audio);
    }

    public void StopAll(bool forceAll = false)
    {
        var audioFiles = new List<Audio>();
        foreach (var list in _audioById.Values)
            foreach (var audio in list)
                audioFiles.Add(audio);

        for (var i = 0; i < audioFiles.Count; i++)
        {
            var audio = audioFiles[i];
            if (forceAll || !audio.IgnoreNonExplicitStops)
                audioFiles[i].Stop();
        }
    }

    public AudioGroup(string id)
    {
        Id = id.ToUpper();
        Volume = 1;
    }
}

public class AudioData
{
    public string Id { get; private set; }
    public float Volume { get; private set; }
    public string Group { get; private set; }
    public string MixerId { get; private set; }
    public AudioClip Clip { get; private set; }
    public string Path { get; private set; }
    public bool RemoveOnComplete { get; private set; }

    public AudioData(string id, string group, string mixerId, float volume)
    {
        Id = id.ToUpper();
        Group = group;
        MixerId = mixerId;
        Volume = volume;
    }

    public AudioData(string id, bool removeOnComplete, float volume = 1)
    {
        Id = id.ToUpper();
        RemoveOnComplete = removeOnComplete;
        Volume = volume;
    }

    public AudioData(string id, AudioClip clip, bool removeOnComplete, float volume = 1)
    {
        Id = id.ToUpper();
        Clip = clip;
        RemoveOnComplete = removeOnComplete;
        Volume = volume;
    }

    public AudioData(string id, string group, string mixerId, AudioClip clip, float volume)
    {
        Id = id.ToUpper();
        Group = group;
        MixerId = mixerId;
        Clip = clip;
        Volume = volume;
    }

    public AudioData(string id, string group, string mixerId, string path, float volume)
    {
        Id = id.ToUpper();
        Group = group;
        MixerId = mixerId;
        Path = path;
        Volume = volume;
    }

    public void Dispose()
    {
        AudioManager.UnregisterAudio(Id);
        Id = null;
        Clip = null;
    }



    public float GetVolumeAtSample(int sample)
    {
        var vol = 0f;

        if (sample <= Clip.samples)
        {
            var samples = new float[Clip.samples - sample];
            Clip.GetData(samples, sample);
            vol = Mathf.Max(Mathf.Abs(samples[0]), Mathf.Abs(samples[1]));
        }

        return vol;
    }

    public float[] GetSampleVolumes(int scale)
    {
        var numSamples = Clip.samples / scale;
        var volumes = new float[numSamples];
        var samples = new float[Clip.samples];
        Clip.GetData(samples, 0);

        for (var i = 0; i < numSamples; i++)
        {
            var vol = Mathf.Max(Mathf.Abs(samples[i * scale]), Mathf.Abs(samples[i * scale + 1]));
            volumes[i] = vol;
        }

        return volumes;
    }
}


public class Audio
{
    public int InstanceId { get; private set; }

    public AudioGroup Group { get; private set; }

    public string MixerId { get; private set; }

    public bool IgnoreNonExplicitStops;

    private float _volume = 1;
    public float Volume
    {
        get
        {
            return _volume;
        }
        set
        {
            _volume = value;
            RefreshVolume();
        }
    }

    private bool _isMute;
    public bool IsMute
    {
        get
        {
            return _isMute;
        }
        set
        {
            _isMute = value;
            RefreshVolume();
        }
    }

    public AudioData Data { get; private set; }
    public AudioSource Source { get; private set; }

    public int Loops { get; private set; }
    public int LoopsRemaining { get; private set; }

    public bool IsComplete { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsDisposed { get; private set; }


    public delegate void AudioEvent(Audio audio);
    public event AudioEvent OnAudioFinished;

    private bool _hasStarted;

    public void RefreshVolume()
    {
        if (Source != null)
            Source.volume = IsMute ? 0 : Data.Volume * Volume * Group.Volume * AudioManager.Volume;
    }

    public IEnumerator Play()
    {
        if (IsPaused)
        {
            Unpause();
            yield break;
        }

        RefreshVolume();


        if (Data.Clip == null && Data.Path != null)
        {
            Debug.Log("load audio clip");
            yield return LoadAudioClip(Data.Path, (clip) => Source.clip = clip);

            Debug.Log("clip: " + Source.clip);
        }
        else if (Data.Clip != null)
            Source.clip = Data.Clip;

        if (Source == null || Source.clip == null)
        {
            Debug.Log(Data.Id);
            Debug.LogError("source.clip was null");
            yield break;
        }

        //if (LoopsRemaining != 0)
        //    Source.loop = true;
        Source.Play();



        if (LoopsRemaining == 0)
        {
            while (true)
            {
                if (!_hasStarted && Source.time > 0)
                    _hasStarted = true;
                else if (_hasStarted && Source.time == 0)
                    break;

                yield return new WaitForEndOfFrame();


                if (IsDisposed)
                    yield break;
            }


            /*
            if (LoopsRemaining > 0)
                LoopsRemaining--;

            if (LoopsRemaining != 0)
            {
                Play();
                yield break;
            }*/

            var data = Data;
            IsComplete = !IsDisposed;

            Group.Stop(this);

            if (data.RemoveOnComplete)
            {
                data.Dispose();
            }
        }
        else
        {
            while (true)
            {
                if (Source == null)
                    yield break;

                //Debug.Log("_hasStarted = " + _hasStarted + ", Source.time = " + Source.time);
                if (!_hasStarted && Source.time > 0)
                    _hasStarted = true;
                else if (_hasStarted && Source.time == 0)
                {
                    if (IsDisposed)
                    {
                        //Debug.Log("Is Disposed so breaking out of loop music");
                        yield break;
                    }

                    _hasStarted = false;
                    Source.time = 0;
                    Source.Play();
                    //Debug.Log("new loop");
                }

                yield return new WaitForEndOfFrame();
            }

        }

    }

    public void Pause()
    {
        IsPaused = true;
        Source.Pause();
    }

    public void Unpause()
    {
        IsPaused = false;
        Source.UnPause();
    }

    public void Stop()
    {
        Group.Stop(this);
    }

    public void Dispose()
    {
        OnAudioFinished?.Invoke(this);
        Data = null;
        Group = null;
        Source.Stop();
        Source.clip = null;
        GameObject.DestroyImmediate(Source);
        IsDisposed = true;
    }

    public Audio(int instanceId, AudioGroup group, AudioData data, int loops = 0, float volume = 1, string mixerId = null)
    {
        InstanceId = instanceId;
        Data = data;
        Group = group;
        Loops = loops;
        LoopsRemaining = Loops;
        Volume = volume;
        Source = AudioManager.GetInstance().gameObject.AddComponent<AudioSource>();
        MixerId = mixerId;
        /*
        if (MixerId != null)
        {
            try
            {
                var mixers = Resources.Load<AudioMixer>("audiomanagermixer").FindMatchingGroups(mixerId == null ? Data.MixerId : mixerId);
                if (mixers.Length > 0)
                    Source.outputAudioMixerGroup = mixers[0];
                else
                    Debug.LogError("Invalid mixer id " + mixerId + " for audio instanceId " + instanceId);
            } catch (System.Exception ex)
            {
                Debug.LogError("error creating audio mixer: " + ex.Message);
            }
        }
        */
    }


    public IEnumerator LoadAudioClip(string path, Action<AudioClip> callback)
    {
        if (path.StartsWith("/"))
            path = path.Remove(0, 1);

        Debug.Log("trying to load audio clip");
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(AudioManager.GetStreamingPath() + "/Audio/" + path, AudioType.MPEG))
        {
            Debug.Log("sending audio clip request " + www.url);
            yield return www.SendWebRequest();
            Debug.Log("got audio clip request");

            /*
        WWW www = new WWW(GetStreamingPath() + "/" + path);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
            Debug.LogError("Error loading audio at path " + www.url + " with error: " + www.error);
            */
            AudioClip clip = null;

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("getting audioclip content");
                clip = DownloadHandlerAudioClip.GetContent(www);
                Debug.Log("clip: " + clip);
            }
            Debug.Log("clip: " + clip);
            clip.LoadAudioData();
            callback(clip);
        }
        yield break;
    }
}