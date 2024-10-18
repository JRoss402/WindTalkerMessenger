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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

           /* builder.Entity<Message>();
                 .HasOne(b => b.IdentitySender)
                 .WithMany() 
                 .HasForeignKey(b => b.MessageSenderEmail)
                 .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Message>();
                .HasOne(b => b.IdentityReceiver)
                .WithMany() 
                .HasForeignKey(b => b.MessageReceiverEmail)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<MessageQueue>()
                .HasOne(b => b.IdentitySender)
                .WithMany() 
                .HasForeignKey(b => b.MsgSenderEmail)
                .OnDelete(DeleteBehavior.SetNull);


            builder.Entity<MessageQueue>()
                .HasOne(b => b.IdentityReceiver)
                .WithMany() 
                .HasForeignKey(b => b.MsgReceiverEmail)
                .OnDelete(DeleteBehavior.SetNull);*/
            
        }
    }
}
