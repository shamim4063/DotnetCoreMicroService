using System;

namespace UserManagement.Domain;

public sealed class UserWarehouseProjection
{
    public long UserId { get; private set; }
    public long WarehouseId { get; private set; }
    public bool IsDefault { get; private set; }
    public string WarehouseCode { get; private set; } = default!;
    public string WarehouseName { get; private set; } = default!;
    public string Status { get; private set; } = default!;
    public string? OtherMetadata { get; private set; }

    private UserWarehouseProjection() { }

    public UserWarehouseProjection(long userId, long warehouseId, bool isDefault, string warehouseCode, string warehouseName, string status, string? otherMetadata = null)
    {
        UserId = userId;
        WarehouseId = warehouseId;
        IsDefault = isDefault;
        WarehouseCode = warehouseCode;
        WarehouseName = warehouseName;
        Status = status;
        OtherMetadata = otherMetadata;
    }
}
