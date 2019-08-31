using System;
using System.Collections.Concurrent;

namespace DaOAuthV2.Gui.Front.Tools
{
    internal static class ClientAuthorizationStack
    {
        private static readonly ConcurrentDictionary<Guid, ClientRedirectInfo> _clients = new ConcurrentDictionary<Guid, ClientRedirectInfo>();

        internal static Guid Add(ClientRedirectInfo c)
        {
            var g = Guid.NewGuid();
            _clients.TryAdd(g, c);
            return g;
        }

        internal static ClientRedirectInfo Get(Guid g)
        {
            return _clients.TryGetValue(g, out var c) ? c : null;
        }

        internal static void Delete(Guid g)
        {
            _clients.TryRemove(g, out var c);
        }
    }

    internal class ClientRedirectInfo
    {
        public ClientRedirectInfo(string responseType, Uri redirectUri, string scope, string state, string clientPublicId)
        {
            ResponseType = responseType;
            RedirectUri = redirectUri;
            Scope = scope;
            State = state;
            ClientPublicId = clientPublicId;
        }

        internal readonly string ResponseType;
        internal readonly Uri RedirectUri;
        internal readonly string Scope;
        internal readonly string State;
        internal readonly string ClientPublicId;
    }
}
