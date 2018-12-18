using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGenScript : MonoBehaviour {

    // Probably doesn't need to be a MonoBehavior?

    // What do we need to do to generate a room?
    //  - Create floorplan
    //      - Bool some random shapes
    //      - Carpet the floor!
    //  - Add nonfunctional props
    //      - Windows, wall decorations,
    //  - Add functional objects
    //      - Vacuum starting point
    //      - Things we can interact with (table? lights?)
    //  - Prettify things
    //      - Initial carpet state
    //      - Random textures for wallpaper, furniture, etc.
    //
    //
    //  Notes
    //      - Random seed! Rooms should be sharable O:

    public float minPrimaryRoomSize;
    public float maxPrimaryRoomSize;

    public float minSecondaryRoomSize;
    public float maxSecondaryRoomSize;

    int maxNumSecondaryAreas;


    void Generate (int seed)
    {
        Debug.Log(string.Format("Seed: {0}", seed));
        Random.seed = seed;
        // Using the provided seed, generate a room!
        LayoutFloor();
    }
    
    void LayoutFloor ()
    {
        // Primary floor area
        Rect mainArea = Rect.MinMaxRect(0.0f, 0.0f, Random.Range(minPrimaryRoomSize, maxPrimaryRoomSize), Random.Range(minPrimaryRoomSize, maxPrimaryRoomSize));

        // Add some number of secondary areas
        List<Rect> secondaryAreas = new List<Rect>();
        int numSecondaryAreas = Random.Range(0, maxNumSecondaryAreas);
        for (int i = 0; i > numSecondaryAreas; i++)
        {
            Rect secondaryArea = CreateSecondaryArea(mainArea, true);
            secondaryAreas.Add(secondaryArea);
        }

    }

    void LayoutProps ()
    {

    }

    void LayoutFunctionalObjects ()
    {
        // Layout important points -- ex. starting point
    }




    Rect CreateSecondaryArea(Rect parentArea, bool clampToParent)
    {
        /*
        Given a parent rect, creates a secondary rect adjacent to the parent.
        */

        Rect secondaryArea = Rect.MinMaxRect(0.0f, 0.0f, Random.Range(minSecondaryRoomSize, maxSecondaryRoomSize), Random.Range(minSecondaryRoomSize, maxSecondaryRoomSize));
        // Pick a side to connect to and how to connect the secondary area to it
        bool minJustified = (Random.value > 0.5f);          // Whether the rect will be justified to the min (lower, left) bound, or to the max (upper, right) bound
        int side = Random.Range(0, 4);
        // 0 and 1 are left/right; 2 and 3 are bottom/top
        if (side < 2)           // adjacent to left/right
        {
            // Clamp height
            if (clampToParent)
            {
                secondaryArea.height = Mathf.Min(secondaryArea.height, parentArea.height);
            }

            float centerX = side == 0 ? parentArea.xMin - (secondaryArea.width / 2) : parentArea.xMax + (secondaryArea.width / 2);
            float centerY = minJustified ? parentArea.yMin + (secondaryArea.height / 2) : parentArea.yMax - (secondaryArea.height / 2);
            secondaryArea.center = new Vector2(centerX, centerY);
        }
        else                    // adjacent to bottom/top
        {
            // Clamp width
            if (clampToParent)
            {
                secondaryArea.width = Mathf.Min(secondaryArea.width, parentArea.width);
            }

            float centerX = minJustified ? parentArea.xMin + (secondaryArea.width / 2) : parentArea.xMax - (secondaryArea.width / 2);
            float centerY = side == 2 ? parentArea.yMin - (secondaryArea.height / 2) : parentArea.yMax + (secondaryArea.height / 2);
            secondaryArea.center = new Vector2(centerX, centerY);
        }

        return secondaryArea;
    }






	// Use this for initialization
	void Start ()
    {
        int test = Random.Range(0, 10);
        Generate(test);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
