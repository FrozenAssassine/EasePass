using Microsoft.UI.Xaml.Documents;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace EasePass.Helper.Database;

public class DeferredSaveHelper
{
    private readonly TimeSpan _delay;
    private CancellationTokenSource _cts;
    private bool _saveScheduled = false;
    public bool SaveScheduled { get => _saveScheduled; }
    private readonly object _lock = new object(); //prevents race conditions

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

    public async Task ForceSaveNow(Func<bool> saveFunc)
    {
        CancelPending();
        await Task.Run(saveFunc);
    }
    public async Task<bool> RequestSaveAsync(Func<bool> saveFunc)
    {
        lock (_lock)
        {
            _saveScheduled = true;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
        }

        var token = _cts.Token;

        try
        {
            await Task.Delay(_delay, token);
            if (!token.IsCancellationRequested)
            {
                _saveScheduled = false;
                return await Task.Run(saveFunc); 
            }
        }
        catch (TaskCanceledException) { /*Expected exception!*/ }

        return false;
    }
}
