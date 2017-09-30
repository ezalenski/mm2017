using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//---------- CHANGE THIS NAME HERE -------
public class TEAM_RED_SCRIPT : MonoBehaviour
{
    //private Vector3 position = new Vector3(20.0f, 0.0f, 20.0f);

    /// <summary>
    /// DO NOT MODIFY THIS! 
    /// vvvvvvvvv
    /// </summary>
    [SerializeField]
    public CharacterScript character1;
    [SerializeField]
    public CharacterScript character2;
    [SerializeField]
    public CharacterScript character3;
    /// <summary>
    /// ^^^^^^^^
    /// </summary>
    /// 


    // USEFUL VARIABLES
    private ObjectiveScript middleObjective;
    private ObjectiveScript leftObjective;
    private ObjectiveScript rightObjective;
    private float timer = 0;

    private team ourTeamColor;
    //---------- CHANGE THIS NAME HERE -------
    public static TEAM_RED_SCRIPT AddYourselfTo(GameObject host)
    {
        //---------- CHANGE THIS NAME HERE -------
        return host.AddComponent<TEAM_RED_SCRIPT>();
    }

    void Start()
    {
        // Set up code. This populates your characters with their controlling scripts
        character1 = transform.Find("Character1").gameObject.GetComponent<CharacterScript>();
        character2 = transform.Find("Character2").gameObject.GetComponent<CharacterScript>();
        character3 = transform.Find("Character3").gameObject.GetComponent<CharacterScript>();

        // populate the objectives
        middleObjective = GameObject.Find("MiddleObjective").GetComponent<ObjectiveScript>();
        leftObjective = GameObject.Find("LeftObjective").GetComponent<ObjectiveScript>();
        rightObjective = GameObject.Find("RightObjective").GetComponent<ObjectiveScript>();

        // save our team, changes every time
        ourTeamColor = character1.getTeam();
        //Makes gametimer call every second
        InvokeRepeating("gameTimer", 0.0f, 1.0f);

    }

    void Update()
    {
        //Set caracter loadouts, can only happen when the characters are at base.
        if (character1.getZone() == zone.BlueBase || character1.getZone() == zone.RedBase)
            character1.setLoadout(loadout.LONG);
        if (character2.getZone() == zone.BlueBase || character2.getZone() == zone.RedBase)
            character2.setLoadout(loadout.LONG);
        if (character2.getZone() == zone.BlueBase || character2.getZone() == zone.RedBase)
            character3.setLoadout(loadout.LONG);

        // in the first couple of seconds we just scan around
        if (timer < 10)
        {
            character1.FaceClosestWaypoint();
            character2.FaceClosestWaypoint();
            character3.FaceClosestWaypoint();
            character1.MoveChar(new Vector3(-8.8f, 1.5f, 13.5f));
        }
        // place sniper in position, run to cover if attacked
        if (character1.attackedFromLocations.Capacity == 0)
        {
            character1.MoveChar(new Vector3(-8.8f, 1.5f, 13.5f));
            character1.SetFacing(middleObjective.transform.position);
        }
        else
        {
            character1.MoveChar(character1.FindClosestCover(character1.attackedFromLocations[0]));
        }
        // send other two to capture
        if (middleObjective.getControllingTeam() != character1.getTeam())
        {
            character2.MoveChar(middleObjective.transform.position);
            character2.SetFacing(middleObjective.transform.position);
            character3.MoveChar(middleObjective.transform.position);
            character3.SetFacing(middleObjective.transform.position);
        }
        else
        {
            // Then left
            if (leftObjective.getControllingTeam() != character1.getTeam())
            {
                character2.MoveChar(leftObjective.transform.position);
                character2.SetFacing(leftObjective.transform.position);
                character3.MoveChar(leftObjective.transform.position);
                character3.SetFacing(leftObjective.transform.position);
            }
            // Then RIght
            if (rightObjective.getControllingTeam() != character1.getTeam())
            {
                character2.MoveChar(rightObjective.transform.position);
                character2.SetFacing(rightObjective.transform.position);
                character3.MoveChar(rightObjective.transform.position);
                character3.SetFacing(rightObjective.transform.position);
            }
        }
    }

    // a simple function to track game time
    public void gameTimer()
    {
        timer += 1;
    }

    public void startingStrategy() {
        List<CharacterScript> characters = new List<CharacterScript>();

        characters.Add( character2 );
        characters.Add( character1 );
        characters.Add( character3 );

        if ( characters[0].getZone() == zone.BlueBase || character1.getZone() == zone.RedBase )
            characters[0].setLoadout( loadout.MEDIUM );
        if ( characters[1].getZone() == zone.BlueBase || character2.getZone() == zone.RedBase )
            characters[1].setLoadout( loadout.SHORT );
        if ( characters[2].getZone() == zone.BlueBase || character2.getZone() == zone.RedBase )
            characters[2].setLoadout( loadout.LONG );

        pincerStrategy( characters );
    }

    // 1-0-2
    public void pincerStrategy( List<CharacterScript> characters ) {
        if ( leftObjective.getControllingTeam() != characters[0].getTeam() &&
        rightObjective.getControllingTeam() != characters[1].getTeam() ) {
            characters[0].MoveChar( leftObjective.transform.position );

            characters[1].MoveChar( rightObjective.transform.position );
            characters[2].MoveChar( rightObjective.transform.position );
        } else {
            characters[0].MoveChar( middleObjective.transform.position );
            characters[1].MoveChar( middleObjective.transform.position );
            characters[2].MoveChar( middleObjective.transform.position );
        }
    }

    public void spread( List<CharacterScript> characters ) {
        if ( characters[0].getZone() == zone.BlueBase || character1.getZone() == zone.RedBase )
            characters[0].setLoadout( loadout.MEDIUM );
        if ( characters[1].getZone() == zone.BlueBase || character2.getZone() == zone.RedBase )
            characters[1].setLoadout( loadout.MEDIUM );
        if ( characters[2].getZone() == zone.BlueBase || character2.getZone() == zone.RedBase )
            characters[2].setLoadout( loadout.MEDIUM );

        characters[0].MoveChar( leftObjective.transform.position );
        characters[1].MoveChar( middleObjective.transform.position );
        characters[2].MoveChar( rightObjective.transform.position );
    }

    // want to take:
    /* - teammems within close proximity (dist depend on type?)
            - if solo, scan larger (don't watch back until at point))
            - if 2 total, back to back/windshield wiper
            - if all, one point to nearest point, other two windshield */
    public Vector3 scanWide(CharacterScript){
        
    }

    public Vector3 scanNearestPoint(CharacterScript){

    }

    public Vector3 eyesOnTarget(CharacterScript){

    }

    public Vector3 buddySystemScan(CharacterScript, buddyAorB){
        
    }
}

