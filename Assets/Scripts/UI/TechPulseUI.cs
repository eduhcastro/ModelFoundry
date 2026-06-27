using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Controls the TechPulse (X-clone) social media UI with an identical X.com dark-mode profile page layout.
/// </summary>
public sealed class TechPulseUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup mainPanel;
    [SerializeField] private RectTransform scrollContent;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject postPrefab;

    [Header("Close Action")]
    [SerializeField] private Button closeButton;
    
    [Header("X.com Profile UI Header Elements")]
    [SerializeField] private TextMeshProUGUI profileCompanyNameText;
    [SerializeField] private TextMeshProUGUI profileCompanyHandleText;
    [SerializeField] private TextMeshProUGUI profileCompanyBioText;
    [SerializeField] private TextMeshProUGUI profileFollowersText;
    [SerializeField] private TextMeshProUGUI profileFollowingText;
    [SerializeField] private TextMeshProUGUI profilePostsCountText;

    [Header("State")]
    private bool isOpen;
    private List<GameObject> activePostUI = new List<GameObject>();
    private Dictionary<string, Sprite> rivalLogos = new Dictionary<string, Sprite>();

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
            
        if (mainPanel != null)
        {
            mainPanel.alpha = 0f;
            mainPanel.interactable = false;
            mainPanel.blocksRaycasts = false;
            var rt = mainPanel.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = new Vector2(420f, -30f); // Initialize offscreen right (420 width)
            }
        }

        LoadRivalLogos();
    }

    private void LoadRivalLogos()
    {
        string baseDir = System.IO.Path.Combine(Application.dataPath, "../MyAssets/Rivals");
        rivalLogos["NeuraCorp"] = LoadSprite(System.IO.Path.Combine(baseDir, "openai.png"));
        rivalLogos["AnthroTech"] = LoadSprite(System.IO.Path.Combine(baseDir, "anthropic.png"));
        rivalLogos["Quantum Minds"] = LoadSprite(System.IO.Path.Combine(baseDir, "grok.png"));
    }

    private Sprite LoadSprite(string path)
    {
        if (System.IO.File.Exists(path))
        {
            try
            {
                byte[] data = System.IO.File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2);
                if (tex.LoadImage(data))
                {
                    return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading logo from {path}: {e.Message}");
            }
        }
        return null;
    }


    private void Start()
    {
        if (TechPulseFeed.Instance != null)
        {
            TechPulseFeed.Instance.OnNewPost += HandleNewPost;
            // Load existing
            foreach (var post in TechPulseFeed.Instance.Posts)
            {
                CreatePostUI(post);
            }
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += UpdateProfile;
        }
        UpdateProfile();
    }

    private void OnDestroy()
    {
        if (TechPulseFeed.Instance != null)
        {
            TechPulseFeed.Instance.OnNewPost -= HandleNewPost;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= UpdateProfile;
        }
    }

    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }

    public void Open()
    {
        if (isOpen || mainPanel == null) return;
        isOpen = true;
        UpdateProfile();
        UIAnimations.FadeCanvasGroup(this, mainPanel, 0f, 1f, GameDesignConstants.AnimFast);
        
        // Slide in from right
        var rt = mainPanel.GetComponent<RectTransform>();
        if (rt != null)
        {
            var targetPos = new Vector2(0f, -30f); // Target pos keeps the Y offset
            rt.anchoredPosition = new Vector2(420f, -30f); // Start offscreen
            UIAnimations.SlideRectTransform(this, rt, rt.anchoredPosition, targetPos, GameDesignConstants.AnimFast);
        }
    }

    public void Close()
    {
        if (!isOpen || mainPanel == null) return;
        isOpen = false;
        
        var rt = mainPanel.GetComponent<RectTransform>();
        if (rt != null)
        {
            var targetPos = new Vector2(420f, -30f); // Slide back to offscreen
            UIAnimations.SlideRectTransform(this, rt, rt.anchoredPosition, targetPos, GameDesignConstants.AnimFast);
        }
        
        UIAnimations.FadeCanvasGroup(this, mainPanel, 1f, 0f, GameDesignConstants.AnimFast);
    }

    private void HandleNewPost(TechPulsePost post)
    {
        // Add to top of list
        CreatePostUI(post, true);
        UpdateProfile();
    }

    public void UpdateProfile()
    {
        if (GameManager.Instance == null) return;
        
        string compName = GameManager.Instance.CompanyName;
        string handle = "@" + compName.Replace(" ", "").ToLower();
        
        if (profileCompanyNameText != null) profileCompanyNameText.text = compName;
        CompanyIdentityCatalog.ApplyToCompanyText(profileCompanyNameText, GameManager.Instance.CompanyFontKey, GameManager.Instance.CompanyColor);
        if (profileCompanyHandleText != null) profileCompanyHandleText.text = handle;
        if (profileCompanyBioText != null) profileCompanyBioText.text = LocalizationManager.T("techpulse.bio");
        
        // Use the actual Followers and Following fields from GameManager
        int followersVal = GameManager.Instance.Followers;
        int followingVal = GameManager.Instance.Following;

        string formattedFollowers = followersVal >= 1000000 ? $"{followersVal / 1000000f:F1}M" :
                                    followersVal >= 1000 ? $"{followersVal / 1000f:F1}K" : followersVal.ToString();

        string formattedFollowing = followingVal >= 1000000 ? $"{followingVal / 1000000f:F1}M" :
                                    followingVal >= 1000 ? $"{followingVal / 1000f:F1}K" : followingVal.ToString();
                                    
        if (profileFollowersText != null) profileFollowersText.text = $"<b>{formattedFollowers}</b> <color=#71767B>Followers</color>";
        if (profileFollowingText != null) profileFollowingText.text = $"<b>{formattedFollowing}</b> <color=#71767B>Following</color>";
        
        int postsCount = TechPulseFeed.Instance != null ? TechPulseFeed.Instance.Posts.Count : 0;
        if (profilePostsCountText != null) profilePostsCountText.text = $"{postsCount} posts";
    }

    private void CreatePostUI(TechPulsePost post, bool atTop = false)
    {
        if (postPrefab == null || scrollContent == null) return;

        var go = Instantiate(postPrefab, scrollContent);
        go.SetActive(true);
        if (atTop)
        {
            go.transform.SetAsFirstSibling();
        }

        activePostUI.Add(go);
        if (activePostUI.Count > 50) // Keep UI clean
        {
            var old = activePostUI[0];
            activePostUI.RemoveAt(0);
            Destroy(old);
        }

        // Setup UI elements inside prefab (under PostContainer)
        var nameText = go.transform.Find("PostContainer/AuthorName")?.GetComponent<TextMeshProUGUI>();
        if (nameText != null) { nameText.text = post.AuthorName; nameText.color = post.AuthorColor; }

        var handleText = go.transform.Find("PostContainer/AuthorHandle")?.GetComponent<TextMeshProUGUI>();
        if (handleText != null) handleText.text = $"{post.AuthorHandle} • {post.Timestamp}";

        var contentText = go.transform.Find("PostContainer/Content")?.GetComponent<TextMeshProUGUI>();
        if (contentText != null) contentText.text = post.Content;

        var statsText = go.transform.Find("PostContainer/Stats")?.GetComponent<TextMeshProUGUI>();
        if (statsText != null)
        {
            // Highly-aesthetic X-style action stats
            int mockViews = post.Likes * UnityEngine.Random.Range(8, 25) + UnityEngine.Random.Range(5, 50);
            string formattedViews = mockViews >= 1000 ? $"{mockViews / 1000f:F1}K" : mockViews.ToString();
            
            statsText.text = $"💬 <color=#71767B>{post.Replies}</color>      🔁 <color=#71767B>{post.Reposts}</color>      ❤️ <color=#71767B>{post.Likes}</color>      📊 <color=#71767B>{formattedViews}</color>";
        }

        // Setup custom logo / avatar letter fallback
        var postAvatarObj = go.transform.Find("PostContainer/PostAvatar");
        var avatarSymbolObj = postAvatarObj?.Find("AvatarSymbol");

        if (postAvatarObj != null)
        {
            var avatarImage = postAvatarObj.GetComponent<Image>();
            
            // Check if this post is from a rival with a custom logo
            var gm = GameManager.Instance;
            string playerCompName = gm != null ? gm.CompanyName : "Model Foundry";
            var playerSprite = post.AuthorName == playerCompName && gm != null
                ? CompanyIdentityCatalog.LoadCompanyIcon(gm.CompanyIconKey)
                : null;

            if (playerSprite != null)
            {
                avatarImage.sprite = playerSprite;
                avatarImage.color = Color.white;
                if (avatarSymbolObj != null) avatarSymbolObj.gameObject.SetActive(false);
            }
            else if (rivalLogos.TryGetValue(post.AuthorName, out Sprite logoSprite) && logoSprite != null)
            {
                avatarImage.sprite = logoSprite;
                avatarImage.color = Color.white; // Ensure it shows in full color
                if (avatarSymbolObj != null) avatarSymbolObj.gameObject.SetActive(false); // Hide the ▲ symbol
            }
            else
            {
                // Fallback to default
                avatarImage.sprite = null;
                avatarImage.color = new Color(0.08f, 0.08f, 0.12f);
                if (avatarSymbolObj != null)
                {
                    avatarSymbolObj.gameObject.SetActive(true);
                    var symbolText = avatarSymbolObj.GetComponent<TextMeshProUGUI>();
                    if (symbolText != null)
                    {
                        if (post.AuthorName == playerCompName)
                        {
                            symbolText.text = !string.IsNullOrEmpty(playerCompName) ? playerCompName.Substring(0, 1).ToUpper() : "▲";
                            symbolText.color = gm != null ? gm.CompanyColor : GameDesignConstants.BrandPrimary;
                        }
                        else
                        {
                            symbolText.text = !string.IsNullOrEmpty(post.AuthorName) ? post.AuthorName.Substring(0, 1).ToUpper() : "▲";
                            symbolText.color = post.AuthorColor;
                        }
                    }
                }
            }
        }
    }
}
