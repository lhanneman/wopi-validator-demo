namespace OfficeOnlineDemo.Models
{
    public class ProofKeyValidationInput
    {
        private string _accessToken;
        private long _timestamp;
        private string _url;
        private string _proof;
        private string _oldProof;

        public ProofKeyValidationInput(string accessToken, long timestamp, string url, string proof, string oldProof)
        {
            _accessToken = accessToken;
            _timestamp = timestamp;
            _url = url;
            _proof = proof;
            _oldProof = oldProof;
        }

        public string AccessToken { get { return _accessToken; } }
        public long Timestamp { get { return _timestamp; } }
        public string Url { get { return _url; } }
        public string Proof { get { return _proof; } }
        public string OldProof { get { return _oldProof; } }
    }
}