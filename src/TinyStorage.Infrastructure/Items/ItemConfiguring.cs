namespace Itmo.TinyStorage.Infrastructure.Items;

public sealed class ItemConfiguring : IEntityTypeConfiguration<ItemModel>
{
    public void Configure(EntityTypeBuilder<ItemModel> builder)
    {
        builder.HasIndex(item => item.Name);
    }
}
