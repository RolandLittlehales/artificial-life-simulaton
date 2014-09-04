using UnityEngine;
using System.Collections;

//A plant.
//When lifespan, energy or health(not used) runs out it dies.
//Energy is used for it do things (Create seeds, heal, etc)

public class PlantBehaviour : MonoBehaviour
{
    public int thisPlantSpeciesID;
    private Vector3 position;
    private float lifeSpan; //How long is the max life of this plant
    private float currentLife; //How long the plant will live for
    private float energy;   //How much energy it has
    private float energyGatherRate;//How fast it gathers energy. Only starts when plant is mature
    private float health;       //How much health it has
    private float maturationTime; //How long till it can reproduce
    private bool isGrownUp; //If it has finished maturing
    float trunkSize;
    float foliageSize;
    int growthType;
    //Reproduce variables
    //Each seed create 1 new plant
    private float reproduceSpeed; //How long it takes till it can send it's seeds out
    private float resetRepSpeed; //Used to rest timer
    private int maxSeeds; // How many seeds it can have at once
    private int currSeeds; //Current amount of seeds
    private float seedDist; //How far the seed can travel before planting
    private float seedCost; //How much each seed costs to be created
    private bool readyToReproduce; //Tells plant manager that it is ready to reproduce
    
    //Heal variables
    //Not used at the moment
    private float healSelf; //How much it costs to heal some of it's self
    private float healAmount; //How much it heals each time
    
    
    //Misc Variabales
    private bool readyToDie;    //If it is ready to be killed off
    private float chanceToMutate; //What chance it has to mutate a variable
    private int secondsLeft;

    void Start()
    {
        readyToDie = false; 
        isGrownUp = false;
        energy = 0.5f;
        secondsLeft = 0;
    }
    
    //Sets plant up with all of the values needed to start it's life
    //if not used plant will instantly die
    public void SetUpPlant(float lifespan, Vector3 pos, float enGathRate, float matTime, 
                           float repSpeed, int maxSeed, float seedDistance, float seedcost, 
                           float mutateChance, float trunkRate, float foliageRate, int growthType)
    {
            
        currentLife = 0;
        lifeSpan = lifespan;
        position = pos;
        energyGatherRate = enGathRate;
        maturationTime = matTime;
        
        reproduceSpeed = repSpeed;
        resetRepSpeed = repSpeed;
        maxSeeds = maxSeed;
        seedDist = seedDistance;
        seedCost = seedcost;
            
        chanceToMutate = mutateChance;
       
        Foliage treetop = GetComponentInChildren<Foliage>();
        Trunk trunk =  GetComponentInChildren<Trunk>();
        trunkSize = trunkRate;
        foliageSize= foliageRate;
        trunk.SetGrowthRate(trunkSize, growthType);
        treetop.SetGrowthRate(foliageSize, growthType, trunk.GetVerticalGrowthRate());

    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        currentLife += deltaTime;//Update life left
       // transform.localScale += new Vector3(foliageSize*energyGatherRate, trunkSize*energyGatherRate, foliageSize*energyGatherRate);
        if(transform.position.y < position.y)
        {
            transform.position.Set(position.x,position.y,position.z);   
        }
        //Checks if plant is an adult
        //if so stop up dating maturation time
        //and start making seeds
        

        if (!isGrownUp)
        {
            maturationTime -= deltaTime;
            if (maturationTime <= 0)
            {
                isGrownUp = true;   
            }
            
            
        } else
        {
            //increment energy levels
            energy += (energyGatherRate * deltaTime);
            
            reproduceSpeed -= deltaTime; // updates timer
            
            if (reproduceSpeed <= 0)
            {
                reproduceSpeed = resetRepSpeed;
                if (currSeeds > 0)
                {
                    readyToReproduce = true;    
                }
            }
                
                
            //Create new seed
            if (energy > seedCost)
            {
                if (currSeeds != maxSeeds && energy > seedCost)
                {
                    energy -= seedCost;
                    currSeeds++;
                }
            }
            #region Kill plant conditions
            //'Unstable' ones are killed off by thier mutations
            //Used to simulate terminal natural birth defects another problems
            //Also used to help keep plant numbers down to limit simulation size

            if ((currentLife - secondsLeft) >= 1.0f)
            {
                float chanceToDie = Random.Range(0.0f, 1.0f);
                
                if (chanceToMutate > chanceToDie)
                {
                    Debug.LogWarning("MutateKill");
                    readyToDie = true;  
                }
            }
            secondsLeft = (int)currentLife; //Updated after so that doesn't test as often
            
            //If it falls off the world kill it
            if (position.y < 0)
            {
                readyToDie = true;  
            }
            
            //If lifespan is done
            if (currentLife >= lifeSpan)
            {
                readyToDie = true;  
            }
            //if no energy left
            if (energy <= 0)
            {
                Debug.LogWarning("EnergyDeath");
                readyToDie = true;  
            }
            
            //if Ready to be killed
            //Kill it
            /*if (readyToDie)
            {
                Destroy(gameObject);
            }*/
            #endregion
        }
    }

    public bool IsReadyToReproduce()
    {
        return readyToReproduce;    
    }

    public void FinshSeedDispersal()
    {
        readyToReproduce = false;
        
    }
    public void KillPlant()
    {
        Destroy(gameObject);
    }
    public void IsReadyToDie()
    {
        readyToDie = true;      
    }
    public bool GetReadyToDie()
    {
        return  readyToDie;      
    }

    public float StealEnergy(float amountStolen)
    {
        energy -= amountStolen; 
        return amountStolen; 
    }
    
    
    
    
    #region Get methods
    public int GetGrowthType()
    {
        return growthType;
    }
    public float GetFoliage()
    {
        return foliageSize;
    }
    public float GetTrunk()
    {
        return trunkSize;
    }
    public Vector3 GetPosition()
    {
        return position;    
    }

    public float GetLifeSpan()
    {
        return lifeSpan;    
    }

    public float GetEnergyGatherRate()
    {
        return energyGatherRate;    
    }

    public float GetMatTime()
    {
        return maturationTime;  
    }

    public float GetReproduceSpeed()
    {
        return reproduceSpeed;  
    }
    
    public int GetCurrSeedCount()
    {
        return currSeeds;   
    }

    public float GetSeedCost()
    {
        return seedCost;    
    }

    public float GetSeedDist()
    {
        return seedDist;    
    }

    public int GetMaxSeeds()
    {
        return  maxSeeds;
    }

    public float GetChanceToMutate()
    {
        return chanceToMutate;  
    }
    
    public void SetID(int ID)
    {
        thisPlantSpeciesID = ID;    
    }
    #endregion
    
    
    
    
}
