using UnityEngine;

/*
 * This is a pool of matchables which will be instantiated during load time.
 * Remember to always activate each game object before requesting a new one.
 * This class is also a Singleton which can be accessed through Instance.
 * 
 * This class also handles the types, sprites, and colours of the matchables.
 * The type can be randomized or incremented.
 */
public class MatchablePool : ObjectPool<Matchable>
{
    [SerializeField] private int howManyTypes;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Color[] colors;

    [SerializeField] private Sprite match4Powerup;
    [SerializeField] private Sprite match5Powerup;
    [SerializeField] private Sprite crossPowerup;

    //  Get a matchable from the pool and randomize its type
    public Matchable GetRandomMatchable()
    {
        Matchable randomMatchable = GetPooledObject();

        RandomizeType(randomMatchable);

        return randomMatchable;
    }
    //  Randomize the type of a matchable
    public void RandomizeType(Matchable toRandomize)
    {
        int random = Random.Range(0, howManyTypes);

        toRandomize.SetType(random, sprites[random], colors[random]);
    }
    //  Increment the type of a matchable and return its new type
    public int NextType(Matchable matchable)
    {
        int nextType = (matchable.Type + 1) % howManyTypes;

        matchable.SetType(nextType, sprites[nextType], colors[nextType]);

        return nextType;
    }
    //  upgrade a matchable when the player makes a match of more than 3
    public Matchable UpgradeMatchable(Matchable toBeUpgraded, MatchType type)
    {
        if(type == MatchType.cross)
            return toBeUpgraded.Upgrade(MatchType.cross, crossPowerup);

        if(type == MatchType.match4)
            return toBeUpgraded.Upgrade(MatchType.match4, match4Powerup);

        if(type == MatchType.match5)
            return toBeUpgraded.Upgrade(MatchType.match5, match5Powerup);

        Debug.LogWarning("Tried to upgrade a matchable with an invalid match type.");
        return toBeUpgraded;
    }
    //  Manually set the type of a matchable, used for testing obscure cases
    public void ChangeType(Matchable toChange, int type)
    {
        toChange.SetType(type, sprites[type], colors[type]);
    }
}
