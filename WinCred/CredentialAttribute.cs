namespace Advapi32.WinCred
{
    public class CredentialAttribute
    {
        public string Keyword { get; set; }
        public uint Flags { get; set; }
        public byte[] Value { get; set; }

        public override string ToString()
            => $"{nameof(CredentialAttribute)} {{"
                + $"{nameof(Keyword)}: {Keyword}"
                + $", {nameof(Flags)}: {Flags}"
                + $", {nameof(Value)}: [{(Value != null ? string.Join(" ", Value) : "")}"
                + $"}}";
    }
}
