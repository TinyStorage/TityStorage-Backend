namespace Itmo.TinyStorage.Infrastructure;

public class TinyStorageContext(DbContextOptions<TinyStorageContext> options, IUserAccessor user) : DbContext(options)
{
    private const string SchemaName = "tiny_storage";

    private IDbContextTransaction? _currentTransaction;

    public DbSet<ItemModel> Items { get; set; } = null!;
    public DbSet<ItemAuditModel> ItemAudits { get; set; } = null!;

    public bool HasActiveTransaction => _currentTransaction != null;

    public IDbContextTransaction? GetCurrentTransaction() => _currentTransaction;

    public IExecutionStrategy CreateExecutionStrategy() => Database.CreateExecutionStrategy();

    public async Task<IDbContextTransaction?> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
        {
            return null;
        }

        _currentTransaction = await Database
            .BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken)
            .ConfigureAwait(false);

        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        if (transaction != _currentTransaction)
        {
            throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");
        }

        try
        {
            await SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder
            .HasDefaultSchema(SchemaName)
            .ApplyConfigurationsFromAssembly(typeof(TinyStorageContext).Assembly);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSnakeCaseNamingConvention()
            .AddInterceptors(new UpdateAuditableInterceptor(user));

    private void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
