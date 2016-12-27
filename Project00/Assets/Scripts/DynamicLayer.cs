using UnityEngine;
using System.Collections;

//Only for objects on the same layer
public class DynamicLayer : MonoBehaviour {

    private Animator anim;
    private Animator otherAnim;

    private SpriteRenderer thisSprite;
    private SpriteRenderer otherSprite;
    private int origSortingOrder;
    private int otherOrigSortingOrder;

    private bool changedLayer;

    public GameObject thisShadow;
    private GameObject otherShadow;

    // Use this for initialization
    void Start () {
        thisSprite = GetComponent<SpriteRenderer>();
        origSortingOrder = thisSprite.sortingOrder;

        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    void OnTriggerStay2D(Collider2D other)
    {
        //Only change the layer ONCE per collision 
        //(OnTriggerEnter2D won't work since we need to keep checking which object has a higher y-value
        if (!changedLayer && (other.gameObject.tag != "WalkZone"))
        {
            changedLayer = true;
            float thisY = thisShadow.transform.position.y;//Use the shadow's y-value to compare
            otherShadow = other.gameObject.transform.FindChild("shadow_0").gameObject;
            float otherY = otherShadow.gameObject.transform.position.y;
            otherSprite = other.gameObject.GetComponent<SpriteRenderer>();
            otherOrigSortingOrder = other.gameObject.GetComponent<SpriteRenderer>().sortingOrder;

            //this should be "behind" other
            if (thisY > otherY)
            {
                if (thisSprite.sortingOrder <= otherSprite.sortingOrder)//if not behind
                {
                    thisSprite.sortingOrder -= 1;
                }
            }
            //This should be "in front" of other
            else if (thisY < otherY)
            {
                if (thisSprite.sortingOrder >= otherSprite.sortingOrder)//if not in front
                {
                    thisSprite.sortingOrder += 1;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag != "WalkZone")
        {
            changedLayer = false;
            thisSprite.sortingOrder = 0;//origSortingOrder;//restore original sorting order
            otherSprite.sortingOrder = 0;//otherOrigSortingOrder;
        }
    }
}
