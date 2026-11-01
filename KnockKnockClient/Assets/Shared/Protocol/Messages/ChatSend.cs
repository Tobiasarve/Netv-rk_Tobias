namespace KnockKnockArena.Shared.Protocol.Messages
{
    public sealed class ChatSend
    {
        public string Text = "";

        public byte[] ToPayload() => ProtocolSerialization.WritePayload(writer =>
            ProtocolSerialization.WriteString(writer, Text));

        public static ChatSend FromPayload(byte[] payload) => ProtocolSerialization.ReadPayload(payload,
            reader => new ChatSend { Text = ProtocolSerialization.ReadString(reader) });
    }
}