using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIStateManager : MonoBehaviour
{
    // Instance
    public static AIStateManager aiManager;

    // Start is called before the first frame update
    void Start()
    {
        // Attach Instance
        aiManager = this;

    }
    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Call this function on init of the scene
    /// It'll check for all the NPC's and assign their state machine
    /// </summary>
    private void InitAIStateMachines()
    {
        // Currently, i'll check using tags(?)
        // but i think it needs to be changed later on
        // Probably will have a parent object that stores all the AIs for that level
        // And it will look for that parent object

        GameObject[] activeNPCs = GameObject.FindGameObjectsWithTag("AI");

        // Loop through the NPCs in the list
        foreach(GameObject npc in activeNPCs)
        {
            AIController aiController = npc.GetComponent<AIController>();

            if (aiController.sm == null)
            {
                aiController.sm = new StateMachine();

                for (int index = 0; index < aiController.stateNames.Length; ++index)
                {
                    if (aiController.stateNames[index] == "State Name would be here")
                    {
                        // Add new state
                        //aiController.sm.AddState()
                    }
                }
            }
        }
    }

    /*
     * The Actual Fetching of GameObject from the Enemy Template
     */
    //private void SpawnNewEnemyFromTemplate(SpawningEnemyTemplate enemySpawn, SpellDirectory.BASE_ELEMEMTS waveResist)
    //{
    //    // if we are using endlessLimit, then don't spawn if more
    //    // than limit
    //    if (endlessMode)
    //        if (useEndlessLimit)
    //            if (endlessSpawnedMobs > endlessMobLimit)
    //                return;

    //    // Check if we are using a unique Spawning Position or
    //    // using the random spawn positions
    //    Vector3 newPos;
    //    if (enemySpawn.uniqueSpawnPosition)
    //    {
    //        newPos = enemySpawn.spawnPosition;
    //    } 
    //    else
    //    {
    //        //clear the array
    //        allowedSpawnZone = new List<GameObject>();
    //        for (int i = 0; i < enemyZone.Length; ++i)
    //        {
    //            if (enemyZone[i].GetComponent<Renderer>().isVisible)        //check if area is visble by the camera
    //            {
    //                //Debug.Log("Zone " + i + " IN SIGHT");
    //                //allowedSpawnZone.Add(enemyZone[i]);
    //            }
    //            else    // not visible
    //            {
    //                //Debug.Log("Zone " + i + " OUT SIGHT");
    //                //Add to the list of spawn boxes where can enemies can spawn in
    //                allowedSpawnZone.Add(enemyZone[i]);
    //            }
    //        }

    //        if (allowedSpawnZone.Count == 0)
    //        {
    //            //No spawn box avalible (can be avoid with proper spawn box placement)
    //            //reset timer so it has doesnt check constantly
    //            enemySpawn.mobSpawnTimer = 0.0f;

    //            //exit out so no enemy spawns
    //            return;
    //        }
    //        //randomise a zone to spawn in
    //        int spawnZone = Random.Range(0, allowedSpawnZone.Count);

    //        // Set spawning Collider box
    //        Collider m_collider = allowedSpawnZone[spawnZone].GetComponent<Collider>();

    //        //get random pos
    //        newPos = new Vector3(Random.Range(m_collider.bounds.min.x, m_collider.bounds.max.x),
    //                                        0.0f,
    //                                        Random.Range(m_collider.bounds.min.z, m_collider.bounds.max.z));
    //    }


    //    // Get the new Enemy since we can spawn
    //    GameObject newEnemyGameObject = null;
    //    // Get From Pooler to prevent instantiating at RunTime
    //    objectPooler.GetComponent<ObjectPooler>().GetGameObject_Referrence(enemySpawn.mobPrefabNameInFetchGO, ref newEnemyGameObject);
    //    // if false, error has occured..
    //    if (newEnemyGameObject == null)
    //        return;
    //    // Add into list to keep track of what enemies are still alive
    //    enemySpawn.aliveMobs.Add(newEnemyGameObject);
    //    // Attach State Machine to newly created AI
    //    if (enemySpawn.stateNames.Length > 0)
    //    {
    //        // Check if their state machine have already been created..
    //        if (newEnemyGameObject.GetComponent<MobAI>().sm == null)
    //        {
    //            // Create the State Machine
    //            newEnemyGameObject.GetComponent<MobAI>().sm = new StateMachine();

    //            // Detect and Attach States
    //            for (int index = 0; index < enemySpawn.stateNames.Length; ++index)
    //            {
    //                if (enemySpawn.stateNames[index] == "EyeMobChase")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new EyeMob_ChaseState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "EyeMobAttack")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new EyeMob_AttackState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "EyeMobDodge")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new EyeMob_DodgeState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "EyeMobDead")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new EyeMob_DeadState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "PlantMobChase")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new PlantMob_ChaseState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "PlantMobAttack")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new PlantMob_AttackState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "PlantMobDodge")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new PlantMob_DodgeState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "PlantMobDead")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new PlantMob_DeadState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "WaterBossChase")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new WaterBoss_ChaseState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "WaterBossAttack")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new WaterBoss_AttackState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "WaterBossDodge")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new WaterBoss_DodgeState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "WaterBossDead")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new WaterBoss_DeadState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "FireBossChase")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new FireBoss_ChaseState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "FireBossAttack")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new FireBoss_AttackState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "FireBossDodge")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new FireBoss_DodgeState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "FireBossDead")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new FireBoss_DeadState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "AirBossChase")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new AirBoss_ChaseState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "AirBossAttack")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new AirBoss_AttackState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "AirBossDodge")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new AirBoss_DodgeState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "AirBossDead")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new AirBoss_DeadState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "EarthBossChase")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new EarthBoss_ChaseState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "EarthBossAttack")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new EarthBoss_AttackState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "EarthBossDodge")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new EarthBoss_DodgeState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else if (enemySpawn.stateNames[index] == "EarthBossDead")
    //                {
    //                    newEnemyGameObject.GetComponent<MobAI>().sm.AddState(new EarthBoss_DeadState(enemySpawn.stateNames[index], newEnemyGameObject));
    //                }
    //                else
    //                    Debug.Log("AIStateManager: CANNOT FIND STATE!!!");
    //            }
    //            // Set next State to first State
    //            newEnemyGameObject.GetComponent<MobAI>().sm.SetNextState(enemySpawn.stateNames[0]);
    //        }
    //        // State Machine already created, so just set next state
    //        else
    //        {
    //            // Set next State to first State
    //            newEnemyGameObject.GetComponent<MobAI>().sm.SetNextState(enemySpawn.stateNames[0]);
    //        }
    //    }

    //    // Check if we need to elvelate the position
    //    // to the object's Y pos
    //    if (!enemySpawn.uniqueSpawnPosition)
    //        newPos.y = newEnemyGameObject.transform.position.y;
    //    // Set Position
    //    newEnemyGameObject.transform.position = newPos;
    //    // Set health
    //    newEnemyGameObject.GetComponent<MobAI>().ResetHealth();
    //    // Set resistance
    //    newEnemyGameObject.GetComponent<MobAI>().ElementalResistance = waveResist;

    //}
    //public SpawningEnemyTemplate GetEnemyTemplate(string nameOfPrefab)
    //{
    //    foreach(SpawningEnemyTemplate enemyTemplate in listOfEnemiesToSpawn)
    //    {
    //        if (enemyTemplate.mobPrefabNameInFetchGO == nameOfPrefab)
    //            return enemyTemplate;
    //    }
    //    return null;
    //}
    /*
    *  Returns the SpawnEnemyTemplate Index if any using the name Of the prefab
    *  Provided in the parameters 
    */
    //public int GetEnemyTemplateIndex(string nameOfPrefab)
    //{
    //    for(int index = 0; index < listOfEnemiesToSpawn.Count; ++index)
    //    {
    //        if (listOfEnemiesToSpawn[index].mobPrefabNameInFetchGO == nameOfPrefab)
    //            return index;
    //    }
    //    return -1;
    //}
}
