using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Models;

namespace PetShop.Services
{
    public class PasswordResetService
    {
        private readonly PetShopContext _context;
        private readonly IEmailService _emailService;

        public PasswordResetService(PetShopContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task RequestPasswordResetAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return;

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            var resetUrl = $"http://127.0.0.1:3000/Frontend/password-reset/reset.html?token={token}";

            await _emailService.SendAsync(user.Email, "Password Reset",
                $"Click the link to reset your password: {resetUrl}");
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var resetToken = await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token && t.ExpiresAt > DateTime.UtcNow);

            if (resetToken == null) return false;

            resetToken.User.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _context.PasswordResetTokens.Remove(resetToken);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
