namespace TinyStorage.Application.Shared.UnitTests.Common;

public sealed class TinyStorageContextTests
{
    [Fact]
    public async Task BeginTransactionAsync_CreatesAndReturnsTransaction()
    {
        await using var ctx = CreateContext(out var conn);

        var tx = await ctx.BeginTransactionAsync();
        ctx.HasActiveTransaction.Should().BeTrue();
        ctx.GetCurrentTransaction().Should().BeSameAs(tx);

        await ctx.CommitTransactionAsync(tx!);
        ctx.HasActiveTransaction.Should().BeFalse();
        ctx.GetCurrentTransaction().Should().BeNull();
    }

    [Fact]
    public async Task BeginTransactionAsync_ReturnsNull_IfTransactionAlreadyActive()
    {
        await using var ctx = CreateContext(out var conn);

        var tx1 = await ctx.BeginTransactionAsync();
        var tx2 = await ctx.BeginTransactionAsync();
        tx2.Should().BeNull(); // Вторую не даст

        await ctx.CommitTransactionAsync(tx1!);
    }

    [Fact]
    public async Task CommitTransactionAsync_Throws_IfNotCurrentTransaction()
    {
        await using var ctx = CreateContext(out var conn);
        var tx = await ctx.BeginTransactionAsync();

        var fakeTx = new Mock<IDbContextTransaction>();
        fakeTx.SetupGet(x => x.TransactionId).Returns(Guid.NewGuid());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            ctx.CommitTransactionAsync(fakeTx.Object));
        await ctx.CommitTransactionAsync(tx!);
    }

    [Fact]
    public void OnModelCreating_DoesNotThrow()
    {
        using var ctx = CreateContext(out var _);
        ctx.Database.EnsureCreated();
        ctx.Items.Should().NotBeNull();
    }

    private sealed class ProxyTinyStorageContext : TinyStorageContext
    {
        public bool SaveChangesShouldThrow { get; set; }

        public ProxyTinyStorageContext(TinyStorageContext baseCtx)
            : base(new DbContextOptions<TinyStorageContext>(), new Mock<IUserAccessor>().Object)
        {
            Database.OpenConnection();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (SaveChangesShouldThrow) throw new Exception("fail!");
            return base.SaveChangesAsync(cancellationToken);
        }
    }

    private static TinyStorageContext CreateContext(out SqliteConnection connection)
    {
        connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<TinyStorageContext>()
            .UseSqlite(connection)
            .Options;

        var userAccessor = new Mock<IUserAccessor>().Object;
        var ctx = new TinyStorageContext(options, userAccessor);
        ctx.Database.EnsureCreated();
        return ctx;
    }
}
