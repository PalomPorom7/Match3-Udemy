using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class will manage the player's score and resolving matches
 * 
 * This is a Singleton, so only 1 can exist in the scene and it can be accessed through Instance
 */
public class ScoreManager : Singleton<ScoreManager>
{
    private MatchablePool pool;
    private MatchableGrid grid;
    private AudioMixer audioMixer;

    [SerializeField]
    private Transform collectionPoint;

    //  UI text element for displaying the score and combo multiplier
    [SerializeField]
    private Text    scoreText,
                    comboText;

    //  UI Slider element for displaying the time remaining in the combo
    [SerializeField]
    private Image  comboSlider;

    // actual score, and a combo multiplier
    private int score,
                comboMultiplier;

    //  getter for score
    public int Score
    {
        get
        {
            return score;
        }
    }
    // how much time has passed since the player last scored?
    private float timeSinceLastScore;
    
    //  how much time should we allow before resetting the combo multiplier?
    [SerializeField]
    private float   maxComboTime,
                    currentComboTime;

    //  is the combo timer currently running?
    private bool timerIsActive;

    //  get references to other game objects in Start
    private void Start()
    {
        pool = (MatchablePool) MatchablePool.Instance;
        grid = (MatchableGrid) MatchableGrid.Instance;
        audioMixer = AudioMixer.Instance;

        comboText.enabled = false;
        comboSlider.gameObject.SetActive(false);
    }

    //  when the player hits retry, reset the score and combo
    public void Reset()
    {
        score = 0;
        scoreText.text = score.ToString();
        timeSinceLastScore = maxComboTime;
    }

    // add an amount to the score and update the UI text
    public void AddScore(int amount)
    {
        score += amount * IncreaseCombo();
        scoreText.text = score.ToString();

        timeSinceLastScore = 0;

        if(!timerIsActive)
            StartCoroutine(ComboTimer());

        //  play score sound
        audioMixer.PlaySound(SoundEffects.score);
    }
    //  Combo timer coroutine, counts up to max combo time before resetting the combo multiplier
    private IEnumerator ComboTimer()
    {
        timerIsActive = true;
        //  enable UI elements
        comboText.enabled = true;
        comboSlider.gameObject.SetActive(true);

        do
        {
            //  add time elapsed and update the value of the slider
            timeSinceLastScore += Time.deltaTime;
            comboSlider.fillAmount = 1 - timeSinceLastScore / currentComboTime;
            yield return null;
        }
        while(timeSinceLastScore < currentComboTime);

        // reset the combo multiplier and disable UI elements
        comboMultiplier = 0;
        comboText.enabled = false;
        comboSlider.gameObject.SetActive(false);

        timerIsActive = false;
    }
    private int IncreaseCombo()
    {
        comboText.text = "Combo x" + ++comboMultiplier;

        //  make the timer shrink logarithmically
        currentComboTime = maxComboTime - Mathf.Log(comboMultiplier) / 2;

        return comboMultiplier;
    }
    // coroutine for resolving a match
    public IEnumerator ResolveMatch(Match toResolve, MatchType powerupUsed = MatchType.invalid)
    {
        Matchable powerupFormed = null;
        Matchable matchable;

        Transform target = collectionPoint;

        //  if no powerup was used to trigger this match and a larger match is made, create a powerup
        if(powerupUsed == MatchType.invalid && toResolve.Count > 3)
        {
            powerupFormed = pool.UpgradeMatchable(toResolve.ToBeUpgraded, toResolve.Type);
            toResolve.RemoveMatchable(powerupFormed);
            target = powerupFormed.transform;
            powerupFormed.SortingOrder = 3;

            // play upgrade sound
            audioMixer.PlaySound(SoundEffects.upgrade);
        }
        else
        {
            // play resolve sound
            audioMixer.PlaySound(SoundEffects.resolve);
        }
        //  iterate through every matchable in the match
        for(int i = 0; i != toResolve.Count; ++i)
        {
            matchable = toResolve.Matchables[i];
 
            //  only allow gems used as powerups to resolve gems
            if(powerupUsed != MatchType.match5 && matchable.IsGem)
                continue;

            // remove the matchable from the grid
            grid.RemoveItemAt(matchable.position);

            // move them off to the side of the screen simultaneously
            // and wait for the last one to finish
            if(i == toResolve.Count - 1)
                yield return StartCoroutine(matchable.Resolve(target));
            else
                StartCoroutine(matchable.Resolve(target));
        }
        // update the player's score
        AddScore(toResolve.Count * toResolve.Count);

        //  if there was a powerup, reset the sorting order
        if(powerupFormed != null)
            powerupFormed.SortingOrder = 0;
    }
}
