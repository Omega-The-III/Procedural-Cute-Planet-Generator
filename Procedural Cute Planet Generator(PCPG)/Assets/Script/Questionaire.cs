using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Questionaire : MonoBehaviour
{
    public List<ThreshHoldType> ThreshHoldList = new List<ThreshHoldType>();
    public List<question> Questions = new List<question>();
    public QuestionaireObject questionObject;
    public GameObject buttonHolder;
    public GameObject buttonPrefab;
    private int currentQuestion = 0;

    [Header("Heightmap variables")]
    [SerializeField] private float mountainThreshold;
    [SerializeField] private float waterThreshold;
    [SerializeField] private float heightNoiseScale;

    [Header("Blendmap variables")]
    [SerializeField] private float blendNoiseScale;
    [SerializeField] private float grassThreshold;
    [SerializeField] private float desertThreshold;
    [SerializeField] private float snowThreshold;
    [SerializeField] private float volcanicThreshold;

    public List<Biomes> biomePriorityList = new List<Biomes>();
    public MapGenerator mapGenScript;
    void Start()
    {
        if (Questions.Count > 0)
        {
            //Set header and description text
            questionObject.header.text = Questions[0].header;
            questionObject.discription.text = Questions[0].questionDescription;

            //Remove all old buttons
            foreach (Transform child in buttonHolder.transform)
                Destroy(child.gameObject);

            //Reset button spawn position

            Vector3 buttonPos = buttonHolder.transform.position;
            
            //Spawn x amount of buttons with their own 'awnser' listener
            for (int i = 0; i < Questions[currentQuestion].awnsers.Length; i++)
            {
                GameObject newButton = Instantiate(buttonPrefab, new Vector3(buttonPos.x, buttonPos.y - i * 40, buttonPos.z), Quaternion.identity, buttonHolder.transform);
                var anwser = Questions[currentQuestion].awnsers[i];
                newButton.GetComponent<Button>().onClick.AddListener(delegate { InsertAwnser(anwser); });
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = Questions[currentQuestion].awnsers[i].awnserText;
            }
        }
    }
    public void InsertAwnser(awnser newAwnser)
    {
        switch (newAwnser.valueType)
        {
            case valueChangeType.mountain:
                mountainThreshold += newAwnser.value;
                break;
            case valueChangeType.ocean:
                waterThreshold += newAwnser.value;
                break;
            case valueChangeType.grassBiome:
                grassThreshold += newAwnser.value;
                break;
            case valueChangeType.snowBiome:
                snowThreshold += newAwnser.value;
                break;
            case valueChangeType.desertBiome:
                desertThreshold += newAwnser.value;
                break;
            case valueChangeType.volcanicBiome:
                volcanicThreshold += newAwnser.value;
                break;
        }
        NextQuestion();
    }
    public void NextQuestion()
    {
        currentQuestion++;
        if (currentQuestion < Questions.Count)
        {
            questionObject.header.text = Questions[currentQuestion].header;
            questionObject.discription.text = Questions[currentQuestion].questionDescription;

            foreach (Transform child in buttonHolder.transform)
                Destroy(child.gameObject);

            Vector3 buttonPos = buttonHolder.transform.position;
            for (int i = 0; i < Questions[currentQuestion].awnsers.Length; i++)
            {
                GameObject newButton = Instantiate(buttonPrefab, new Vector3(buttonPos.x, buttonPos.y - i * 40, buttonPos.z), Quaternion.identity, buttonHolder.transform);
                var anwser = Questions[currentQuestion].awnsers[i];
                newButton.GetComponent<Button>().onClick.AddListener(delegate { InsertAwnser(anwser); });
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = Questions[currentQuestion].awnsers[i].awnserText;
            }
        }
        else
        {
            questionObject.transform.parent.gameObject.SetActive(false);
            GeneratePlanet();
        }
    }
    private void GeneratePlanet()
    {
        //These values should be changed in the questionaire
        mountainThreshold = Mathf.Clamp(mountainThreshold, 0.5f, 0.7f);
        mapGenScript.mountainThreshold = mountainThreshold;

        if(mountainThreshold >= 0.7f)
            waterThreshold = Mathf.Clamp(waterThreshold, 0.4f, 0.5f);
        else
            waterThreshold = Mathf.Clamp(waterThreshold, 0.2f, 0.5f);
        mapGenScript.waterThreshold = waterThreshold;

        mapGenScript.heightNoiseScale = heightNoiseScale;

            //Biome stuff
        mapGenScript.blendNoiseScale = blendNoiseScale;
        mapGenScript.grassThreshold = grassThreshold;
        mapGenScript.desertThreshold = desertThreshold;
        mapGenScript.snowThreshold = snowThreshold;
        mapGenScript.volcanicThreshold = volcanicThreshold;

        /*ThreshHoldType grass = new ThreshHoldType();
        grass.value = grassThreshold;
        grass.valueType = valueChangeType.grassBiome;
        ThreshHoldList.Add(grass);

        ThreshHoldType desert = new ThreshHoldType();
        desert.value = desertThreshold;
        desert.valueType = valueChangeType.desertBiome;
        ThreshHoldList.Add(desert);

        ThreshHoldType snow = new ThreshHoldType();
        snow.value = snowThreshold;
        snow.valueType = valueChangeType.snowBiome;
        ThreshHoldList.Add(snow);

        ThreshHoldType volcanic = new ThreshHoldType();
        volcanic.value = volcanicThreshold;
        volcanic.valueType = valueChangeType.volcanicBiome;
        ThreshHoldList.Add(volcanic);
        ThreshHoldList.Sort(SortmyShit);*/

        biomePriorityList.Add(Biomes.desert);
        biomePriorityList.Add(Biomes.volcanic);
        biomePriorityList.Add(Biomes.snow);
        biomePriorityList.Add(Biomes.grass);

        mapGenScript.GenerateHeightMap();
        mapGenScript.GenerateBiomeBlendmap(biomePriorityList);
    }
    private static int SortmyShit(int fuck1, int fuck2)
    {
        if (fuck1 < fuck2)
            return fuck1;
        else
            return fuck2;
    }
}