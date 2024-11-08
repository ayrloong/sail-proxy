namespace Sail.Kubernetes.Controller.Queues;


public class WorkQueue<TItem> : IWorkQueue<TItem>
{
    private readonly object _sync = new();
    private readonly Dictionary<TItem, object> _dirty = new();
    private readonly Dictionary<TItem, object> _processing = new();
    private readonly Queue<TItem> _queue = new();
    private readonly SemaphoreSlim _semaphore = new(0);
    private readonly CancellationTokenSource _shuttingDown = new();
    private bool _disposedValue;


    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }


    public void Add(TItem item)
    {
        lock (_sync)
        {
            if (_shuttingDown.IsCancellationRequested)
            {
                return;
            }

            if (_dirty.ContainsKey(item))
            {
                return;
            }

            _dirty.Add(item, null);
            if (_processing.ContainsKey(item))
            {
                return;
            }

            _queue.Enqueue(item);
            _semaphore.Release();
        }
    }
    
    public async Task<(TItem item, bool shutdown)> GetAsync(CancellationToken cancellationToken)
    {
        using (var linkedTokenSource =
               CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _shuttingDown.Token))
        {
            try
            {
                await _semaphore.WaitAsync(linkedTokenSource.Token);

                await OnGetAsync(linkedTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                if (_shuttingDown.IsCancellationRequested)
                {
                    return (default, true);
                }

                throw;
            }
        }

        lock (_sync)
        {
            if (_queue.Count == 0 || _shuttingDown.IsCancellationRequested)
            {
                _semaphore.Release();
                return (default, true);
            }

            var item = _queue.Dequeue();

            _processing.Add(item, null);
            _dirty.Remove(item);

            return (item, false);
        }
    }

    public void Done(TItem item)
    {
        lock (_sync)
        {
            _processing.Remove(item);
            if (_dirty.ContainsKey(item))
            {
                _queue.Enqueue(item);
                _semaphore.Release();
            }
        }
    }


    public int Len()
    {
        lock (_sync)
        {
            return _queue.Count;
        }
    }


    public void ShutDown()
    {
        lock (_sync)
        {
            _shuttingDown.Cancel();
            _semaphore.Release();
        }
    }


    public bool ShuttingDown()
    {
        return _shuttingDown.IsCancellationRequested;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _semaphore.Dispose();
            }

            _disposedValue = true;
        }
    }


    protected virtual Task OnGetAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}