using UnityEngine;

public class Pedestal : MonoBehaviour
{
    [Header("Pedestal Settings")]
    [SerializeField] private int pedestalID = 0; // 0, 1, 2 for the three pedestals
    
    public int GetPedestalID()
    {
        return pedestalID;
    }
} 