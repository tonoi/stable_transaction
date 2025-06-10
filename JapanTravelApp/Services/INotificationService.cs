using Plugin.LocalNotification;

namespace JapanTravelApp.Services;

public class AlarmModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime ScheduledTime { get; set; }
    public bool IsActive { get; set; }
    public bool IsRepeating { get; set; }
}

public interface INotificationService
{
    Task<bool> RequestPermissionAsync();
    Task<bool> ScheduleAlarmAsync(AlarmModel alarm);
    Task<bool> CancelAlarmAsync(int alarmId);
    Task<List<AlarmModel>> GetActiveAlarmsAsync();
    Task<bool> SendImmediateNotificationAsync(string title, string message);
}

public class NotificationService : INotificationService
{
    private readonly List<AlarmModel> _alarms = new();
    private int _nextAlarmId = 1;

    public Task<bool> RequestPermissionAsync()
    {
        try
        {
            // Request notification permission
            return Task.Run(() => LocalNotificationCenter.Current.RequestNotificationPermission());
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<bool> ScheduleAlarmAsync(AlarmModel alarm)
    {
        await Task.Delay(200);
        try
        {
            alarm.Id = _nextAlarmId++;
            alarm.IsActive = true;
            var notification = new NotificationRequest
            {
                NotificationId = alarm.Id,
                Title = alarm.Title,
                Description = alarm.Message,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = alarm.ScheduledTime,
                    RepeatType = alarm.IsRepeating ? NotificationRepeat.Daily : NotificationRepeat.No
                }
            };
            // Simplified for build
            LocalNotificationCenter.Current.Show(notification);
            _alarms.Add(alarm);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CancelAlarmAsync(int alarmId)
    {
        await Task.Delay(100);
        try
        {
            // Simplified for build
            LocalNotificationCenter.Current.Cancel(alarmId);
            var alarm = _alarms.FirstOrDefault(a => a.Id == alarmId);
            if (alarm != null)
            {
                alarm.IsActive = false;
                _alarms.Remove(alarm);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<AlarmModel>> GetActiveAlarmsAsync()
    {
        await Task.Delay(100);
        return _alarms.Where(a => a.IsActive).ToList();
    }

    public async Task<bool> SendImmediateNotificationAsync(string title, string message)
    {
        await Task.Delay(100);
        try
        {
            var notification = new NotificationRequest
            {
                NotificationId = _nextAlarmId++,
                Title = title,
                Description = message,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1)
                }
            };
            // Simplified for build
            LocalNotificationCenter.Current.Show(notification);
            return true;
        }
        catch
        {
            return false;
        }
    }
}