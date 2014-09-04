    using UnityEngine;
using System.Collections;
using System;

/*General notes about animals
Cost to stay alive dependent on size.


*/

/*  Order of AI behaviour
    Automatic/passive things first. (Growing)
    Living/avoid enemies
    reproducing
    eating
*/


public class AnimalBehaviour : MonoBehaviour
{

    public enum AnimalType
    {
        herbivore,
        carnivore,
        omnivore,
    }

#region variables
    public int thisAnimalSpeciesID;
    private float deltaTime;
    public AnimalType animalType; //If it is carnivore, herbivore or omivore.

//Living
    private float age;  //Current age.
    private bool alive; //if it is alive.
    private bool isReadyToDie; //if it is ready to be killed.
    private float avgMaxAge; // The avergae maximum age for the animal.
    private bool isOld; //if it is near avgMaxAge. Chances to die increase
    private float energy; //Used to exist, replenish from food, move, etc.
//--

//AI
    private float willToLive;//Modified by age, and own scareLevel.
    private float maxWillToLive;
    private float health; //Dies when health = 0. Energy used to heal.
    private float willToReproduce; //increments through time??
//--

//Eat
    private float hunger; //How hungry it is out of 100. 100 = Full.
    private float eatEffeciency; //How much energy it takes per eatSize.
    private float eatSize; //How much energy it takes from the other object.



//Scare stuff (part of fight or flight AI??)
    private float scareLevel; //How scary it looks = size and naturalScariness modified by age.
    private float size; //How big the thing is currently.
    private float maxSize; //How big it can be.
    private float naturalScariness; //How scary it looks naturally.


// fight stuff  goes here.
//No fight stuff at the moment.


//Reproduction values use?
    private float maxDistance; //distance from parent child is spawned.
    private float minDistance;
    private float reproduceCycle  = 15.0f; //How long before it is ready to make more babies.
    private bool isReadyToPopBaby; //Weather or not it is ready to give birth.

//turning and moving
    private Vector3 position;
    private float rotation = 0.0f;
    private Vector3 turnSpeed = new Vector3(0f, 2.0f, 0); //How far it rotates. Turns right or left only.
    private float turnFreq = 0.025f; //How often it decideds to adjust it's turn.
    private float speed; //How fast it moves forwards.
    Vector3 tarPos;         //Target's position


    float secondDeltaTime;
    float creatureGrowTime;
    float reproduceTimer;
   
    bool newSecond;
    bool growCreature;
#endregion

// Update is called once per frame
    void Update()
    {
        //If dead
        if (energy < 0)
        { 
            isReadyToDie = true;
            alive = false; 
        }

        if (alive)
        {

        
            bool targetTrue = false; //Used for when a target has been set.


            deltaTime += Time.deltaTime;
            secondDeltaTime += deltaTime;
            reproduceTimer += deltaTime;
           
            if(secondDeltaTime >= 1.0f)
            {
                newSecond = true;
                secondDeltaTime = 0.0f;
            }else{
                newSecond = false;
            }

            //grow animal if not full size
            if (size < maxSize)
            {
                creatureGrowTime += deltaTime;
                rotation /= size;
                if (creatureGrowTime > 2.5f)
                {
                    growCreature= true;
                    //Grow creature
                    if (growCreature)
                    {
                        size += (maxSize / 4);
                        transform.localScale += new Vector3(size, size, size);

                        energy -= 0.75f;
                       
                    }
                    growCreature = false;
                    creatureGrowTime = 0.0f;

                }
            }
           

            #region choose target
            //Chooses a target
            if(!newSecond)
            {
            tarPos = setTarget(isReadyToPopBaby);
            
            //A vector to show that there is no target position
            //noTarget is return if there is no target.
            Vector3 noTarget = new Vector3(0, -99, 0); 
            if (tarPos != noTarget)
            {
                targetTrue = true;
            }


            //face target
            if (targetTrue)
            {
                FaceTarget();

                //Moves to target
                MoveForwards();
              
            }
            }
            #endregion move to target
          


        }

        //if dead speed up energy decrease due to decay
        else
        {
            energy -= 0.1f;
        }
       
        //cost of simply being alive
        energy -= 0.01f * size; //smaller the animal less energy lost due to being alive.

        if (isReadyToDie && energy <= 0)
        {
            Destroy(gameObject);
        }


        deltaTime = 0.0f;
    }

    void FaceTarget()
    {
        if (tarPos != position)
        {
            Quaternion faceRot = Quaternion.LookRotation((tarPos - position));
            transform.rotation = Quaternion.Slerp(transform.rotation, faceRot, rotation * deltaTime);
            
            Debug.DrawLine(tarPos, position, Color.yellow);
        }
    }

    void MoveForwards()
    {

        transform.Translate(Vector3.forward * speed /(size*2));
        position = transform.position;
    }
 
