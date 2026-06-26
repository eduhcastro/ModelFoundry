using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class HiringController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PrototypeProjectController projectController;

    [Header("ML Engineer (Tier 1)")]
    [SerializeField] private Button mlButton;
    [SerializeField] private GameObject mlAgentObj;
    [SerializeField] private PrototypeWorkstation mlWorkstation;
    [SerializeField] private Transform[] mlWaypoints;
    [SerializeField] private float mlCost = 5000f;

    [Header("Research Scientist (Tier 2)")]
    [SerializeField] private Button scientistButton;
    [SerializeField] private GameObject scientistAgentObj;
    [SerializeField] private PrototypeWorkstation scientistWorkstation;
    [SerializeField] private Transform[] scientistWaypoints;
    [SerializeField] private float scientistCost = 8000f;

    [Header("Data Engineer (Tier 2)")]
    [SerializeField] private Button dataButton;
    [SerializeField] private GameObject dataAgentObj;
    [SerializeField] private PrototypeWorkstation dataWorkstation;
    [SerializeField] private Transform[] dataWaypoints;
    [SerializeField] private float dataCost = 6000f;

    [Header("Safety Researcher (Tier 3)")]
    [SerializeField] private Button safetyButton;
    [SerializeField] private GameObject safetyAgentObj;
    [SerializeField] private PrototypeWorkstation safetyWorkstation;
    [SerializeField] private Transform[] safetyWaypoints;
    [SerializeField] private float safetyCost = 12000f;

    [Header("Infrastructure Engineer (Tier 4)")]
    [SerializeField] private Button infraButton;
    [SerializeField] private GameObject infraAgentObj;
    [SerializeField] private PrototypeWorkstation infraWorkstation;
    [SerializeField] private Transform[] infraWaypoints;
    [SerializeField] private float infraCost = 10000f;

    [Header("GPU Technician (Tier 4)")]
    [SerializeField] private Button gpuTechButton;
    [SerializeField] private GameObject gpuTechAgentObj;
    [SerializeField] private PrototypeWorkstation gpuTechWorkstation;
    [SerializeField] private Transform[] gpuTechWaypoints;
    [SerializeField] private float gpuTechCost = 7000f;

    [Header("MLOps Engineer (Tier 4)")]
    [SerializeField] private Button mlopsButton;
    [SerializeField] private GameObject mlopsAgentObj;
    [SerializeField] private PrototypeWorkstation mlopsWorkstation;
    [SerializeField] private Transform[] mlopsWaypoints;
    [SerializeField] private float mlopsCost = 9000f;

    [Header("Backend Engineer (Tier 4)")]
    [SerializeField] private Button backendButton;
    [SerializeField] private GameObject backendAgentObj;
    [SerializeField] private PrototypeWorkstation backendWorkstation;
    [SerializeField] private Transform[] backendWaypoints;
    [SerializeField] private float backendCost = 6000f;

    [Header("Finance Lead (Tier 2)")]
    [SerializeField] private Button financeButton;
    [SerializeField] private GameObject financeAgentObj;
    [SerializeField] private PrototypeWorkstation financeWorkstation;
    [SerializeField] private Transform[] financeWaypoints;
    [SerializeField] private float financeCost = 15000f;

    [Header("Recruiter (Tier 2)")]
    [SerializeField] private Button recruiterButton;
    [SerializeField] private GameObject recruiterAgentObj;
    [SerializeField] private PrototypeWorkstation recruiterWorkstation;
    [SerializeField] private Transform[] recruiterWaypoints;
    [SerializeField] private float recruiterCost = 12000f;

    [Header("Product Manager (Tier 4)")]
    [SerializeField] private Button pmButton;
    [SerializeField] private GameObject pmAgentObj;
    [SerializeField] private PrototypeWorkstation pmWorkstation;
    [SerializeField] private Transform[] pmWaypoints;
    [SerializeField] private float pmCost = 18000f;

    [Header("Sales Executive (Tier 4)")]
    [SerializeField] private Button salesButton;
    [SerializeField] private GameObject salesAgentObj;
    [SerializeField] private PrototypeWorkstation salesWorkstation;
    [SerializeField] private Transform[] salesWaypoints;
    [SerializeField] private float salesCost = 14000f;

    [Header("Community Manager (Tier 3)")]
    [SerializeField] private Button communityButton;
    [SerializeField] private GameObject communityAgentObj;
    [SerializeField] private PrototypeWorkstation communityWorkstation;
    [SerializeField] private Transform[] communityWaypoints;
    [SerializeField] private float communityCost = 10000f;

    private void Start()
    {
        if (mlButton != null) mlButton.onClick.AddListener(HireMLEngineer);
        if (scientistButton != null) scientistButton.onClick.AddListener(HireResearchScientist);
        if (dataButton != null) dataButton.onClick.AddListener(HireDataEngineer);
        if (safetyButton != null) safetyButton.onClick.AddListener(HireSafetyResearcher);
        if (infraButton != null) infraButton.onClick.AddListener(HireInfrastructureEngineer);
        if (gpuTechButton != null) gpuTechButton.onClick.AddListener(HireGpuTechnician);
        if (mlopsButton != null) mlopsButton.onClick.AddListener(HireMlopsEngineer);
        if (backendButton != null) backendButton.onClick.AddListener(HireBackendEngineer);
        if (financeButton != null) financeButton.onClick.AddListener(HireFinanceLead);
        if (recruiterButton != null) recruiterButton.onClick.AddListener(HireRecruiter);
        if (pmButton != null) pmButton.onClick.AddListener(HireProductManager);
        if (salesButton != null) salesButton.onClick.AddListener(HireSalesExecutive);
        if (communityButton != null) communityButton.onClick.AddListener(HireCommunityManager);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += UpdateButtonsState;
        }

        UpdateButtonsState();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= UpdateButtonsState;
        }
    }

    public void HireMLEngineer()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.HasMLEngineer) return;

        if (!gm.SpendCash(mlCost))
        {
            ToastNotification.ShowGlobal("Not enough cash to hire an ML Engineer!", ToastNotification.Category.Danger);
            return;
        }

        gm.SetHasMLEngineer(true);
        ActivateAgent(mlAgentObj, mlWorkstation, mlWaypoints);

        if (projectController != null)
        {
            projectController.RecalculateSpecs();
        }

        gm.AddCompetence(15f);
        ToastNotification.ShowGlobal("ML Engineer hired! Salary: $1,200/mo. +15 Competence!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    public void HireResearchScientist()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.HasResearchScientist) return;

        if (gm.OfficeTier < 2)
        {
            ToastNotification.ShowGlobal("Requires Office Tier 2 (Corporate Suite)!", ToastNotification.Category.Warning);
            return;
        }

        if (!gm.SpendCash(scientistCost))
        {
            ToastNotification.ShowGlobal("Not enough cash to hire a Research Scientist!", ToastNotification.Category.Danger);
            return;
        }

        gm.SetHasResearchScientist(true);
        ActivateAgent(scientistAgentObj, scientistWorkstation, scientistWaypoints);

        gm.AddCompetence(25f);
        ToastNotification.ShowGlobal("Research Scientist hired! Salary: $2,500/mo. +25 Competence!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    public void HireDataEngineer()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.HasDataEngineer) return;

        if (gm.OfficeTier < 2)
        {
            ToastNotification.ShowGlobal("Requires Office Tier 2 (Corporate Suite)!", ToastNotification.Category.Warning);
            return;
        }

        if (!gm.SpendCash(dataCost))
        {
            ToastNotification.ShowGlobal("Not enough cash to hire a Data Engineer!", ToastNotification.Category.Danger);
            return;
        }

        gm.SetHasDataEngineer(true);
        ActivateAgent(dataAgentObj, dataWorkstation, dataWaypoints);

        gm.AddCompetence(20f);
        ToastNotification.ShowGlobal("Data Engineer hired! Salary: $1,800/mo. Model Quality +20% on launch!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    public void HireSafetyResearcher()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.HasSafetyResearcher) return;

        if (gm.OfficeTier < 3)
        {
            ToastNotification.ShowGlobal("Requires Office Tier 3 (Secret R&D Lab)!", ToastNotification.Category.Warning);
            return;
        }

        if (!gm.SpendCash(safetyCost))
        {
            ToastNotification.ShowGlobal("Not enough cash to hire a Safety Researcher!", ToastNotification.Category.Danger);
            return;
        }

        gm.SetHasSafetyResearcher(true);
        ActivateAgent(safetyAgentObj, safetyWorkstation, safetyWaypoints);

        gm.AddCompetence(30f);
        ToastNotification.ShowGlobal("Safety Researcher hired! Salary: $3,500/mo. Safety risk & penalties mitigated!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    public void HireInfrastructureEngineer()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.HasInfrastructureEngineer) return;

        if (gm.OfficeTier < 4)
        {
            ToastNotification.ShowGlobal("Requires Office Tier 4 (Modular Datacenter)!", ToastNotification.Category.Warning);
            return;
        }

        if (!gm.SpendCash(infraCost))
        {
            ToastNotification.ShowGlobal("Not enough cash to hire an Infrastructure Engineer!", ToastNotification.Category.Danger);
            return;
        }

        gm.SetHasInfrastructureEngineer(true);
        ActivateAgent(infraAgentObj, infraWorkstation, infraWaypoints);

        gm.AddCompetence(20f);
        ToastNotification.ShowGlobal("Infrastructure Engineer hired! Salary: $2,800/mo. GPU power draw -25%!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    public void HireGpuTechnician()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.HasGpuTechnician) return;

        if (gm.OfficeTier < 4)
        {
            ToastNotification.ShowGlobal("Requires Office Tier 4 (Modular Datacenter)!", ToastNotification.Category.Warning);
            return;
        }

        if (!gm.SpendCash(gpuTechCost))
        {
            ToastNotification.ShowGlobal("Not enough cash to hire a GPU Technician!", ToastNotification.Category.Danger);
            return;
        }

        gm.SetHasGpuTechnician(true);
        ActivateAgent(gpuTechAgentObj, gpuTechWorkstation, gpuTechWaypoints);

        gm.AddCompetence(15f);
        ToastNotification.ShowGlobal("GPU Technician hired! Salary: $1,900/mo. Heat -10%, Training speed +10%!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    public void HireMlopsEngineer()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.HasMlopsEngineer) return;

        if (gm.OfficeTier < 4)
        {
            ToastNotification.ShowGlobal("Requires Office Tier 4 (Modular Datacenter)!", ToastNotification.Category.Warning);
            return;
        }

        if (!gm.SpendCash(mlopsCost))
        {
            ToastNotification.ShowGlobal("Not enough cash to hire an MLOps Engineer!", ToastNotification.Category.Danger);
            return;
        }

        gm.SetHasMlopsEngineer(true);
        ActivateAgent(mlopsAgentObj, mlopsWorkstation, mlopsWaypoints);

        gm.AddCompetence(25f);
        ToastNotification.ShowGlobal("MLOps Engineer hired! Salary: $2,200/mo. Model inference cost -20%!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    public void HireBackendEngineer()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.HasBackendEngineer) return;

        if (gm.OfficeTier < 4)
        {
            ToastNotification.ShowGlobal("Requires Office Tier 4 (Modular Datacenter)!", ToastNotification.Category.Warning);
            return;
        }

        if (!gm.SpendCash(backendCost))
        {
            ToastNotification.ShowGlobal("Not enough cash to hire a Backend Engineer!", ToastNotification.Category.Danger);
            return;
        }

        gm.SetHasBackendEngineer(true);
        ActivateAgent(backendAgentObj, backendWorkstation, backendWaypoints);

        gm.AddCompetence(15f);
        ToastNotification.ShowGlobal("Backend Engineer hired! Salary: $1,500/mo. Active contract slots +1!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    public void HireFinanceLead()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.HasFinanceLead) return;

        if (gm.OfficeTier < 2)
        {
            ToastNotification.ShowGlobal("Requires Office Tier 2 (Corporate Suite)!", ToastNotification.Category.Warning);
            return;
        }

        float actualCost = financeCost * (gm.HasRecruiter ? 0.7f : 1.0f);
        if (!gm.SpendCash(actualCost))
        {
            ToastNotification.ShowGlobal("Not enough cash to hire a Finance Lead!", ToastNotification.Category.Danger);
            return;
        }

        gm.SetHasFinanceLead(true);
        ActivateAgent(financeAgentObj, financeWorkstation, financeWaypoints);

        gm.AddCompetence(20f);
        ToastNotification.ShowGlobal("Finance Lead hired! Salary: $3,500/mo. Burn rate -10%, Funding evaluations +20%!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    public void HireRecruiter()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.HasRecruiter) return;

        if (gm.OfficeTier < 2)
        {
            ToastNotification.ShowGlobal("Requires Office Tier 2 (Corporate Suite)!", ToastNotification.Category.Warning);
            return;
        }

        if (!gm.SpendCash(recruiterCost))
        {
            ToastNotification.ShowGlobal("Not enough cash to hire a Recruiter!", ToastNotification.Category.Danger);
            return;
        }

        gm.SetHasRecruiter(true);
        ActivateAgent(recruiterAgentObj, recruiterWorkstation, recruiterWaypoints);

        gm.AddCompetence(10f);
        ToastNotification.ShowGlobal("Recruiter hired! Salary: $2,500/mo. Future hiring costs -30%, Competence +10!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    public void HireProductManager()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.HasProductManager) return;

        if (gm.OfficeTier < 4)
        {
            ToastNotification.ShowGlobal("Requires Office Tier 4 (Modular Datacenter)!", ToastNotification.Category.Warning);
            return;
        }

        float actualCost = pmCost * (gm.HasRecruiter ? 0.7f : 1.0f);
        if (!gm.SpendCash(actualCost))
        {
            ToastNotification.ShowGlobal("Not enough cash to hire a Product Manager!", ToastNotification.Category.Danger);
            return;
        }

        gm.SetHasProductManager(true);
        ActivateAgent(pmAgentObj, pmWorkstation, pmWaypoints);

        gm.AddCompetence(25f);
        ToastNotification.ShowGlobal("Product Manager hired! Salary: $4,000/mo. Training speed +10%, Model quality +5!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    public void HireSalesExecutive()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.HasSalesExecutive) return;

        if (gm.OfficeTier < 4)
        {
            ToastNotification.ShowGlobal("Requires Office Tier 4 (Modular Datacenter)!", ToastNotification.Category.Warning);
            return;
        }

        float actualCost = salesCost * (gm.HasRecruiter ? 0.7f : 1.0f);
        if (!gm.SpendCash(actualCost))
        {
            ToastNotification.ShowGlobal("Not enough cash to hire a Sales Executive!", ToastNotification.Category.Danger);
            return;
        }

        gm.SetHasSalesExecutive(true);
        ActivateAgent(salesAgentObj, salesWorkstation, salesWaypoints);

        gm.AddCompetence(20f);
        ToastNotification.ShowGlobal("Sales Executive hired! Salary: $3,000/mo. Contract payouts & model revenue +20%, +1 Contract slot!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    public void HireCommunityManager()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.HasCommunityManager) return;

        if (gm.OfficeTier < 3)
        {
            ToastNotification.ShowGlobal("Requires Office Tier 3 (Secret R&D Lab)!", ToastNotification.Category.Warning);
            return;
        }

        float actualCost = communityCost * (gm.HasRecruiter ? 0.7f : 1.0f);
        if (!gm.SpendCash(actualCost))
        {
            ToastNotification.ShowGlobal("Not enough cash to hire a Community Manager!", ToastNotification.Category.Danger);
            return;
        }

        gm.SetHasCommunityManager(true);
        ActivateAgent(communityAgentObj, communityWorkstation, communityWaypoints);

        gm.AddCompetence(15f);
        ToastNotification.ShowGlobal("Community Manager hired! Salary: $2,000/mo. Follower growth +50%, inactivity penalties halved!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    private void ActivateAgent(GameObject agentObj, PrototypeWorkstation workstation, Transform[] wps)
    {
        if (agentObj != null)
        {
            agentObj.SetActive(true);
            var agent = agentObj.GetComponent<PrototypeEmployeeAgent>();
            if (agent == null)
            {
                agent = agentObj.AddComponent<PrototypeEmployeeAgent>();
            }

            agent.InitializeAgent(workstation, wps);

            if (workstation != null)
            {
                workstation.AssignedAgent = agent;
            }

            agent.StartWalking();
        }
    }

    private void DeactivateAgent(GameObject agentObj, PrototypeWorkstation workstation)
    {
        if (agentObj != null)
        {
            agentObj.SetActive(false);
        }
        if (workstation != null)
        {
            workstation.AssignedAgent = null;
        }
    }

    public void UpdateButtonsState()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        bool hasRecruiter = gm.HasRecruiter;

        // ML Engineer Button
        UpdateRoleButton(mlButton, gm.HasMLEngineer, true, $"HIRE ML (${(mlCost * (hasRecruiter ? 0.7f : 1.0f)):N0})");

        // Scientist Button
        UpdateRoleButton(scientistButton, gm.HasResearchScientist, gm.OfficeTier >= 2, $"HIRE SCIENTIST (${(scientistCost * (hasRecruiter ? 0.7f : 1.0f)):N0})");

        // Data Engineer Button
        UpdateRoleButton(dataButton, gm.HasDataEngineer, gm.OfficeTier >= 2, $"HIRE DATA ENG (${(dataCost * (hasRecruiter ? 0.7f : 1.0f)):N0})");

        // Safety Researcher Button
        UpdateRoleButton(safetyButton, gm.HasSafetyResearcher, gm.OfficeTier >= 3, $"HIRE SAFETY (${(safetyCost * (hasRecruiter ? 0.7f : 1.0f)):N0})");

        // Infrastructure Engineer Button
        UpdateRoleButton(infraButton, gm.HasInfrastructureEngineer, gm.OfficeTier >= 4, $"HIRE INFRA (${(infraCost * (hasRecruiter ? 0.7f : 1.0f)):N0})");

        // GPU Technician Button
        UpdateRoleButton(gpuTechButton, gm.HasGpuTechnician, gm.OfficeTier >= 4, $"HIRE GPU TECH (${(gpuTechCost * (hasRecruiter ? 0.7f : 1.0f)):N0})");

        // MLOps Engineer Button
        UpdateRoleButton(mlopsButton, gm.HasMlopsEngineer, gm.OfficeTier >= 4, $"HIRE MLOPS (${(mlopsCost * (hasRecruiter ? 0.7f : 1.0f)):N0})");

        // Backend Engineer Button
        UpdateRoleButton(backendButton, gm.HasBackendEngineer, gm.OfficeTier >= 4, $"HIRE BACKEND (${(backendCost * (hasRecruiter ? 0.7f : 1.0f)):N0})");

        // Finance Lead Button
        UpdateRoleButton(financeButton, gm.HasFinanceLead, gm.OfficeTier >= 2, $"HIRE FINANCE (${(financeCost * (hasRecruiter ? 0.7f : 1.0f)):N0})");

        // Recruiter Button (not discounted by itself)
        UpdateRoleButton(recruiterButton, gm.HasRecruiter, gm.OfficeTier >= 2, $"HIRE RECRUITER (${recruiterCost:N0})");

        // PM Button
        UpdateRoleButton(pmButton, gm.HasProductManager, gm.OfficeTier >= 4, $"HIRE PM (${(pmCost * (hasRecruiter ? 0.7f : 1.0f)):N0})");

        // Sales Executive Button
        UpdateRoleButton(salesButton, gm.HasSalesExecutive, gm.OfficeTier >= 4, $"HIRE SALES (${(salesCost * (hasRecruiter ? 0.7f : 1.0f)):N0})");

        // Community Manager Button
        UpdateRoleButton(communityButton, gm.HasCommunityManager, gm.OfficeTier >= 3, $"HIRE COMMUNITY (${(communityCost * (hasRecruiter ? 0.7f : 1.0f)):N0})");
    }

    private void UpdateRoleButton(Button btn, bool hired, bool officeTierUnlocked, string activeLabel)
    {
        if (btn == null) return;
        var text = btn.GetComponentInChildren<TextMeshProUGUI>();

        if (hired)
        {
            btn.interactable = false;
            if (text != null) text.text = "HIRED";
        }
        else if (!officeTierUnlocked)
        {
            btn.interactable = false;
            if (text != null) text.text = "LOCKED";
        }
        else
        {
            btn.interactable = true;
            if (text != null) text.text = activeLabel;
        }
    }

    public void LoadHiredStates(
        bool hiredMl, 
        bool hiredScientist, 
        bool hiredData, 
        bool hiredSafety,
        bool hiredInfra = false,
        bool hiredGpuTech = false,
        bool hiredMlops = false,
        bool hiredBackend = false,
        bool hiredFinance = false,
        bool hiredRecruiter = false,
        bool hiredPm = false,
        bool hiredSales = false,
        bool hiredCommunity = false)
    {
        // ML Engineer
        if (hiredMl) ActivateAgent(mlAgentObj, mlWorkstation, mlWaypoints);
        else DeactivateAgent(mlAgentObj, mlWorkstation);

        // Research Scientist
        if (hiredScientist) ActivateAgent(scientistAgentObj, scientistWorkstation, scientistWaypoints);
        else DeactivateAgent(scientistAgentObj, scientistWorkstation);

        // Data Engineer
        if (hiredData) ActivateAgent(dataAgentObj, dataWorkstation, dataWaypoints);
        else DeactivateAgent(dataAgentObj, dataWorkstation);

        // Safety Researcher
        if (hiredSafety) ActivateAgent(safetyAgentObj, safetyWorkstation, safetyWaypoints);
        else DeactivateAgent(safetyAgentObj, safetyWorkstation);

        // Infrastructure Engineer
        if (hiredInfra) ActivateAgent(infraAgentObj, infraWorkstation, infraWaypoints);
        else DeactivateAgent(infraAgentObj, infraWorkstation);

        // GPU Technician
        if (hiredGpuTech) ActivateAgent(gpuTechAgentObj, gpuTechWorkstation, gpuTechWaypoints);
        else DeactivateAgent(gpuTechAgentObj, gpuTechWorkstation);

        // MLOps Engineer
        if (hiredMlops) ActivateAgent(mlopsAgentObj, mlopsWorkstation, mlopsWaypoints);
        else DeactivateAgent(mlopsAgentObj, mlopsWorkstation);

        // Backend Engineer
        if (hiredBackend) ActivateAgent(backendAgentObj, backendWorkstation, backendWaypoints);
        else DeactivateAgent(backendAgentObj, backendWorkstation);

        // Finance Lead
        if (hiredFinance) ActivateAgent(financeAgentObj, financeWorkstation, financeWaypoints);
        else DeactivateAgent(financeAgentObj, financeWorkstation);

        // Recruiter
        if (hiredRecruiter) ActivateAgent(recruiterAgentObj, recruiterWorkstation, recruiterWaypoints);
        else DeactivateAgent(recruiterAgentObj, recruiterWorkstation);

        // PM
        if (hiredPm) ActivateAgent(pmAgentObj, pmWorkstation, pmWaypoints);
        else DeactivateAgent(pmAgentObj, pmWorkstation);

        // Sales Executive
        if (hiredSales) ActivateAgent(salesAgentObj, salesWorkstation, salesWaypoints);
        else DeactivateAgent(salesAgentObj, salesWorkstation);

        // Community Manager
        if (hiredCommunity) ActivateAgent(communityAgentObj, communityWorkstation, communityWaypoints);
        else DeactivateAgent(communityAgentObj, communityWorkstation);

        UpdateButtonsState();
    }
}
