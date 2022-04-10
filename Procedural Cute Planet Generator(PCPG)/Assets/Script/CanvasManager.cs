using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [Header("Questionaire variables")]
    public List<question> Questions = new List<question>();
    public QuestionaireObject questionaireRefrenceObject;
    [SerializeField] private GameObject buttonHolder;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private int buttonDist;

    [Header("Custom Edit Mode stuff")]
    [SerializeField] private GameObject editModeCanvas;
    [SerializeField] private GameObject sliderHolder;
    [SerializeField] private GameObject sliderPrefab;
    [SerializeField] private int sliderDistance;
    [SerializeField] private List<SliderObject> Sliders = new List<SliderObject>();
    private List<SliderRefrenceScript> SliderRefrences = new List<SliderRefrenceScript>();

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

    private int currentQuestion = 0;
    private bool editModeActive = false;
    void Start()
    {
        biomePriorityList.Add(Biomes.desert);
        biomePriorityList.Add(Biomes.volcanic);
        biomePriorityList.Add(Biomes.snow);
        biomePriorityList.Add(Biomes.grass);

        if (Questions.Count > 0)
        {
            //Set header and description text
            questionaireRefrenceObject.header.text = Questions[0].header;
            questionaireRefrenceObject.discription.text = Questions[0].questionDescription;

            //Reset button spawn position to start of button holder
            Vector3 buttonPos = buttonHolder.transform.position;
            
            //Spawn x amount of buttons with their own 'awnser' listener
            for (int i = 0; i < Questions[currentQuestion].awnsers.Length; i++)
            {
                GameObject newButton = Instantiate(buttonPrefab, new Vector3(buttonPos.x, buttonPos.y - i * buttonDist, buttonPos.z), Quaternion.identity, buttonHolder.transform);
                var anwser = Questions[currentQuestion].awnsers[i];
                newButton.GetComponent<Button>().onClick.AddListener(delegate { InsertAwnser(anwser); });
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = Questions[currentQuestion].awnsers[i].awnserText;
            }
        }

        int amountOfExistingSliders = 0;
        //Setup custom edit mode sliders
        for (int i = 0; i < Sliders.Count; i ++) 
        {
            Vector3 newPos;
            int cutOffPoint = 6;
            if (amountOfExistingSliders < cutOffPoint)
                newPos = sliderHolder.transform.position - new Vector3(0, i * sliderDistance, 0);
            else
                newPos = sliderHolder.transform.position - new Vector3(-sliderDistance * 6, i * sliderDistance - cutOffPoint * sliderDistance, 0);

            GameObject sliderObject = Instantiate(sliderPrefab, newPos, Quaternion.identity, sliderHolder.transform);
            SliderRefrenceScript sliderRefrence = sliderObject.GetComponent<SliderRefrenceScript>();

            sliderRefrence.header.text = Sliders[i].name;
            sliderRefrence.slider.value = Sliders[i].value;
            sliderRefrence.slider.minValue = Sliders[i].minVal;
            sliderRefrence.slider.maxValue = Sliders[i].maxVal;
            sliderRefrence.value.text = Sliders[i].value.ToString();

            amountOfExistingSliders++;
            SliderRefrences.Add(sliderRefrence);
        }
        editModeCanvas.SetActive(false);
    }
    private void FixedUpdate()
    {
        //This is kinda bad for performance but i ran out of time to make this :P
        if (editModeActive)
        {
            for (int i = 0; i < Sliders.Count; i++)
            {
                SliderRefrences[i].value.text = Mathf.Abs(SliderRefrences[i].slider.value).ToString();
            }
        }
    }

    #region questionStuff
    public void InsertAwnser(awnser newAwnser)
    {
        //Insert a new awnser into buttons OnClick() listeners. This corresponds to the awnsers biome type.
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
            questionaireRefrenceObject.header.text = Questions[currentQuestion].header;
            questionaireRefrenceObject.discription.text = Questions[currentQuestion].questionDescription;

            foreach (Transform child in buttonHolder.transform)
                Destroy(child.gameObject);

            Vector3 buttonPos = buttonHolder.transform.position;
            for (int i = 0; i < Questions[currentQuestion].awnsers.Length; i++)
            {
                GameObject newButton = Instantiate(buttonPrefab, new Vector3(buttonPos.x, buttonPos.y - i * buttonDist, buttonPos.z), Quaternion.identity, buttonHolder.transform);
                var anwser = Questions[currentQuestion].awnsers[i];
                newButton.GetComponent<Button>().onClick.AddListener(delegate { InsertAwnser(anwser); });
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = Questions[currentQuestion].awnsers[i].awnserText;
            }
        }
        //If there are no more questions left then start the planet generation process
        else
        {
            questionaireRefrenceObject.transform.parent.gameObject.SetActive(false);
            GeneratePlanet();
        }
    }
    #endregion

    #region buttonStuff
    public void ToCustomEditMode()
    {
        questionaireRefrenceObject.transform.gameObject.SetActive(false);
        editModeActive = true;
        editModeCanvas.SetActive(true);
    }
    public void FromCustomEditBack()
    {
        questionaireRefrenceObject.transform.gameObject.SetActive(true);
        editModeActive = false;
        editModeCanvas.SetActive(false);
    }
#endregion

    public void FinnishCustomEditMode()
    {
        editModeCanvas.SetActive(false);

        //Biome values
        mapGenScript.grassThreshold = SliderRefrences[0].slider.value;
        mapGenScript.desertThreshold = SliderRefrences[1].slider.value;
        mapGenScript.snowThreshold = SliderRefrences[2].slider.value;
        mapGenScript.volcanicThreshold = SliderRefrences[3].slider.value;

        //Height values
        mapGenScript.mountainThreshold = SliderRefrences[4].slider.value;
        mapGenScript.waterThreshold = SliderRefrences[5].slider.value;

        //Noises
        mapGenScript.blendNoiseScale = SliderRefrences[6].slider.value;
        mapGenScript.heightNoiseScale = SliderRefrences[7].slider.value;

        mapGenScript.GenerateHeightMap();
        mapGenScript.GenerateBiomeBlendmap(biomePriorityList);
    }
    private void GeneratePlanet()
    {
        //Clamps otherwise you get ridiculous/ugly planets.
        mountainThreshold = Mathf.Clamp(mountainThreshold, 0.5f, 0.7f);
        mapGenScript.mountainThreshold = mountainThreshold;

        if(mountainThreshold >= 0.7f)
            waterThreshold = Mathf.Clamp(waterThreshold, 0.4f, 0.5f);
        else
            waterThreshold = Mathf.Clamp(waterThreshold, 0.2f, 0.5f);
        mapGenScript.waterThreshold = waterThreshold;

        //Setting Noise values;
        mapGenScript.heightNoiseScale = heightNoiseScale;
        mapGenScript.blendNoiseScale = blendNoiseScale;

        //Passing on Biome data
        mapGenScript.grassThreshold = grassThreshold;
        mapGenScript.desertThreshold = desertThreshold;
        mapGenScript.snowThreshold = snowThreshold;
        mapGenScript.volcanicThreshold = volcanicThreshold;

        mapGenScript.GenerateHeightMap();
        mapGenScript.GenerateBiomeBlendmap(biomePriorityList);
    }
}