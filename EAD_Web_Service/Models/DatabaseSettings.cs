namespace EAD_Web_Service.Models
{
    public class DatabaseSettings
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
        public string? UsersCollectionName { get; set; }
        public string? VendorsCollectionName { get; set; }
        public string? CategoriesCollectionName { get; set; }
        public string? ProductsCollectionName { get; set; }
        public string? CartsCollectionName { get; set; }
        public string? OrdersCollectionName { get; set; }
    }
}
