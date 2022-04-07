[System.Serializable]
public class question
{
    public string header;
    public string questionDescription;
    public awnser[] awnsers;
}

[System.Serializable]
public class awnser
{
    public string awnserText;
    public valueChangeType valueType;
    public float value;
}

[System.Serializable]
public class ThreshHoldType
{
    public float value;
    public valueChangeType valueType;
}