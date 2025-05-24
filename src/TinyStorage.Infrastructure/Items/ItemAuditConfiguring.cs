namespace Itmo.TinyStorage.Infrastructure.Items;

public sealed class ItemAuditConfiguring : IEntityTypeConfiguration<ItemAuditModel>
{
    public void Configure(EntityTypeBuilder<ItemAuditModel> builder)
    {
        builder
            .HasOne(a => a.ItemModel)
            .WithMany(i => i.ItemAuditModels)
            .HasForeignKey(a => a.ItemId);
    }
}