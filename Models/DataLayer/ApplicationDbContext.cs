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
        public DbSet<ChatMessage> Chats { get; set; }
        public DbSet<MessageQueue> Queues { get; set; }
        //public DbSet<Groups> Groups { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ChatMessage>()
                .HasOne(b => b.IdentitySender)
                .WithMany() 
                .HasForeignKey(b => b.MsgSenderEmail)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ChatMessage>()
                .HasOne(b => b.IdentityReceiver)
                .WithMany() 
                .HasForeignKey(b => b.MsgReceiverEmail)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<MessageQueue>()
                .HasOne(b => b.IdentitySender)
                .WithMany() 
                .HasForeignKey(b => b.MsgSenderEmail)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<MessageQueue>()
                .HasOne(b => b.IdentityReceiver)
                .WithMany() 
                .HasForeignKey(b => b.MsgReceiverEmail)
                .OnDelete(DeleteBehavior.Cascade);
            


        }
    }
}
