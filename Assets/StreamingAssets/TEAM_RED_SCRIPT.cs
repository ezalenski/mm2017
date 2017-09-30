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
    private LoggingSystem logSystem;
    private Vector3 blueSpawn = new Vector3(55.0f, 1.325f, -30.0f);
    private Vector3 redSpawn = new Vector3(-55.0f, 1.325f, 30.0f);
    private List<Character> characters;
    //---------- CHANGE THIS NAME HERE -------
    public static TEAM_RED_SCRIPT AddYourselfTo(GameObject host)
    {
        //---------- CHANGE THIS NAME HERE -------
        return host.AddComponent<TEAM_RED_SCRIPT>();
    }
    public List<CharacterScript> players = new List<CharacterScript>();

    [System.Serializable]
    public class Character {
        public CharacterScript cs;
        public bool locked;
        public int id;
        private loadout lo;

        public Character(CharacterScript script, int i) {
            cs = script;
            id = i;
            locked = false;
        }

        public int getRange() {
            switch(lo) {
                case loadout.SHORT:
                    return 15;
                case loadout.MEDIUM:
                    return 25;
                default:
                case loadout.LONG:
                    return 35;
            }
        }

        public void setLoadout(loadout newLoadout) {
            cs.setLoadout(newLoadout);
            lo = newLoadout;
        }
    }

    [System.Serializable]
    private class LoggingSystem {
        public List<Enemy> enemies;
        //public List<Item> knownItems;
        private Vector3 enemySpawn;

        [System.Serializable]
        public class Enemy {
            public int id;
            public Vector3 location = Vector3.negativeInfinity;
            public int dmgTaken = 0;
            public bool dead = false;
            public loadout lo = loadout.LONG;
        }

        public LoggingSystem(Vector3 enemySpawnLoc) {
            enemySpawn = enemySpawnLoc;
            enemies = new List<Enemy>();
            for(int i = 0; i < 3; i++) {
                enemies.Add(new Enemy());
                enemies[i].id = i;
                enemies[i].location = enemySpawnLoc;
            }
        }

        //call with StartCoroutine(logSystem.addEnemyLocation, id, loc)
        public IEnumerator addEnemyLocation(int id, Vector3 loc) {
            enemies[id].location = loc;
            yield return new WaitForSeconds(5.0f);
            enemies[id].location = Vector3.negativeInfinity;
        }

        public void addEnemyDmg(int id, int dmg) {
            enemies[id].dmgTaken += dmg;
        }

        public void addEnemyLoadout(int id, loadout lo) {
            enemies[id].lo = lo;
        }


        //call with StartCoroutine(logSystem.enemyDead, id)
        public IEnumerator enemyDead(int id) {
            Enemy deadEnemy = enemies[id];
            deadEnemy.dead = true;
            deadEnemy.location = Vector3.negativeInfinity;
            deadEnemy.lo = loadout.LONG;
            deadEnemy.dmgTaken = 0;
            yield return new WaitForSeconds(5.0f);
            deadEnemy.dead = false;
            deadEnemy.location = enemySpawn;
        }
    }

    void Start()
    {
        // Set up code. This populates your characters with their controlling scripts
        character1 = transform.Find("Character1").gameObject.GetComponent<CharacterScript>();
        character2 = transform.Find("Character2").gameObject.GetComponent<CharacterScript>();
        character3 = transform.Find("Character3").gameObject.GetComponent<CharacterScript>();
        characters = new List<Character>();
        characters.Add(new Character(character1, 0));
        characters.Add(new Character(character2, 1));
        characters.Add(new Character(character3, 2));

        // populate the objectives
        middleObjective = GameObject.Find("MiddleObjective").GetComponent<ObjectiveScript>();
        leftObjective = GameObject.Find("LeftObjective").GetComponent<ObjectiveScript>();
        rightObjective = GameObject.Find("RightObjective").GetComponent<ObjectiveScript>();

        // save our team, changes every time
        ourTeamColor = character1.getTeam();
        
        logSystem = (ourTeamColor == team.blue) ? new LoggingSystem(redSpawn) : new LoggingSystem(blueSpawn);
        Debug.Log("Enemy " + logSystem.enemies[0].location);
        
        //Makes gametimer call every second
        InvokeRepeating("gameTimer", 0.0f, 1.0f);
    }

    void Update()
    {
        /*//Set caracter loadouts, can only happen when the characters are at base.
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
        }*/
        Dictionary<int, Vector3> found = checkVisible(characters);
        foreach (Character c in characters) {
            if(!found.ContainsKey(c.id))
                c.cs.rotateAngle(500);
            else
                c.cs.SetFacing(found[c.id]);
        }

        if ( leftObjective.getControllingTeam() != ourTeamColor ||
            rightObjective.getControllingTeam() != ourTeamColor ) {
            startingStrategy();
        } else {
            if ( middleObjective.getControllingTeam() != ourTeamColor ) {
                rushMidStrategy();
            } else if ( characters[0].cs.visibleEnemyLocations.Count > 1 ) {
                pincerSwapLong();
            } else if ( leftObjective.getControllingTeam() == ourTeamColor &&
                rightObjective.getControllingTeam() == ourTeamColor &&
                middleObjective.getControllingTeam() == ourTeamColor) {
                    spreadStrategy();
            }
        }
    }

    // a simple function to track game time
    public void gameTimer()
    {
        timer += 1;
    }

