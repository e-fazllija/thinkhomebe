using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BackEnd.Interfaces;
using System.Collections.Concurrent;

namespace BackEnd.Services
{
    /// <summary>
    /// Centralized service for Azure Key Vault secret management
    /// Retrieves all secrets once during application startup to improve performance
    /// </summary>
    public class KeyVaultService : IKeyVaultService
    {
        public string DbConnectionString { get; private set; } = string.Empty;
        public string AuthKey { get; private set; } = string.Empty;
        public string StorageConnectionString { get; private set; } = string.Empty;
        public string MailPassword { get; private set; } = string.Empty;

        private readonly ILogger<KeyVaultService> _logger;

        public KeyVaultService(IConfiguration configuration, ILogger<KeyVaultService> logger)
        {
            _logger = logger;
            InitializeSecrets(configuration);
        }

        /// <summary>
        /// Initializes all secrets from Azure Key Vault in parallel for better performance
        /// </summary>
        /// <param name="configuration">Application configuration</param>
        private void InitializeSecrets(IConfiguration configuration)
        {
            try
            {
                _logger.LogInformation("Initializing Azure Key Vault secrets...");
                var startTime = DateTime.UtcNow;

                var keyVaultUrl = configuration.GetSection("KeyVault:Url").Value!;
                var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

                // Get all secret names from configuration
                var dbSecretName = configuration.GetSection("KeyVault:Secrets:DbConnectionString").Value!;
                var authSecretName = configuration.GetSection("KeyVault:Secrets:AuthKey").Value!;
                var storageSecretName = configuration.GetSection("KeyVault:Secrets:StorageConnectionString").Value!;
                var mailSecretName = configuration.GetSection("KeyVault:Secrets:MailPassword").Value!;

                // Retrieve all secrets in parallel for better performance
                var dbTask = client.GetSecretAsync(dbSecretName);
                var authTask = client.GetSecretAsync(authSecretName);
                var storageTask = client.GetSecretAsync(storageSecretName);
                var mailTask = client.GetSecretAsync(mailSecretName);

                // Wait for all tasks to complete
                Task.WaitAll(dbTask, authTask, storageTask, mailTask);

                // Assign values
                DbConnectionString = dbTask.Result.Value.Value;
                AuthKey = authTask.Result.Value.Value;
                StorageConnectionString = storageTask.Result.Value.Value;
                MailPassword = mailTask.Result.Value.Value;

                var duration = DateTime.UtcNow - startTime;
                _logger.LogInformation("Azure Key Vault secrets initialized successfully in {Duration}ms", duration.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Azure Key Vault secrets");
                throw new InvalidOperationException("Failed to retrieve secrets from Azure Key Vault", ex);
            }
        }
    }
}
