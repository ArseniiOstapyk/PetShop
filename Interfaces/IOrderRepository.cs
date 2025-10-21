using PetShop.DTO.Order;

namespace PetShop.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderListItemDto>> GetMyOrdersAsync(int ownerId);
        Task<OrderDetailDto?> GetMyOrderByIdAsync(int orderId, int ownerId);
        Task<OrderDetailDto> AddListingToCurrentOrNewOrderAsync(int ownerId, int listingId);
        Task<bool> RemoveItemAsync(int orderId, int orderListingId, int ownerId);
        Task<bool> DeleteAsync(int orderId, int ownerId);

        Task<bool> MarkPaidAsync(int orderId, int ownerId);
    }
}
