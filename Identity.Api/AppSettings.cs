namespace Identity.Api
{
    public class AppSettings
    {
        public string MvcClient { get; set; }

        public bool UseCustomizationData { get; set; }
    }

    public class CatalogSettings
    {
        public string EventBusConnection { get; set; }
    }
}
