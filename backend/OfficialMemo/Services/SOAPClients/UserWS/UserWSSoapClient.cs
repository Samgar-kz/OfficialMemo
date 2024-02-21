namespace UserWS;
public partial class UserWSSoapClient
{
    public static string ServiceUrl { get; set; } = string.Empty;
    static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials)
    {
        serviceEndpoint.Address = new System.ServiceModel.EndpointAddress(ServiceUrl);
        //clientCredentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;
    }
}
