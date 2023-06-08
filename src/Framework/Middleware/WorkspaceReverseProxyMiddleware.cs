using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Data.Models;
using Framework.Services;
using Microsoft.AspNetCore.Http;

namespace Framework.Middleware
{
    public class WorkspaceReverseProxyMiddleware
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly RequestDelegate _nextMiddleware;

        public WorkspaceReverseProxyMiddleware(
            RequestDelegate nextMiddleware
        )
        {
            _nextMiddleware = nextMiddleware;
        }

        public async Task Invoke(HttpContext context, IWorkspaceService _workspaceService)
        {
            // check if request is for workspace
            if (!(context.WebSockets.IsWebSocketRequest ||
                (context.Request.Path.Value?.StartsWith("/code", StringComparison.OrdinalIgnoreCase) ?? false) ||
                (context.Request.Path.Value?.StartsWith("/stable", StringComparison.OrdinalIgnoreCase) ?? false) ||
                context.Request.Headers.TryGetValue("Referer", out var referer) && new Uri(referer.ToString()).AbsolutePath.StartsWith("/code", StringComparison.OrdinalIgnoreCase)
            ))
            {
                await _nextMiddleware(context);
                return;
            }

            // get cookie value
            var workspaceId = context.Request.Cookies[Consts.WORKSPACE_ID_COOKIE_NAME];
            if (workspaceId == null)
            {
                // redirect to error page
                context.Response.Redirect("/Home/Error");
                return;
            }

            // get target base
            var workspaceIpPort = await _workspaceService.GetWorkspaceUrlAsync(workspaceId);

            // DEBUGGING ONLY
            // workspaceIpPort = "http://localhost:3031";

            var httpTargetBase = workspaceIpPort;
            var host = workspaceIpPort.Split("://")[1];
            var wsTargetBase = $"ws://{host}";

            // Websocket proxy
            if (context.WebSockets.IsWebSocketRequest)
            {
                var wsTargetUri = new Uri(wsTargetBase + context.Request.Path + context.Request.QueryString);
                using (var client = new ClientWebSocket())
                {
                    await client.ConnectAsync(wsTargetUri, context.RequestAborted);
                    using (var server = await context.WebSockets.AcceptWebSocketAsync())
                    {
                        await Task.WhenAll(ProxyWebSocket(client, server, context.RequestAborted), ProxyWebSocket(server, client, context.RequestAborted));
                    }
                }
            }

            var targetUri = BuildTargetUri(context.Request, httpTargetBase);

            if (targetUri != null)
            {
                var targetRequestMessage = CreateTargetMessage(context, targetUri);

                using (var responseMessage = await _httpClient.SendAsync(targetRequestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
                {
                    context.Response.StatusCode = (int)responseMessage.StatusCode;
                    CopyFromTargetResponseHeaders(context, responseMessage);
                    await responseMessage.Content.CopyToAsync(context.Response.Body);
                }
                return;
            }
            await _nextMiddleware(context);
        }

        private HttpRequestMessage CreateTargetMessage(HttpContext context, Uri targetUri)
        {
            var requestMessage = new HttpRequestMessage();
            CopyFromOriginalRequestContentAndHeaders(context, requestMessage);

            requestMessage.RequestUri = targetUri;
            requestMessage.Headers.Host = targetUri.Host;
            requestMessage.Method = GetMethod(context.Request.Method);

            return requestMessage;
        }

        private void CopyFromOriginalRequestContentAndHeaders(HttpContext context, HttpRequestMessage requestMessage)
        {
            var requestMethod = context.Request.Method;

            if (!HttpMethods.IsGet(requestMethod) &&
              !HttpMethods.IsHead(requestMethod) &&
              !HttpMethods.IsDelete(requestMethod) &&
              !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(context.Request.Body);
                requestMessage.Content = streamContent;
            }

            foreach (var header in context.Request.Headers)
            {
                requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }

        private void CopyFromTargetResponseHeaders(HttpContext context, HttpResponseMessage responseMessage)
        {
            foreach (var header in responseMessage.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
            context.Response.Headers.Remove("transfer-encoding");
        }
        private static HttpMethod GetMethod(string method)
        {
            if (HttpMethods.IsDelete(method)) return HttpMethod.Delete;
            if (HttpMethods.IsGet(method)) return HttpMethod.Get;
            if (HttpMethods.IsHead(method)) return HttpMethod.Head;
            if (HttpMethods.IsOptions(method)) return HttpMethod.Options;
            if (HttpMethods.IsPost(method)) return HttpMethod.Post;
            if (HttpMethods.IsPut(method)) return HttpMethod.Put;
            if (HttpMethods.IsTrace(method)) return HttpMethod.Trace;
            return new HttpMethod(method);
        }

        private Uri? BuildTargetUri(HttpRequest request, string targetUri)
        {
            if (request.Path.StartsWithSegments("/code", out var remainingPath))
                return new Uri(targetUri + remainingPath);

            if (request.Path.Value?.StartsWith("/stable") ?? false)
                return new Uri(targetUri + request.Path + request.QueryString);

            // check referer
            if (request.Headers.TryGetValue("Referer", out var referer))
            {
                var refererUri = new Uri(referer.ToString());
                if (refererUri.AbsolutePath.StartsWith("/code", StringComparison.OrdinalIgnoreCase))
                {
                    return new Uri(targetUri + request.Path + request.QueryString);
                }
            }

            return null;
        }

        private static async Task ProxyWebSocket(WebSocket source, WebSocket destination, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 4];
            while (true)
            {
                var result = await source.ReceiveAsync(buffer, cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await destination.CloseOutputAsync(source.CloseStatus!.Value, source.CloseStatusDescription, cancellationToken);
                    return;
                }
                await destination.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, cancellationToken);
            }
        }
    }
}