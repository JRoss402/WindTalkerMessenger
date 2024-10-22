using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Models.DataLayer
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Message> Chats { get; set; }
        public DbSet<MessageQueue> Queues { get; set; }
        //public DbSet<Groups> Groups { get; set; }
        public DbSet<Guest> Guests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Message>()
                .Property(x => x.MessageReceiverEmail)
                .HasDefaultValue("Guest");

            builder.Entity<Message>()
                .Property(x => x.MessageSenderEmail)
                .HasDefaultValue("Guest");

            builder.Entity<MessageQueue>()
                .Property(x => x.MessageReceiverEmail)
                .HasDefaultValue("Guest");

            builder.Entity<MessageQueue>()
                .Property(x => x.MessageSenderEmail)
                .HasDefaultValue("Guest");        
        }
    }
}
