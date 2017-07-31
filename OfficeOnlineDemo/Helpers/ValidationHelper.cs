namespace OfficeOnlineDemo.Helpers
{
    public class ValidationHelper
    {
        /// <summary>
        /// Validate that the provided access token is valid to get access to requested resource.
        /// </summary>
        /// <param name="requestData">Request information, including requested file Id</param>
        /// <param name="writeAccessRequired">Whether write permission is requested or not.</param>
        /// <returns>true when access token is correct and user has access to document, false otherwise.</returns>
        public static bool ValidateAccess(string accessToken, bool writeAccessRequired)
        {
            // TODO: Access token validation is not implemented in this sample.
            // For more details on access tokens, see the documentation
            // https://wopi.readthedocs.io/projects/wopirest/en/latest/concepts.html#term-access-token
            return !string.IsNullOrWhiteSpace(accessToken);
        }
    }
}