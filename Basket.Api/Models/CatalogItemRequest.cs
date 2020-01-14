namespace Basket.Api.Models
{
    public class CatalogItemRequest
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public decimal? Price { get; set; }

        public string PictureUri { get; set; }
    }
}