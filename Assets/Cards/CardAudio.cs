using UnityEngine;

public class CardAudio : AudioPlayer
{
    public static CardAudio Instance;

    /* Audio list tips
     * 0. Card flip
     * 1. Card deal
     * 2. Card flip
     * 3. Multiple cards slide
     * 4. Multiple cards slide
     * 5. Card slide
     */

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    new void Start()
    {
        base.Start();
    }

    public void SetCardSRCs()
    {
        foreach (var card in Cardbox.Instance.cards)
            sources.Add(card.GetComponent<AudioSource>());
    }
}
