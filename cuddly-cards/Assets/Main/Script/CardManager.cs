using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField]
    GameObject _cardBlueprint;

    [SerializeField]
    Transform _cardParent;

    readonly float CARDHEIGHT = 0.005f;

    float _jitterAmount = 0.01f;


    public void Start()
    {
        for (int i = 0; i < 100; i++)
        {            
            Instantiate(_cardBlueprint, new Vector3(Random.Range(-1f, 1f) * _jitterAmount, i * CARDHEIGHT, Random.Range(-1f, 1f) * _jitterAmount), Quaternion.identity, _cardParent);
        }

        
    }
}