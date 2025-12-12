#if (IL2CPPMELON)
using S1GameTime = Il2CppScheduleOne.GameTime;
using S1TimeManager = Il2CppScheduleOne.GameTime.TimeManager;
using S1DevUtilities = Il2CppScheduleOne.DevUtilities;
#elif (MONOMELON || MONOBEPINEX || IL2CPPBEPINEX)
using S1GameTime = ScheduleOne.GameTime;
using S1DevUtilities = ScheduleOne.DevUtilities;
#endif

using System;

namespace S1API.GameTime
{
    /// <summary>
    /// Provides access to various time management functions in the game.
    /// </summary>
    public static class TimeManager
    {
        /// <summary>
        /// Called when a new in-game day starts.
        /// </summary>
        public static Action OnDayPass = new Action(() => { });

        /// <summary>
        /// Called when a new in-game week starts.
        /// </summary>
        public static Action OnWeekPass = new Action(() => {});

        /// <summary>
        /// Called when the player starts sleeping.
        /// </summary>
        public static Action OnSleepStart = new Action(() => {});

        /// <summary>
        /// Called when the player finishes sleeping.
        /// Parameter: total minutes skipped during sleep.
        /// </summary>
        public static Action<int> OnSleepEnd = new Action<int>(minutes => {});


        /// <summary>
        /// Called at every tick of gametime.
        /// </summary>
        public static Action OnTick = new Action(() => {});

        private static int _lastSleepSkippedMinutes;

        static TimeManager()
        {
            if (_s1Instance != null)
            {
                Setup(_s1Instance);
            }
        }



        /// <summary>
        /// The current in-game day (Monday, Tuesday, etc.).
        /// </summary>
        public static Day CurrentDay => (Day)_s1Instance.CurrentDay;

        /// <summary>
        /// The number of in-game days elapsed.
        /// </summary>
        public static int ElapsedDays => _s1Instance.ElapsedDays;

        /// <summary>
        /// The current 24-hour time (e.g., 1330 for 1:30 PM).
        /// </summary>
        public static int CurrentTime => _s1Instance.CurrentTime;

        /// <summary>
        /// Whether it is currently nighttime in-game.
        /// </summary>
        public static bool IsNight => _s1Instance.IsNight;

        /// <summary>
        /// Whether the game is currently at the end of the day (4:00 AM).
        /// </summary>
        public static bool IsEndOfDay => _s1Instance.IsEndOfDay;

        /// <summary>
        /// Whether the player is currently sleeping.
        /// </summary>
        public static bool SleepInProgress => _s1Instance.SleepInProgress;

        /// <summary>
        /// Whether the time is currently overridden (frozen or custom).
        /// </summary>
        public static bool TimeOverridden => _s1Instance.TimeOverridden;

        /// <summary>
        /// The current normalized time of day (0.0 = start, 1.0 = end).
        /// </summary>
        public static float NormalizedTime => _s1Instance.NormalizedTime;

        /// <summary>
        /// Total playtime (in seconds).
        /// </summary>
        public static float Playtime => _s1Instance.Playtime;

        /// <summary>
        /// Fast-forwards time to morning wake time (7:00 AM).
        /// </summary>
        public static void FastForwardToWakeTime() => _s1Instance.FastForwardToWakeTime();

        /// <summary>
        /// Sets the current time manually.
        /// </summary>
        public static void SetTime(int time24h, bool local = false) => _s1Instance.SetTime(time24h, local);

        /// <summary>
        /// Sets the number of elapsed in-game days.
        /// </summary>
        public static void SetElapsedDays(int days) => _s1Instance.SetElapsedDays(days);

        /// <summary>
        /// Gets the current time formatted in 12-hour AM/PM format.
        /// </summary>
        public static string GetFormatted12HourTime()
        {
            return S1GameTime.TimeManager.Get12HourTime(CurrentTime, true);
        }

        /// <summary>
        /// Returns true if the current time is within the specified 24-hour range.
        /// </summary>
        public static bool IsCurrentTimeWithinRange(int startTime24h, int endTime24h)
        {
            return _s1Instance.IsCurrentTimeWithinRange(startTime24h, endTime24h);
        }

        /// <summary>
        /// Converts 24-hour time to total minutes.
        /// </summary>
        public static int GetMinutesFrom24HourTime(int time24h)
        {

            return S1GameTime.TimeManager.GetMinSumFrom24HourTime(time24h);
        }

        /// <summary>
        /// Converts total minutes into 24-hour time format.
        /// </summary>
        public static int Get24HourTimeFromMinutes(int minutes)
        {
            return S1GameTime.TimeManager.Get24HourTimeFromMinSum(minutes);
        }

        #region Internal

        static internal S1GameTime.TimeManager? _lastTimeManager = null;

        static internal void Setup(S1GameTime.TimeManager manager) {

            if (_lastTimeManager != null) {
                //Possibly unsubscribe from the old manager, would require caching
            }

            manager.onDayPass += new Action(() => OnDayPass());
            manager.onWeekPass += new Action(() => OnWeekPass());
            manager.onTick += new Action(() => OnTick());
            manager.onHourPass += new Action(() => OnHourPass());
            manager.onMinutePass += new Action(() => OnMinutePass());
            manager.onSleepStart += new Action(() => OnSleepStart());
            manager.onTimeSkip += new Action<int>(minutes => _lastSleepSkippedMinutes = minutes);
            manager.onSleepEnd += new Action(() => OnSleepEnd(_lastSleepSkippedMinutes));
        }

        static internal S1GameTime.TimeManager _s1Instance => S1DevUtilities.NetworkSingleton<S1GameTime.TimeManager>.Instance;

        #endregion
    }
}