    void MoveBackwards(float pushAmount)
    {
        float push = pushAmount;
        transform.Translate(Vector3.back * speed * push);
        position = transform.position;
    }
    void MoveRight(float pushAmount)
    {
        float push = pushAmount;
        transform.Translate(Vector3.right * -speed * push);
        position = transform.position;
    }
    void MoveLeft(float pushAmount)
    {
        float push = pushAmount;
        transform.Translate(Vector3.left * -speed * push);
        position = transform.position;
    }
    Vector3 FindClosestThreat()
    {
        GameObject[] avoidList;
        GameObject closestCarnivore = null;
        float distance = Mathf.Infinity;
        avoidList = GameObject.FindGameObjectsWithTag("Carnivore");

        if (avoidList.Length > 0)
        {   
            
            //foreach (GameObject potentialtarget in targetList) {
            for (int i = 0; i < avoidList.Length; i++)
            {
                
                GameObject potentialtarget = avoidList [i];
                Vector3 diff = potentialtarget.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                
                if (curDistance < distance)
                {
                    closestCarnivore = potentialtarget;
                    distance = curDistance;
                }
            }
            Debug.Log("ClosestThreatFound");
            return  closestCarnivore.transform.position;
        }
        return Vector3.zero;


    }
    Vector3 setTarget(bool lookingForMate)
    {
        GameObject[] targetList; //List of objects
       
        GameObject closest = null; //closest obj;
        float distance = Mathf.Infinity; //distance of closest obj
        Vector3 position = transform.position;
        Vector3 diff;
        float curDistance; //distance of current obj
        if (!lookingForMate)
        {
            if (animalType == AnimalType.herbivore)
            {
             targetList = GameObject.FindGameObjectsWithTag("Plant");
                //find closest carnivore, if too close run away!!
                Vector3 threat = FindClosestThreat();
                float distOfThreat;

                if (targetList.Length > 0)
                {   
                
                    //foreach (GameObject potentialtarget in targetList) {
                    for (int i = 0; i < targetList.Length; i++)
                    {
                        GameObject potentialtarget = targetList [i];
                        diff = potentialtarget.transform.position - position;
                        curDistance = diff.sqrMagnitude;
                        
                        if (curDistance < distance)
                        {
                            closest = potentialtarget;
                            distance = curDistance;
                        }
                    }
                    //if there is a threat
                    if(threat != Vector3.zero)
                    {
                        //find how far threat is
                        diff = threat - position;
                        distOfThreat = diff.sqrMagnitude; 
                      
                       
                        //if threat is closer than food
                        if(distOfThreat <= 50.0f)
                        {
                            Debug.Log("Running!!");
                            return -threat;
                        }
                    }

                    return closest.transform.position;
                }
            }
            else if (animalType == AnimalType.carnivore)
            {
                targetList = GameObject.FindGameObjectsWithTag("Herbivore");

               
                if (targetList.Length > 0)
                {   
                    
                    //foreach (GameObject potentialtarget in targetList) {
                    for (int i = 0; i < targetList.Length; i++)
                    {
                        GameObject potentialtarget = targetList [i];
                        diff = potentialtarget.transform.position - position;
                        curDistance = diff.sqrMagnitude;
                        
                        if (curDistance < distance)
                        {
                            closest = potentialtarget;
                            distance = curDistance;
                        }

                    }
                    return closest.transform.position;
                }
            }



        } else //find mate
        {
//Find appropiate type of creature to narrow down search then add to searchlist.
            if(animalType == AnimalType.herbivore)
            {
                targetList = GameObject.FindGameObjectsWithTag("Herbivore");
            }
           else if(animalType == AnimalType.carnivore)
            {
                targetList = GameObject.FindGameObjectsWithTag("Carnivore");
            }
            else
            {
                targetList = GameObject.FindGameObjectsWithTag("Omnivore");
            }
          

            if (targetList.Length > 0)
            {   
                for (int i = 0; i < targetList.Length; i++)
                {
                    GameObject potentialtarget = targetList [i];
                    diff = potentialtarget.transform.position - position;
                    curDistance = diff.sqrMagnitude;
                    
                    if (curDistance < distance)
                    {
                        closest = potentialtarget;
                        distance = curDistance;
                    }
                }
                return closest.transform.position;
            }


        }
//returns a vector out side of game world if no target.
        return new Vector3(0, -99, 0);

    }

