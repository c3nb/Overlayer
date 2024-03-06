namespace Overlayer.Core.TextReplacing.Lexing
{
    public class Token
    {
        public TokenType type;
        public string value;
        public bool afterInvalid;
        public bool IsEmptyToken { get; }
        public Token(TokenType type, string value)
        {
            this.type = type;
            this.value = value;
            IsEmptyToken = string.IsNullOrEmpty(value);
        }
    }
}
