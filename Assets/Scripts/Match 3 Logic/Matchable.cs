using System.Collections;
using UnityEngine;

/*
 * These are the things in the grid that the player will match by swapping them around
 * 
 * They require a sprite to be drawn on the screen
 * 
 * The Matchable Type can only be set along with a sprite and colour
 */
[RequireComponent(typeof(SpriteRenderer))]
public class Matchable : Movable
{
    private MatchablePool pool;
    private MatchableGrid grid;
    private Cursor cursor;

    private int type;

    public int Type
    {
        get
        {
            return type;
        }
    }

    private MatchType powerup = MatchType.invalid;

    public bool IsGem
    {
        get
        {
            return powerup == MatchType.match5;
        }
    }

    private SpriteRenderer spriteRenderer;

    //  where is this matchable in the grid?
    public Vector2Int position;

    //  get a reference to the sprite renderer in Awake
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //  get required references during Start
    private void Start()
    {
        cursor = Cursor.Instance;
        pool = (MatchablePool) MatchablePool.Instance;
        grid = (MatchableGrid) MatchableGrid.Instance;
    }

    //  set the type, sprite, and colour all at once, performed by the pool
    public void SetType(int type, Sprite sprite, Color color)
    {
        this.type = type;
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
    }

    public IEnumerator Resolve(Transform collectionPoint)
    {
        //  if matchable is a powerup, resolve it as such
        if(powerup != MatchType.invalid)
        {
            // resolve a match4 powerup
            if(powerup == MatchType.match4)
            {
                // score everything adjacent to this
                grid.MatchAllAdjacent(this);
            }
            // resolve a cross powerup
            if(powerup == MatchType.cross)
            {
                grid.MatchRowAndColumn(this);
            }

            powerup = MatchType.invalid;
        }
        //  if this was called as the result of a powerup being upgraded, then don't move this off the grid
        if(collectionPoint == null)
            yield break;

        // draw above others in the grid
        spriteRenderer.sortingOrder = 2;

        // move off the grid to a collection point
        yield return StartCoroutine(MoveToTransform(collectionPoint));

        // reset
        spriteRenderer.sortingOrder = 1;

        // return back to the pool
        pool.ReturnObjectToPool(this);
    }

    //  change the sprite of this matchable to be a powerup while retaining colour and type
    public Matchable Upgrade(MatchType powerupType, Sprite powerupSprite)
    {
        // if this is already a powerup, resolve it before upgrading
        if(powerup != MatchType.invalid)
        {
            idle = false;
            StartCoroutine(Resolve(null));
            idle = true;
        }
        if(powerupType == MatchType.match5)
        {
            type = -1;
            spriteRenderer.color = Color.white;
        }
        powerup = powerupType;
        spriteRenderer.sprite = powerupSprite;

        return this;
    }

    //  set the sorting order of the sprite renderer so it will be drawn above or below others
    public int SortingOrder
    {
        set
        {
            spriteRenderer.sortingOrder = value;
        }
    }

    //  when the player clicks, select this as the first selected
    private void OnMouseDown()
    {
        cursor.SelectFirst(this);
    }

    //  when the player releases the click, select nothing
    private void OnMouseUp()
    {
        cursor.SelectFirst(null);
    }

    //  when the player drags the mouse, select this as the second selected
    //  (if using a mouse, this is actually called on every entry, even if they're not dragging, but cursor will filter this behaviour out)
    private void OnMouseEnter()
    {
        cursor.SelectSecond(this);
    }

    public override string ToString()
    {
        return gameObject.name;
    }
}
