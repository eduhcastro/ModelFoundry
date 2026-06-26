using UnityEngine;

public sealed class OfficeVisualController : MonoBehaviour
{
    public static OfficeVisualController Instance { get; private set; }

    [Header("Office Tier Visual Groups")]
    [SerializeField] private GameObject t2ExpansionGroup;
    [SerializeField] private GameObject t3SecretLabGroup;
    [SerializeField] private GameObject t4DatacenterGroup;
    
    [Header("Old Walls to Deactivate on Upgrade")]
    [SerializeField] private GameObject t1LeftWalls;
    [SerializeField] private GameObject t1BackWalls;
    [SerializeField] private GameObject t1RightWalls;

    [Header("Workstations")]
    [SerializeField] private GameObject researchWorkstation;
    [SerializeField] private GameObject dataWorkstation;
    [SerializeField] private GameObject safetyWorkstation;
    [SerializeField] private GameObject infraWorkstation;
    [SerializeField] private GameObject gpuTechWorkstation;
    [SerializeField] private GameObject mlopsWorkstation;
    [SerializeField] private GameObject backendWorkstation;
    [SerializeField] private GameObject financeWorkstation;
    [SerializeField] private GameObject recruiterWorkstation;
    [SerializeField] private GameObject pmWorkstation;
    [SerializeField] private GameObject salesWorkstation;
    [SerializeField] private GameObject communityWorkstation;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Apply initial visual state based on GameManager's office tier
        if (GameManager.Instance != null)
        {
            ApplyOfficeVisuals(GameManager.Instance.OfficeTier);
        }
        else
        {
            ApplyOfficeVisuals(1);
        }
    }

    public void ApplyOfficeVisuals(int tier)
    {
        // Toggle expansions
        if (t2ExpansionGroup != null)
        {
            t2ExpansionGroup.SetActive(tier >= 2);
        }
        if (t3SecretLabGroup != null)
        {
            t3SecretLabGroup.SetActive(tier >= 3);
        }
        if (t4DatacenterGroup != null)
        {
            t4DatacenterGroup.SetActive(tier >= 4);
        }

        // Toggle old wall boundaries to merge spaces
        if (t1LeftWalls != null)
        {
            t1LeftWalls.SetActive(tier == 1);
        }
        if (t1BackWalls != null)
        {
            t1BackWalls.SetActive(tier <= 2);
        }
        if (t1RightWalls != null)
        {
            t1RightWalls.SetActive(tier < 4);
        }

        // Toggle workstation interactability/visibility
        if (researchWorkstation != null)
        {
            researchWorkstation.SetActive(tier >= 2);
        }
        if (dataWorkstation != null)
        {
            dataWorkstation.SetActive(tier >= 2);
        }
        if (financeWorkstation != null)
        {
            financeWorkstation.SetActive(tier >= 2);
        }
        if (recruiterWorkstation != null)
        {
            recruiterWorkstation.SetActive(tier >= 2);
        }

        if (safetyWorkstation != null)
        {
            safetyWorkstation.SetActive(tier >= 3);
        }
        if (communityWorkstation != null)
        {
            communityWorkstation.SetActive(tier >= 3);
        }

        if (infraWorkstation != null)
        {
            infraWorkstation.SetActive(tier >= 4);
        }
        if (gpuTechWorkstation != null)
        {
            gpuTechWorkstation.SetActive(tier >= 4);
        }
        if (mlopsWorkstation != null)
        {
            mlopsWorkstation.SetActive(tier >= 4);
        }
        if (backendWorkstation != null)
        {
            backendWorkstation.SetActive(tier >= 4);
        }
        if (pmWorkstation != null)
        {
            pmWorkstation.SetActive(tier >= 4);
        }
        if (salesWorkstation != null)
        {
            salesWorkstation.SetActive(tier >= 4);
        }
    }
}
