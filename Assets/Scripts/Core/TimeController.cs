using UnityEngine;
using System;

/// <summary>
/// Controls the flow of in-game time. Starting date: January 1, 2017.
/// Speed modes: Paused, Normal (1 day = 4s), Fast (2s), Ultra (1s).
/// Fires events so other systems react to time passing.
/// </summary>
public sealed class TimeController : MonoBehaviour
{
    public static TimeController Instance { get; private set; }

    public enum Speed
    {
        Paused,
        Normal,
        Fast,
        Ultra
    }

    [Header("Configuration")]
    [SerializeField] private float normalDayDuration = 4f;
    [SerializeField] private float fastDayDuration   = 2f;
    [SerializeField] private float ultraDayDuration   = 1f;

    // ── State ────────────────────────────────────────────────────────
    public int   Year       { get; private set; } = 2017;
    public int   Month      { get; private set; } = 1;
    public int   Day        { get; private set; } = 1;
    public int   TotalDays  { get; private set; }
    public Speed CurrentSpeed { get; private set; } = Speed.Normal;

    // ── Events ───────────────────────────────────────────────────────
    public event Action        OnDayPassed;
    public event Action        OnWeekPassed;
    public event Action<int>   OnMonthPassed;
    public event Action<Speed> OnSpeedChanged;

    private float dayTimer;

    private static readonly string[] MonthNames =
    {
        "", "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (CurrentSpeed == Speed.Paused)
        {
            return;
        }

        dayTimer += Time.deltaTime;
        var dayDuration = GetDayDuration();

        if (dayTimer < dayDuration)
        {
            return;
        }

        dayTimer -= dayDuration;
        AdvanceDay();
    }

    // ── Speed controls ───────────────────────────────────────────────

    public void SetSpeed(Speed speed)
    {
        CurrentSpeed = speed;
        OnSpeedChanged?.Invoke(speed);
    }

    public void TogglePause()
    {
        if (CurrentSpeed == Speed.Paused)
        {
            SetSpeed(Speed.Normal);
        }
        else
        {
            SetSpeed(Speed.Paused);
        }
    }

    public void CycleSpeed()
    {
        switch (CurrentSpeed)
        {
            case Speed.Paused:
                SetSpeed(Speed.Normal);
                break;
            case Speed.Normal:
                SetSpeed(Speed.Fast);
                break;
            case Speed.Fast:
                SetSpeed(Speed.Ultra);
                break;
            case Speed.Ultra:
                SetSpeed(Speed.Normal);
                break;
        }
    }

    // ── Formatted output ─────────────────────────────────────────────

    public string FormattedDate => $"{MonthNames[Month]} {Day}, {Year}";

    public string FormattedMonthYear => $"{MonthNames[Month]} {Year}";

    public string SpeedLabel
    {
        get
        {
            switch (CurrentSpeed)
            {
                case Speed.Paused: return "║║";
                case Speed.Normal: return "▶";
                case Speed.Fast:   return "▶▶";
                case Speed.Ultra:  return "▶▶▶";
                default:           return "▶";
            }
        }
    }

    /// <summary>Progress through the current day, 0-1.</summary>
    public float DayProgress => Mathf.Clamp01(dayTimer / GetDayDuration());

    // ── Internal ─────────────────────────────────────────────────────

    private float GetDayDuration()
    {
        switch (CurrentSpeed)
        {
            case Speed.Normal: return normalDayDuration;
            case Speed.Fast:   return fastDayDuration;
            case Speed.Ultra:  return ultraDayDuration;
            default:           return normalDayDuration;
        }
    }

    private int DaysInMonth(int month, int year)
    {
        switch (month)
        {
            case 2:  return (year % 4 == 0) ? 29 : 28;
            case 4:  case 6: case 9: case 11: return 30;
            default: return 31;
        }
    }

    private void AdvanceDay()
    {
        Day++;
        TotalDays++;
        OnDayPassed?.Invoke();

        if (TotalDays % 7 == 0)
        {
            OnWeekPassed?.Invoke();
        }

        if (Day > DaysInMonth(Month, Year))
        {
            Day = 1;
            Month++;

            if (Month > 12)
            {
                Month = 1;
                Year++;
            }

            OnMonthPassed?.Invoke(Month);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ApplyMonthlyBurn();
            }
        }
    }

    public void LoadDateTime(int year, int month, int day, int totalDays)
    {
        Year = year;
        Month = month;
        Day = day;
        TotalDays = totalDays;
        dayTimer = 0f;
        OnDayPassed?.Invoke();
    }
}