    void OnTriggerEnter(Collider col)
    {

        //If herbivore
        if (animalType == AnimalType.herbivore)
        {

            if (col.tag == "Plant")
            {

                PlantBehaviour plant = col.gameObject.GetComponent<PlantBehaviour>();
                EatPlant(plant);
                MoveBackwards(3 );

            } else if (col.tag == "Herbivore")
            {
                AnimalBehaviour animal = col.gameObject.GetComponent<AnimalBehaviour>();
               MoveBackwards(5);
                int side = 0;
                if(animal.position.x > position.x){side =1;}
                if(animal.position.x < position.x){side =2;}
                if(animal.position.z > position.z){side = 3;}
                if(animal.position.z < position.z){side = 4;}

                if(animal.position.x > position.x && animal.position.z > position.z){side =5;}
                if(animal.position.x < position.x && animal.position.z < position.z){side =6;}
                if(animal.position.z > position.z && animal.position.x > position.x){side = 7;}
                if(animal.position.z < position.z && animal.position.x < position.x){side = 8;}
                switch(side)
                {
                    case 1: MoveLeft(5); break;
                    case 2: MoveRight(5); break;
                    case 3: MoveForwards(); break;
                    case 4: MoveBackwards(5); break;
                    case 5 :MoveBackwards(5); MoveLeft(5); break;
                    case 6: MoveBackwards(5); MoveRight(5); break;
                    case 7:MoveForwards(); MoveLeft(5); break;
                    case 8: MoveForwards(); MoveRight(5); break;
                }
            }
        }
        if (animalType == AnimalType.carnivore)
        {
            if (col.tag == "Herbivore")
            {
                AnimalBehaviour animal = col.gameObject.GetComponent<AnimalBehaviour>();
                EatAnimal(animal);
                MoveBackwards(10);
            }
        }
    }

    void OldAgeDeathCheck()
    {
        //The chance that the animal will die due to old age
        float deathByAge = UnityEngine.Random.Range(0.0f, 1.0f);

        if (age <= avgMaxAge)
        { // 12.5% chance of death if young
            if (deathByAge <= 0.125f)
            {
                isReadyToDie = true;
                Debug.LogWarning("Oldage");
            }
        } else
        { // 50% chance of death if over avergae age
            if (deathByAge <= 0.5f)
            {
                isReadyToDie = true;
                Debug.LogWarning("Oldage");
            }
        }
    }

    public int GetSpeciiesID()
    {   
        return thisAnimalSpeciesID;
    }

    public bool isDead()
    {
        return isReadyToDie;
    }

    public bool GetIfReadyToPop()
    {
        if( energy >= 10.0f && reproduceTimer >= reproduceCycle)
        {
            isReadyToPopBaby = true;
            reproduceTimer = 0.0f;
        }
        return isReadyToPopBaby;
    }
    public void setReadyToPop(bool ready)
    {
        isReadyToPopBaby = ready;

    }
//EAT* are the same expect for different object types
//Could have made 1 object but decided to go with 2
//Esp as the target type is already known.
    void EatPlant(PlantBehaviour plant)
    {

        float maxFood = plant.StealEnergy(eatSize);
        maxFood = maxFood / eatEffeciency;

        energy += maxFood;
        hunger += maxFood;
    }

    void EatAnimal(AnimalBehaviour animal)
    {
        float maxFood = animal.StealEnergy(eatSize);
        maxFood = (maxFood / eatEffeciency);

        energy += maxFood;
        hunger += maxFood;
    }
//The amount of energy that is taken
    public float StealEnergy(float amountStolen)
    {
        energy -= amountStolen;
        return amountStolen;
    }

    public void newAnimal(AnimalType type, int ID, Vector3 pos, float eng, float maxAge, float MaxSize, float Speed,
float rot, float foodSize, float eatEff, float maxWillToLIVE)
    {

        animalType = type;
        thisAnimalSpeciesID = ID;
        alive = true;
        isOld = false;
        isReadyToDie = false;
        isReadyToPopBaby = false;
        
        position = pos;
        avgMaxAge = maxAge;
        maxSize = MaxSize;
        speed = Speed;// 0.075f;
        rotation = rot;
        eatSize = foodSize;
        eatEffeciency = eatEff;
        maxWillToLive = maxWillToLIVE;
        willToLive = maxWillToLIVE;

        age = 0;
        energy = 2;
        size = (maxSize / 4);
        hunger = 0.5f;

    }

    public float GetEnergy()
    {
        return energy;
    }
    void  ChangeEnergy(float energyChange)
        {
            energy += energyChange;
        }
    public Vector3 Getposition()
    {
        return position;
    }
    public AnimalType GetType()
    {
        return animalType;
    }
    public float GiveEnergyToYoung()
    {
        float en = energy / 3;
        energy -= en * 2;
        return en / 2;
    }
    public float GetMaxAge()
    {
        return avgMaxAge;
    }
    public float GetMaxSize()
    {
        return maxSize;
    }
    public float GetSpeed()
    {
        return speed;
    }
    public float GetRot()
    {
        return rotation;
    }

    public float GetFoodSize()
    {
        return eatSize;
    }
    public float GetEatEff()
    {
        return eatEffeciency;
    }
    public float GetWillToLive()
    {
        return willToLive;
    }
}






