using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Sail.Kubernetes.Protocol;

namespace Sail.Kubernetes.Controller.Dispatching;

public class DispatchActionResult : IActionResult, IDispatchTarget
{

    private static readonly byte[] _newline = Encoding.UTF8.GetBytes(Environment.NewLine);

    private readonly IDispatcher _dispatcher;
    private Task _task = Task.CompletedTask;
    private readonly object _taskSync = new();
    private HttpContext _httpContext;

    public DispatchActionResult(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var cancellationToken = context.HttpContext.RequestAborted;
        _httpContext = context.HttpContext;
        _httpContext.Response.ContentType = "text/plain";
        _httpContext.Response.Headers["Connection"] = "close";
        await _httpContext.Response.Body.FlushAsync(cancellationToken).ConfigureAwait(false);
        
        _dispatcher.Attach(this);

        try
        {
            var utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(new Message
            {
                MessageType = MessageType.Heartbeat
            });

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(35), cancellationToken).ConfigureAwait(false);
                await SendAsync(utf8Bytes, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (TaskCanceledException)
        {
        }
        finally
        {
            _dispatcher.Detach(this);
        }
    }

    public async Task SendAsync(byte[] utf8Bytes, CancellationToken cancellationToken)
    {
        var result = Task.CompletedTask;
        lock (_taskSync)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_task.IsCanceled || _task.IsFaulted)
            {
                result = _task;
            }
            else
            {
                _task = DoSendAsync(_task, utf8Bytes);
            }

            async Task DoSendAsync(Task task, byte[] bytes)
            {
                await task.ConfigureAwait(false);
                await _httpContext.Response.BodyWriter.WriteAsync(bytes, cancellationToken);
                await _httpContext.Response.BodyWriter.WriteAsync(_newline, cancellationToken);
                await _httpContext.Response.BodyWriter.FlushAsync(cancellationToken);
            }
        }

        await result.ConfigureAwait(false);
    }

    public override string ToString()
    {
        return $"{_httpContext?.Connection.Id}:{_httpContext?.TraceIdentifier}";
    }
}