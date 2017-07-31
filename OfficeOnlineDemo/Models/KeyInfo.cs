namespace OfficeOnlineDemo.Models
{
    public struct KeyInfo
    {
        private string _cspBlob;
        private string _exponent;
        private string _modulus;

        public KeyInfo(string cspBlob, string modulus, string exponent)
        {
            _cspBlob = cspBlob;
            _exponent = exponent;
            _modulus = modulus;
        }

        public string CspBlob { get { return _cspBlob; } }
        public string Exponent { get { return _exponent; } }
        public string Modulus { get { return _modulus; } }
    }
}