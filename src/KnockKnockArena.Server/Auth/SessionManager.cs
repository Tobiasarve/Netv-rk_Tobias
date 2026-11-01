using System;
using System.Collections.Generic;
using System.Linq;

namespace KnockKnockArena.Server.Auth;

public sealed class PlayerSession
{
    public byte PlayerId { get; }
    public string Username { get; }

    /// <summary>
    /// Opaque 16-byte token. The client echoes it in the UDP join packet (M05),
    /// which is how a UDP endpoint gets bound to a logged-in user.
    /// </summary>
    public byte[] Token { get; }

    public PlayerSession(byte playerId, string username, byte[] token)
    {
        PlayerId = playerId;
        Username = username;
        Token = token;
    }
}

/// <summary>
/// Owns the set of active sessions and hands out player ids (1..MaxPlayers; 0 means
/// "no player"). Thread-safe: called from one thread per connected client.
/// </summary>
public sealed class SessionManager
{
    private readonly object _gate = new();
    private readonly Dictionary<byte, PlayerSession> _sessionsById = new();
    private readonly int _maxPlayers;

    public SessionManager(int maxPlayers) => _maxPlayers = maxPlayers;

    public int Count
    {
        get { lock (_gate) return _sessionsById.Count; }
    }

    /// <summary>
    /// Creates a session for a validated user. Fails with a stable reason string
    /// (used in logs and in the LoginResponse error field) when the server is full
    /// or the user is already online.
    /// </summary>
    public bool TryCreate(string username, out PlayerSession? session, out string error)
    {
        lock (_gate)
        {
            // Case-insensitive, like the account lookup: "TEST" is the same account
            // as "test" and must not get a second session.
            if (_sessionsById.Values.Any(s => s.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                session = null;
                error = "already logged in";
                return false;
            }

            for (byte id = 1; id <= _maxPlayers; id++)
            {
                if (!_sessionsById.ContainsKey(id))
                {
                    session = new PlayerSession(id, username, Guid.NewGuid().ToByteArray());
                    _sessionsById[id] = session;
                    error = "";
                    return true;
                }
            }

            session = null;
            error = "server full";
            return false;
        }
    }

    public void Remove(byte playerId)
    {
        lock (_gate)
            _sessionsById.Remove(playerId);
    }

    /// <summary>Snapshot of all active sessions (for the login roster replay).</summary>
    public List<PlayerSession> GetAll()
    {
        lock (_gate)
            return new List<PlayerSession>(_sessionsById.Values);
    }
}