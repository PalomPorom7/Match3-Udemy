using System.Collections.Generic;
using UnityEngine;

public enum Orientation
{
    none,
    horizontal,
    vertical,
    both
}
public enum MatchType
{
    invalid,
    match3,
    match4,
    match5,
    cross
}
/*
 * This is a collection of Matchables that have been matched
 */
public class Match
{
    // the number of matchables that are considered part of this match but aren't added to the list
    private int unlisted = 0;

    //  is this match horizontal or vertical?
    public Orientation orientation = Orientation.none;

    // the internal list of matched matchables
    private List<Matchable> matchables;

    private Matchable toBeUpgraded = null;

    // getters for the list and the list count
    public List<Matchable> Matchables
    {
        get
        {
            return matchables;
        }
    }
    //  getter for number of matchables part of this match
    public int Count
    {
        get
        {
            return matchables.Count + unlisted;
        }
    }
    //  check if a matchable is already in this match
    public bool Contains(Matchable toCompare)
    {
        return matchables.Contains(toCompare);
    }

    // constructor, initializes the list
    public Match()
    {
        matchables = new List<Matchable>(5);
    }
    // overload, also adds a matchable
    public Match(Matchable original) : this()
    {
        AddMatchable(original);
        toBeUpgraded = original;
    }
    //  get the type of the match
    public MatchType Type
    {
        get
        {
            if(orientation == Orientation.both)
                return MatchType.cross;

            else if(matchables.Count > 4)
                return MatchType.match5;

            else if(matchables.Count == 4)
                return MatchType.match4;

            else if(matchables.Count == 3)
                return MatchType.match3;

            else
                return MatchType.invalid;
        }
    }
    //  get the matchable to be upgraded
    public Matchable ToBeUpgraded
    {
        get
        {
            if(toBeUpgraded != null)
                return toBeUpgraded;

            return matchables[Random.Range(0, matchables.Count)];
        }
    }
    // add a matchable to the list
    public void AddMatchable(Matchable toAdd)
    {
        matchables.Add(toAdd);
    }
    // add a matchable to the count without adding it to the list
    public void AddUnlisted()
    {
        ++unlisted;
    }
    // remove a matchable from the list
    public void RemoveMatchable(Matchable toBeRemoved)
    {
        matchables.Remove(toBeRemoved);
    }
    // merge another match into this one
    public void Merge(Match toMerge)
    {
        matchables.AddRange(toMerge.Matchables);

        // update the match orientation

        if
        (
                orientation == Orientation.both
            ||  toMerge.orientation == Orientation.both
            ||  (orientation == Orientation.horizontal && toMerge.orientation == Orientation.vertical)
            ||  (orientation == Orientation.vertical && toMerge.orientation == Orientation.horizontal)
        )
            orientation = Orientation.both;

        else if(toMerge.orientation == Orientation.horizontal)
            orientation = Orientation.horizontal;

        else if(toMerge.orientation == Orientation.vertical)
            orientation = Orientation.vertical;
    }
    // convert the match into a string so we can see it
    public override string ToString()
    {
        string s = "Match of type " + matchables[0].Type + " : ";

        foreach(Matchable m in matchables)
            s += "(" + m.position.x + ", " + m.position.y + ") ";

        return s;
    }
}
