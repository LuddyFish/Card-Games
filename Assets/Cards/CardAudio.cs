using UnityEngine;

public class CardAudio : AudioPlayer
{
    public static CardAudio Instance;

    /* Audio list tips
     * 0. Card flip
     * 1. Card deal
     * 2. Card flip
     * 3. Multiple cards slide
     * 4. Multiple cards slide / shuffle
     * 5. Card slide / Card unselect
     * 6. Card select
     */

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    protected override void Start()
    {
        base.Start();
    }

    public void RegisterCardSRC(AudioSource src)
    {
        sources.Add(src);
    }

    #region Play Calls
    private void PlayCardSound(GameObject card, int srcNum)
    {
        Play(card.GetComponent<AudioSource>(), audios[srcNum]);
    }

    private void PlayCardSound(AudioSource src, int srcNum)
    {
        Play(src, audios[srcNum]);
    }

    #region Named Play Calls
    public void PlayCardDeal(GameObject card)
    {
        PlayCardSound(card, 1);
    }

    public void PlayCardDeal(AudioSource src)
    {
        PlayCardSound(src, 1);
    }

    public void PlayDeckShuffle(GameObject card)
    {
        PlayCardSound(card, 3);
    }

    public void PlayDeckShuffle(AudioSource src)
    {
        PlayCardSound(src, 3);
    }

    public void PlayCardShuffle(GameObject card)
    {
        PlayCardSound(card, 4);
    }

    public void PlayCardShuffle(AudioSource src)
    {
        PlayCardSound(src, 4);
    }
    #endregion
    #endregion

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
