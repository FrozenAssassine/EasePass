using System;
using System.Threading;
using System.Threading.Tasks;

public class DeferredSaveHelper
{
    private TimeSpan _delay;
    public TimeSpan Delay => _delay;

    private CancellationTokenSource _cts;
    private readonly object _lock = new object();

    private Task _pendingSave;
    public bool SaveScheduled { get; private set; } = false;

    public DeferredSaveHelper(TimeSpan? delay = null)
    {
        _delay = delay ?? TimeSpan.FromMilliseconds(5000);
    }

    public void CancelPending()
    {
        lock (_lock)
        {
            _cts?.Cancel();
            SaveScheduled = false;
        }
    }

    public Task ForceSaveNow(Func<bool> saveFunc)
    {
        CancelPending();
        return Task.Run(saveFunc);
    }

    public Task RequestSaveAsync(Func<Task<bool>> saveFunc)
    {
        lock (_lock)
        {
            SaveScheduled = true;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            var token = _cts.Token;

            // Fire-and-forget task
            _pendingSave = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(_delay, token);
                    if (!token.IsCancellationRequested)
                    {
                        SaveScheduled = false;
                        await saveFunc();
                    }
                }
                catch (TaskCanceledException) { }
            });

            return _pendingSave;
        }
    }
}