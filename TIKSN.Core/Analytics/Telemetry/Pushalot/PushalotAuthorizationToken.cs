using System;
using System.Linq;

namespace TIKSN.Analytics.Telemetry.Pushalot
{
    public class PushalotAuthorizationToken
    {
        private const string AUTHORIZATION_TOKEN_ALPHABET = "0123456789abcdef";
        private const int AUTHORIZATION_TOKEN_LENGTH = 32;

        public PushalotAuthorizationToken(string authorizationToken)
        {
            if (string.IsNullOrEmpty(authorizationToken))
            {
                throw new ArgumentException("Authorization token cannot be null or empty string.");
            }

            if (authorizationToken.Length != 32)
            {
                throw new ArgumentException(string.Format("Authorization token length must be {0}.",
                    AUTHORIZATION_TOKEN_LENGTH));
            }

            foreach (var authorizationTokenCharacter in authorizationToken)
            {
                if (!AUTHORIZATION_TOKEN_ALPHABET.ToCharArray()
                    .Any(AlphabetLetter => AlphabetLetter == authorizationTokenCharacter))
                {
                    throw new ArgumentException("Authorization token contains unsupported characters.");
                }
            }

            this.Token = authorizationToken;
        }

        public string Token { get; }

        public override string ToString() => this.Token;
    }
}