/* ------------------ strategy set ------------------ */
    public void startingStrategy() {

        if ( characters[0].cs.getZone() == zone.BlueBase || characters[0].cs.getZone() == zone.RedBase )
            characters[0].setLoadout( loadout.MEDIUM );
        if ( characters[1].cs.getZone() == zone.BlueBase || characters[1].cs.getZone() == zone.RedBase )
            characters[1].setLoadout( loadout.SHORT );
        if ( characters[2].cs.getZone() == zone.BlueBase || characters[2].cs.getZone() == zone.RedBase )
            characters[2].setLoadout( loadout.LONG );

        pincerStrategy();
    }

    public void rushMidStrategy(  ) {
            characters[0].cs.MoveChar( middleObjective.transform.position );
            characters[1].cs.MoveChar( middleObjective.transform.position );
            characters[2].cs.MoveChar( middleObjective.transform.position );
    }

    // 1-0-2
    public void pincerStrategy(  ) {
        characters[0].cs.MoveChar( rightObjective.transform.position );
        characters[1].cs.MoveChar( rightObjective.transform.position );
        characters[2].cs.MoveChar( leftObjective.transform.position );
    }

    public void pincerSwapLong(  ) {
        characters[0].cs.MoveChar( leftObjective.transform.position );
        characters[1].cs.MoveChar( rightObjective.transform.position );
        characters[2].cs.MoveChar( leftObjective.transform.position );
    }

    public void spreadStrategy(  ) {
        if ( characters[1].cs.visibleEnemyLocations.Count < 1 &&
            characters[1].cs.visibleEnemyLocations.Count < 1 ) {
            characters[0].cs.MoveChar( new Vector3( -8.8f, 1.5f, 13.5f ) );
            characters[0].cs.SetFacing( middleObjective.transform.position );
            characters[1].cs.MoveChar( new Vector3(-32f, 1.5f, -26f ) );
            characters[1].cs.SetFacing( leftObjective.transform.position );
            characters[2].cs.MoveChar( new Vector3( 40f, 1.5f, 18f ) );
            characters[2].cs.SetFacing( rightObjective.transform.position );
        }
    }

    public void longRangeStrategy(  ) {
        if ( characters[0].cs.getZone() == zone.BlueBase || character1.getZone() == zone.RedBase )
            characters[0].setLoadout( loadout.LONG );
        if ( characters[1].cs.getZone() == zone.BlueBase || character2.getZone() == zone.RedBase )
            characters[1].setLoadout( loadout.LONG );
        if ( characters[2].cs.getZone() == zone.BlueBase || character2.getZone() == zone.RedBase )
            characters[2].setLoadout( loadout.LONG );

        characters[0].cs.MoveChar( leftObjective.transform.position );
        characters[1].cs.MoveChar( middleObjective.transform.position );
        characters[2].cs.MoveChar( rightObjective.transform.position );
    }

    public void medRangeStrategy(  ) {
        if ( characters[0].cs.getZone() == zone.BlueBase || character1.getZone() == zone.RedBase )
            characters[0].setLoadout( loadout.MEDIUM );
        if ( characters[1].cs.getZone() == zone.BlueBase || character2.getZone() == zone.RedBase )
            characters[1].setLoadout( loadout.MEDIUM );
        if ( characters[2].cs.getZone() == zone.BlueBase || character2.getZone() == zone.RedBase )
            characters[2].setLoadout( loadout.MEDIUM );

        characters[0].cs.MoveChar( leftObjective.transform.position );
        characters[1].cs.MoveChar( middleObjective.transform.position );
        characters[2].cs.MoveChar( rightObjective.transform.position );
    }

    public void shortRangeStrategy(  ) {
        if ( characters[0].cs.getZone() == zone.BlueBase || character1.getZone() == zone.RedBase )
            characters[0].setLoadout( loadout.SHORT );
        if ( characters[1].cs.getZone() == zone.BlueBase || character2.getZone() == zone.RedBase )
            characters[1].setLoadout( loadout.SHORT );
        if ( characters[2].cs.getZone() == zone.BlueBase || character2.getZone() == zone.RedBase )
            characters[2].setLoadout( loadout.SHORT );

        characters[0].cs.MoveChar( leftObjective.transform.position );
        characters[1].cs.MoveChar( middleObjective.transform.position );
        characters[2].cs.MoveChar( rightObjective.transform.position );
    }

    
/* ------------------ end strategy set ------------------ */

    // want to take:
    /* - teammems within close proximity (dist depend on type?)
            - if solo, scan larger (don't watch back until at point))
            - if 2 total, back to back/windshield wiper
            - if all, one point to nearest point, other two windshield */
    public Vector3 scan360(CharacterScript dude){
        dude.rotateAngle(500);
        return Vector3.zero;
    }



    public Dictionary<int, Vector3> checkVisible(List<Character> dudes){
        Dictionary<int, Vector3> ret = new Dictionary<int, Vector3>();
        for (int i = 0; i < 3; i++) {
            Character dude = dudes[i];
            if(dude.cs.visibleEnemyLocations.Count > 0) {
                Vector3 targetLoc = dude.cs.visibleEnemyLocations[0];
                ret[dude.id] = dude.cs.visibleEnemyLocations[0];
                for (int j = 0; j < 3; j++) {
                    Character otherDude = dudes[(i+j)%3];
                    if(!ret.ContainsKey(otherDude.id) && 
                        Vector3.Distance(otherDude.cs.getPrefabObject().transform.position, targetLoc) <= otherDude.getRange())
                        ret[otherDude.id] = targetLoc;
                }
            }
            dude.cs.visibleEnemyLocations.Clear();
        }
        return ret;
    }

    public Vector3 buddySystemScan(List<CharacterScript> buds){
        buds[0].rotateAngle(160);
        buds[1].rotateAngle(-160);
        return Vector3.zero;

    }

}

/*private class Item {
    public bool ourTeam = false;
    public Vector3 location = Vector3.negativeInfinity;
    public typeOfItem type = null;
    public int timeout = 0;
}*/
