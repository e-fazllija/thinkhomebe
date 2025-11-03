namespace BackEnd.Interfaces
{
    /// <summary>
    /// Service interface for centralized Azure Key Vault secret management
    /// </summary>
    public interface IKeyVaultService
    {
        /// <summary>
        /// Database connection string retrieved from Key Vault
        /// </summary>
        string DbConnectionString { get; }

        /// <summary>
        /// JWT authentication key retrieved from Key Vault
        /// </summary>
        string AuthKey { get; }

        /// <summary>
        /// Azure Storage connection string retrieved from Key Vault
        /// </summary>
        string StorageConnectionString { get; }

        /// <summary>
        /// Mail service password retrieved from Key Vault
        /// </summary>
        string MailPassword { get; }
    }
}
