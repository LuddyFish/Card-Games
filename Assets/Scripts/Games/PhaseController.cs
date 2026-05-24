using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This script controls the phases of the game using <see cref="Phase"/>
/// </summary>
public abstract class PhaseController : MonoBehaviour
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    ///     <term>0</term>
    ///     <description>Reset the game</description>
    /// </item>
    /// <item>
    ///     <term>1</term>
    ///     <description>Deal a set of cards</description>
    /// </item>
    /// <item>
    ///     <term>2</term>
    ///     <description>Go to next Player's turn</description>
    /// </item>
    /// <item>
    ///     <term>3</term>
    ///     <description>Determine the winner</description>
    /// </item>
    /// <item>
    ///     <term>4</term>
    ///     <description>Clear Hand</description>
    /// </item>
    /// </list>
    /// </summary>
    public enum Phase
    {
        Reset,
        Deal,
        PlayerTurn,
        RoundEnd,
        Clear
    }

    public Phase CurrentPhase { get; set; }

    public Action OnPhaseComplete;

    #region Phase Logic
    /// <summary>
    /// Drives a single phase of the game. Reset, Deal and Clear use the existing
    /// <see cref="Reshuffle"/>, <see cref="Deal"/> and <see cref="ClearHands"/> methods;
    /// PlayerTurn and RoundEnd dispatch to the abstract hooks below.
    /// </summary>
    /// <param name="phase">Refer to <see cref="Phase"/> for phase numbers</param>
    protected virtual void StartPhase(Phase phase)
    {
        CurrentPhase = phase;
        OnPhaseComplete = null;

        switch (phase)
        {
            case Phase.Reset:
                OnPhaseComplete += AdvancePhase;
                ResetGame();
                OnPhaseComplete?.Invoke();
                break;
            case Phase.Deal:
                OnPhaseComplete += AdvancePhase;
                DealPhase();
                OnPhaseComplete?.Invoke();
                break;
            case Phase.PlayerTurn:
                // Game-controlled completion: subclass invokes OnPhaseComplete when done
                StartPlayerTurn();
                break;
            case Phase.RoundEnd:
                OnPhaseComplete += AdvancePhase;
                EndRound();
                OnPhaseComplete?.Invoke();
                break;
            case Phase.Clear:
                OnPhaseComplete += AdvancePhase;
                ClearHands();
                OnPhaseComplete?.Invoke();
                break;
        }
    }

    protected virtual Phase GetNextPhase(Phase current)
    {
        return current switch
        {
            Phase.Reset => Phase.Deal,
            Phase.Deal => Phase.PlayerTurn,
            Phase.PlayerTurn => Phase.RoundEnd,
            Phase.RoundEnd => Phase.Clear,
            _ => Phase.Reset
        };
    }

    protected virtual float GetPhaseDelayTime(Phase phase)
    {
        return phase switch
        {
            Phase.Reset => 0.75f,
            Phase.Deal => 0.5f,
            Phase.PlayerTurn => 0.2f,
            Phase.RoundEnd => 0.1f,
            Phase.Clear => 3f,
            _ => 0.75f
        };
    }

    /// <summary>
    /// Move to the phase returned by <see cref="GetNextPhase"/>.
    /// </summary>
    protected void AdvancePhase()
    {
        var next = GetNextPhase(CurrentPhase);
        DelayStartPhase(next, GetPhaseDelayTime(next));
    }

    /// <summary>
    /// Enacts a specific stage of the game after <paramref name="t"/> seconds
    /// </summary>
    /// <param name="phase">Refer to <see cref="Phase"/> for phase numbers</param>
    /// <param name="t">Time to wait</param>
    public Coroutine DelayStartPhase(Phase phase, float t) => StartCoroutine(DelayPhase(phase, t));

    private IEnumerator DelayPhase(Phase phase, float t)
    {
        yield return new WaitForSeconds(t);
        StartPhase(phase);
    }

    protected abstract void ResetGame();

    protected abstract void DealPhase();

    protected abstract void StartPlayerTurn();

    protected abstract void EndRound();

    protected abstract void ClearHands();
    #endregion
}
