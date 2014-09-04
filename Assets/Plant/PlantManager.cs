using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlantManager : MonoBehaviour {
	
    public int hardLimit =  500; //max number of plants at any one time
	public List<PlantBehaviour> plantList = new List<PlantBehaviour>();
	public List<int> speciesList = new List<int>();
	public PlantBehaviour plantObj;
	public float gameTime;
	public int plantSpeciesID;
	
	
	// Use this for initialization
	void Start () {
		plantSpeciesID = 0;
		speciesList.Add(0);
		gameTime =0;
	}


	// Update is called once per frame
	void Update () {
		gameTime += Time.deltaTime;
	
    //Start Simulation
		if(Input.GetKeyDown(KeyCode.Space))
		{
			PlantBehaviour plant;
			Vector3 pos =  new Vector3(0,0.5f,0);
			for(int i = 0; i <= 4; i++)
			{
				pos.x = Random.Range(-75,75);
				pos.z = Random.Range(-75,75);
			    plant = Instantiate(plantObj, pos, Quaternion.identity)as PlantBehaviour;
   		 	    plant.SetUpPlant(10.0f, pos,1.0f,2.5f,4.51f,2,25,4,0.05f,0.01f,0.02f,0);
			    plant.SetID(plantSpeciesID);
			    plantList.Add(plant);
			    speciesList[0]++;
			}
		}
		
		//if plant is dead remove from list
		for(int i = 0; i < plantList.Count;i++)
			{
			if(plantList[i].GetReadyToDie())
			{
                PlantBehaviour killplant = plantList[i]; 
                int specIndex = killplant.thisPlantSpeciesID;
                plantList.Remove(killplant);
				speciesList[specIndex]--;
                killplant.KillPlant();
			}

			//if plant is ready make more plants and if there is room
			else{ //protects against null
			if(plantList.Count <= hardLimit)
			{
				if(plantList[i].IsReadyToReproduce())
				{
					int seeds = plantList[i].GetCurrSeedCount();
					MakeNewPlants(seeds, i);
				}
			}
			}
		}
		
	}

	void MakeNewPlants(int seedCount, int index)
	{		
		PlantBehaviour plant;

		for(int i = 0; i < seedCount; i++)
		{		
					
			Vector3 pos = plantList[index].GetPosition();

			float dist = plantList[index].GetSeedDist();
			pos.x += Random.Range(-dist, dist);
			pos.z += Random.Range(-dist, dist);
			   		 	
			float mutateChance = plantList[index].GetChanceToMutate();
			float mutateThreshold = Random.Range(0.0f,1.0f);
			
			//Checks to see if the plant's child will become a different species
			//If so then it muattes one value
			if(mutateChance > mutateThreshold) 
			{
				//choose one to be mutated	
				
				MakeMutatedPlant(index);
				speciesList.Add(1);
				
			}
			//Create an identical plant
			//Copy all of the variables across
			else 
			{
				
				plant = Instantiate(plantObj, pos, Quaternion.identity)as PlantBehaviour;
				
				plant.SetUpPlant(plantList[index].GetLifeSpan(),pos,plantList[index].GetEnergyGatherRate(),
				plantList[index].GetMatTime(),plantList[index].GetReproduceSpeed(),
				plantList[index].GetMaxSeeds(), dist,plantList[index].GetSeedCost(), 
                mutateChance, plantList[index].GetTrunk(), plantList[index].GetFoliage(), plantList[index].GetGrowthType());
						
				int thisPlantID = plantList[index].thisPlantSpeciesID;
				plant.SetID(thisPlantID);
				
				plantList.Add(plant); 
				
				speciesList[thisPlantID]++;
			}  	
		}
		
		plantList[index].FinshSeedDispersal();
	}
	
	
	void MakeMutatedPlant(int index)
	{
			PlantBehaviour plant;
			float mutateValue = Random.Range(0.0f, 1.0f);
			int increaseValue = Random.Range(0,2); //max is exclusive
			
			float lifespan = plantList[index].GetLifeSpan();
			float enGathRate = plantList[index].GetEnergyGatherRate();
			float matTime = plantList[index].GetMatTime();
			float repSpeed = plantList[index].GetReproduceSpeed();
			int maxSeed = plantList[index].GetMaxSeeds();
			float seedDistance = plantList[index].GetSeedDist();
			float seedcost = plantList[index].GetSeedCost();
			float mutateChance = plantList[index].GetChanceToMutate();
             float trunk = plantList[index].GetTrunk();
            float foliage = plantList[index].GetFoliage();
         int growType = Random.Range(0,6); //int range is not max inclusive
		#region Mutate a value
		//Increase mutate chance at 1/6
		//instead of 1/3. This is to make sure not too many mutations occur
		// as well as to help simulate teh fact that evolution doesn't work very fast
			if(mutateValue <= 0.15f)
			{	
				if(increaseValue == 1)
				{
				lifespan += lifespan * 0.33f;
				}
				else
					{
				lifespan -= lifespan * 0.33f;
				}
					
			}
			if(mutateValue > 0.15f && mutateValue <= 0.3f)
			{
				if(increaseValue == 1)
				{
					enGathRate += enGathRate * 0.33f;
                    foliage += foliage/3;
				}
				else
				{
				    enGathRate -= enGathRate * 0.33f;
                    foliage -= foliage/3;
				}
                  

			}
			if(mutateValue > 0.3f && mutateValue <= 0.4f)
			{
				if(increaseValue == 1)
				{
					matTime += matTime * 0.33f;
                trunk += trunk/3;
				}
				else
				{
					matTime -= matTime * 0.33f;
                trunk -= trunk/3;
				}
			}
			if(mutateValue > 0.4f && mutateValue <= 0.5f)
			{
				if(increaseValue == 1)
				{
					repSpeed += repSpeed * 0.33f;
				}
				else
				{
					repSpeed -= repSpeed * 0.33f;
				}
			}
			if(mutateValue > 0.5f && mutateValue <= 0.6f)
			{
				if(increaseValue == 1)
				{
					maxSeed += maxSeed + 2;
                     trunk += trunk/4;
				}
				//Note:
				//This can result in no seeds being produced
				// In this case it's an
				//Evolutionary dead end
				else
				{	
					maxSeed -= maxSeed - 1;
                     trunk -= trunk/4;
				}
					
			}
			if(mutateValue > 0.6f && mutateValue <= 0.7f)
			{
				if(increaseValue == 1)
				{
					seedDistance += seedDistance * 0.33f;
                     trunk += trunk/4;
				}
				else
				{
					seedDistance -= seedDistance * 0.33f;
                    trunk -= trunk/4;
				}
			}
			if(mutateValue > 0.7f && mutateValue <= 0.85f)
			{
				if(increaseValue == 1)
				{
					seedcost += seedcost * 0.33f;
                    foliage += foliage/4;
				}
				else
				{
					seedcost -= seedcost * 0.33f;
                foliage -= foliage/4;
				}
			}
			if(mutateValue > 0.85f && mutateValue <= 1.0f)
			{
				if(increaseValue == 1)
				{
					mutateChance += mutateChance * 0.166f;
				}
				else
				{
					mutateChance -= mutateChance * 0.33f;
				}
			}
		#endregion
			
		Vector3 pos = plantList[index].GetPosition(); 
			
		plant = Instantiate(plantObj, pos, Quaternion.identity)as PlantBehaviour;
		//feed in the values including the changed one
		plant.SetUpPlant(lifespan,pos,enGathRate, matTime,repSpeed,
                         maxSeed, seedDistance,seedcost,mutateChance, 
                         trunk, foliage, growType);

		//increment then set ID
		plantSpeciesID++;
		plant.SetID(plantSpeciesID);

		plantList.Add(plant); 
	}
	
}
