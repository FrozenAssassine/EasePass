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
    private bool _saveScheduled;
    private bool _isSaving;
    public bool SaveScheduled
    {
        get { lock (_lock) return _saveScheduled || _isSaving; }
        private set { lock (_lock) _saveScheduled = value; }
    }

    public Task CurrentSaveTask
    {
        get { lock (_lock) return _pendingSave; }
    }

    public DeferredSaveHelper(TimeSpan? delay = null)
    {
        _delay = delay ?? TimeSpan.FromMilliseconds(5000);
    }

    public void CancelPending()
    {
        lock (_lock)
        {
            _cts?.Cancel();
            _saveScheduled = false;
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
                        lock (_lock)
                        {
                            if (token.IsCancellationRequested) return;
                            _saveScheduled = false;
                            _isSaving = true;
                        }
                        try
                        {
                            await saveFunc();
                        }
                        finally
                        {
                            lock (_lock) _isSaving = false;
                        }
                    }
                }
                catch (TaskCanceledException) { }
            });

            return _pendingSave;
        }
    }
}