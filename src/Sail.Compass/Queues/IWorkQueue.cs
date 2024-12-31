namespace Sail.Compass.Queues;

public interface IWorkQueue<TItem> : IDisposable
{
    /// <summary>
    /// Adds the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    void Add(TItem item);

    /// <summary>
    /// Returns number of items actively waiting in queue.
    /// </summary>
    /// <returns>System.Int32.</returns>
    int Len();

    /// <summary>
    /// Gets the next item in the queue once it is available.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.ValueTuple&lt;TItem, System.Boolean&gt;&gt;.</returns>
    Task<(TItem item, bool shutdown)> GetAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Called after <see cref="GetAsync"/> to inform the queue that the item
    /// processing is complete.
    /// </summary>
    /// <param name="item">The item.</param>
    void Done(TItem item);

    /// <summary>
    /// Shuts down.
    /// </summary>
    void ShutDown();

    /// <summary>
    /// Shutting down.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool ShuttingDown();
}