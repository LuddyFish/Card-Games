using System.Collections;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource[] sources;
    public AudioClip[] audios;

    // --- public API ---
    /// <summary>
    /// Play the whole length of <paramref name="clip"/>.
    /// </summary>
    /// <param name="player">Audio output source</param>
    /// <param name="clip">Audio file</param>
    /// <returns></returns>
    public Coroutine Play(AudioSource player, AudioClip clip)
        => StartCoroutine(PlayAudio(player, clip, 0f, clip.length));

    /// <summary>
    /// Play the length of <paramref name="clip"/> starting from <paramref name="start"/>.
    /// </summary>
    /// <param name="player">Audio output source</param>
    /// <param name="clip">Audio file</param>
    /// <param name="start">Start of the audio</param>
    /// <returns></returns>
    public Coroutine Play(AudioSource player, AudioClip clip, float start)
        => StartCoroutine(PlayAudio(player, clip, start, clip.length));

    /// <summary>
    /// Play <paramref name="clip"/> from <paramref name="start"/> to <paramref name="end"/>.
    /// </summary>
    /// <param name="player">Audio output source</param>
    /// <param name="clip">Audio file</param>
    /// <param name="start">Start of the audio</param>
    /// <param name="end">End of the audio</param>
    /// <returns></returns>
    public Coroutine Play(AudioSource player, AudioClip clip, float start, float end)
        => StartCoroutine(PlayAudio(player, clip, start, end));


    /// <summary>
    /// Play <paramref name="clip"/> from <paramref name="start"/> to <paramref name="end"/> 
    /// after <paramref name="delay"/> seconds has occured.
    /// </summary>
    /// <param name="player">Audio output source</param>
    /// <param name="clip">Audio file</param>
    /// <param name="start">Start of the audio</param>
    /// <param name="end">End of the audio</param>
    /// <param name="delay">Time to wait before playing</param>
    /// <returns></returns>
    public Coroutine Play(AudioSource player, AudioClip clip, float start, float end, float delay)
        => StartCoroutine(PlayAudio(player, clip, start, end, delay));

    // --- Core ---
    /// <summary>
    /// Play <paramref name="clip"/> from <paramref name="start"/> to <paramref name="end"/>.
    /// </summary>
    /// <param name="player">Audio output source</param>
    /// <param name="clip">Audio file</param>
    /// <param name="start">Start of the audio</param>
    /// <param name="end">End of the audio</param>
    /// <returns></returns>
    protected virtual IEnumerator PlayAudio(AudioSource player, AudioClip clip, float start, float end)
    {
        start = Mathf.Clamp(start, 0, clip.length);
        end = Mathf.Clamp(end, start, clip.length);

        OnPlayStart(player, clip, start, end);

        player.clip = clip;
        player.time = start;
        player.Play();
        yield return new WaitForSeconds(end - start);
        player.Stop();

        OnPlayEnd(player, clip, start, end);
    }

    /// <summary>
    /// Play <paramref name="clip"/> from <paramref name="start"/> to <paramref name="end"/> 
    /// after <paramref name="delayStart"/> seconds has occured.
    /// </summary>
    /// <param name="player">Audio output source</param>
    /// <param name="clip">Audio file</param>
    /// <param name="start">Start of the audio</param>
    /// <param name="end">End of the audio</param>
    /// <param name="delayStart">Time to wait before playing</param>
    /// <returns></returns>
    private IEnumerator PlayAudio(AudioSource player, AudioClip clip, float start, float end, float delayStart)
    {
        yield return new WaitForSeconds(Mathf.Abs(delayStart));
        yield return PlayAudio(player, clip, start, end);
    }

    // --- Hooks ---
    protected virtual void OnPlayStart(AudioSource source, AudioClip clip, float start, float end) { }
    protected virtual void OnPlayEnd(AudioSource source, AudioClip clip, float start, float end) { }
}
