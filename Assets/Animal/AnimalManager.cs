using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimalManager : MonoBehaviour
{

    
    public int MaxNumberOfAnimals = 100;
    public List<int> speciesList = new List<int>();
    private AnimalBehaviour animalObj;
    public AnimalBehaviour herbivoreObj;
    public AnimalBehaviour carnivoreObj;
    public float gameTime;
    public int animalSpeciesID;
    public List<AnimalBehaviour> animalList = new List<AnimalBehaviour>();

    // Use this for initialization
    void Start()
    {
        animalSpeciesID = 0;
       
        speciesList.Add(0);
        gameTime = 0;
    }
    private void MakeCarnivore()
    {
        AnimalBehaviour animal;

        Vector3 pos = new Vector3(0, 0.5f, 0);
        animal = Instantiate(carnivoreObj, pos, Quaternion.identity)as AnimalBehaviour;
        
        animal.newAnimal(AnimalBehaviour.AnimalType.carnivore, 1, pos, 25, 20, 1,5, 0.075f, 4, 0.5f, 1.0f);

        animalList.Add(animal);
        speciesList.Add(1);
        speciesList [1]++;
    }
    // Update is called once per frame
    void Update()
    {
        gameTime += Time.deltaTime;

        //Start Simulation
        if (Input.GetKeyDown(KeyCode.Z))
        {
            AnimalBehaviour animal;
            animalObj = herbivoreObj;   
            Vector3 pos = new Vector3(0, 0.5f, 0);
            animal = Instantiate(animalObj, pos, Quaternion.identity)as AnimalBehaviour;

            animal.newAnimal(AnimalBehaviour.AnimalType.herbivore,0, pos, 15, 20, 1,4, 0.075f, 4, 0.5f, 1.0f);
            
            

            animalList.Add(animal);
            speciesList [0]++;

            Invoke("MakeCarnivore",0.2f *(60.0f)); // make carnivores after a given amount of minutes.
        }

        for (int i = 0; i < animalList.Count; i++)
        {
            
            if (animalList [i] == null)
            {
                int specIndex = animalList [i].thisAnimalSpeciesID;
                animalList.Remove(animalList [i]);
                speciesList[specIndex]--;
            }else{

                if (animalList [i].GetIfReadyToPop() && animalList.Count <= MaxNumberOfAnimals)
                {
                    MakeBaby(2, i);
                    animalList [i].setReadyToPop(false);

                }
            }
        }
    }

    public void MakeBaby(int numberOfBabies, int motherRef)
    {
        float mutate = Random.Range(1, 100);
        AnimalBehaviour animal;
        AnimalBehaviour mother = animalList [motherRef];
        Vector3 pos = mother.Getposition();
        float newPosX = Random.Range(-5.0f, 5.0f);
        float newPosZ = Random.Range(-5.0f, 5.0f);
        AnimalBehaviour.AnimalType type;
        int ID = mother.GetSpeciiesID();
        float energy, maxAge, maxSize;
        float speed, rot, foodSize, eatEff, willToLive;

       for (int i = 0; i < numberOfBabies; i++)
        {
       
        
            type = mother.GetType();
            energy = mother.GiveEnergyToYoung();
            maxAge = mother.GetMaxAge();
            maxSize = mother.GetMaxSize();
            speed = mother.GetSpeed();
            rot = mother.GetRot();
            foodSize = mother.GetFoodSize();
            eatEff = mother.GetEatEff();
            willToLive = mother.GetWillToLive();
            pos = new Vector3(pos.x + newPosX, pos.y, pos.z + newPosZ);

        

            if (mutate <= 20)
            { //make a new species
                //Randomise stuff here
                ID++;
            }

            if(type == AnimalBehaviour.AnimalType.herbivore)
            {
                animalObj = herbivoreObj;
            }
            else{
                animalObj =  carnivoreObj;  
            }

            animal = Instantiate(animalObj, pos, Quaternion.identity)as AnimalBehaviour;
            animal.newAnimal(type, ID, pos, energy, maxAge, maxSize, speed, rot, foodSize, eatEff, willToLive);
            animalList.Add(animal);

            speciesList.Add(ID);
            speciesList[ID]++;
        }
    }


}
