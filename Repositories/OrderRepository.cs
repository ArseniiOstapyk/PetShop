using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.DTO.Order;
using PetShop.Interfaces;
using PetShop.Models;
using System.Security.Cryptography;

namespace PetShop.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PetShopContext _context;

        public OrderRepository(PetShopContext context)
        {
            _context = context;
        }

        private static int Random8DigitInt()
        {
            var bytes = RandomNumberGenerator.GetBytes(4);
            uint val = BitConverter.ToUInt32(bytes, 0) % 100_000_000;
            return (int)val;
        }

        private async Task<int> GenerateUniqueNumberAsync()
        {
            for (int i = 0; i < 5; i++)
            {
                int candidate = Random8DigitInt();
                bool exists = await _context.Orders.AnyAsync(o => o.Number == candidate);
                if (!exists) return candidate;
            }
            int lastId = await _context.Orders.OrderByDescending(o => o.Id).Select(o => o.Id).FirstOrDefaultAsync();
            return (lastId + 1) % 100_000_000;
        }

        private static string Format8(int number) => number.ToString("D8");

        public async Task<IEnumerable<OrderListItemDto>> GetMyOrdersAsync(int ownerId)
        {
            var rows = await _context.Orders
                .Where(o => o.OwnerId == ownerId)
                .Select(o => new
                {
                    o.Id,
                    o.Number,
                    CreatedOn = o.CreatedOn ?? DateTime.UtcNow,
                    IsPaid = o.IsPaid ?? false,
                    ListingsCount = _context.OrderListings.Count(ol => ol.OrderId == o.Id),
                    TotalSum = _context.OrderListings
                        .Where(ol => ol.OrderId == o.Id)
                        .Join(_context.Listings, ol => ol.ListingId, l => l.Id, (ol, l) => l.Price)
                        .Sum()
                })
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return rows.Select(x => new OrderListItemDto
            {
                Id = x.Id,
                Number = Format8(x.Number),
                ListingsCount = x.ListingsCount,
                TotalSum = x.TotalSum,
                IsPaid = x.IsPaid,
                CreatedOn = x.CreatedOn
            });
        }

        public async Task<OrderDetailDto?> GetMyOrderByIdAsync(int orderId, int ownerId)
        {
            var head = await _context.Orders
                .Where(o => o.Id == orderId && o.OwnerId == ownerId)
                .Select(o => new
                {
                    o.Id,
                    o.Number,
                    IsPaid = o.IsPaid ?? false,
                    CreatedOn = o.CreatedOn ?? DateTime.UtcNow
                })
                .FirstOrDefaultAsync();

            if (head == null) return null;

            var items = await
                (from ol in _context.OrderListings
                 where ol.OrderId == orderId
                 join l in _context.Listings on ol.ListingId equals l.Id
                 join u in _context.Users on ol.OwnerId equals u.Id into gu
                 from u in gu.DefaultIfEmpty()
                 select new OrderItemDto
                 {
                     OrderListingId = ol.Id,
                     ListingId = l.Id,
                     ListingName = l.Name,
                     Price = l.Price,
                     SellerEmail = u != null ? u.Email : null
                 }).ToListAsync();

            return new OrderDetailDto
            {
                Id = head.Id,
                Number = Format8(head.Number),
                IsPaid = head.IsPaid,
                CreatedOn = head.CreatedOn,
                ListingsCount = items.Count,
                TotalSum = items.Sum(i => i.Price),
                Items = items
            };
        }

        public async Task<OrderDetailDto> AddListingToCurrentOrNewOrderAsync(int ownerId, int listingId)
        {
            var listing = await _context.Listings
                .Select(l => new { l.Id, l.OwnerId })
                .FirstOrDefaultAsync(l => l.Id == listingId);
            if (listing == null) throw new KeyNotFoundException("Listing not found.");

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OwnerId == ownerId && (o.IsPaid == null || o.IsPaid == false));

            if (order == null)
            {
                order = new Order
                {
                    OwnerId = ownerId,
                    CreatedOn = DateTime.UtcNow,
                    IsPaid = false,
                    Number = await GenerateUniqueNumberAsync()
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
            }

            bool exists = await _context.OrderListings.AnyAsync(ol => ol.OrderId == order.Id && ol.ListingId == listingId);
            if (!exists)
            {
                _context.OrderListings.Add(new OrderListing
                {
                    OrderId = order.Id,
                    ListingId = listingId,
                    OwnerId = listing.OwnerId
                });
                await _context.SaveChangesAsync();
            }

            var dto = await GetMyOrderByIdAsync(order.Id, ownerId);
            return dto ?? throw new InvalidOperationException("Failed to load order after update.");
        }

        public async Task<bool> RemoveItemAsync(int orderId, int orderListingId, int ownerId)
        {
            bool isOwner = await _context.Orders.AnyAsync(o => o.Id == orderId && o.OwnerId == ownerId);
            if (!isOwner) return false;

            var item = await _context.OrderListings
                .FirstOrDefaultAsync(ol => ol.Id == orderListingId && ol.OrderId == orderId);
            if (item == null) return false;

            _context.OrderListings.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int orderId, int ownerId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.OwnerId == ownerId);
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkPaidAsync(int orderId, int ownerId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.OwnerId == ownerId);
            if (order == null) return false;

            order.IsPaid = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
