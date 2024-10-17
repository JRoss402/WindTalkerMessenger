using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WindTalkerMessenger.Models.DataLayer;
using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Controllers
{
    public class ChatMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatMessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ChatMessages
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Chats.Include(c => c.IdentityReceiver).Include(c => c.IdentitySender);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ChatMessages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Chats == null)
            {
                return NotFound();
            }

            var chatMessage = await _context.Chats
                .Include(c => c.IdentityReceiver)
                .Include(c => c.IdentitySender)
                .FirstOrDefaultAsync(m => m.ChatMessageId == id);
            if (chatMessage == null)
            {
                return NotFound();
            }

            return View(chatMessage);
        }

        // GET: ChatMessages/Create
        public IActionResult Create()
        {
            ViewData["MsgReceiverEmail"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["MsgSenderEmail"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ChatMessages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChatMessageId,MessageUID,MessageStatus,UserMessage,MessageDate,isLoaded,MsgSenderEmail,MsgReceiverEmail")] ChatMessage chatMessage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chatMessage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MsgReceiverEmail"] = new SelectList(_context.Users, "Id", "Id", chatMessage.MsgReceiverEmail);
            ViewData["MsgSenderEmail"] = new SelectList(_context.Users, "Id", "Id", chatMessage.MsgSenderEmail);
            return View(chatMessage);
        }

        // GET: ChatMessages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Chats == null)
            {
                return NotFound();
            }

            var chatMessage = await _context.Chats.FindAsync(id);
            if (chatMessage == null)
            {
                return NotFound();
            }
            ViewData["MsgReceiverEmail"] = new SelectList(_context.Users, "Id", "Id", chatMessage.MsgReceiverEmail);
            ViewData["MsgSenderEmail"] = new SelectList(_context.Users, "Id", "Id", chatMessage.MsgSenderEmail);
            return View(chatMessage);
        }

        // POST: ChatMessages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChatMessageId,MessageUID,MessageStatus,UserMessage,MessageDate,isLoaded,MsgSenderEmail,MsgReceiverEmail")] ChatMessage chatMessage)
        {
            if (id != chatMessage.ChatMessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chatMessage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChatMessageExists(chatMessage.ChatMessageId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MsgReceiverEmail"] = new SelectList(_context.Users, "Id", "Id", chatMessage.MsgReceiverEmail);
            ViewData["MsgSenderEmail"] = new SelectList(_context.Users, "Id", "Id", chatMessage.MsgSenderEmail);
            return View(chatMessage);
        }

        // GET: ChatMessages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Chats == null)
            {
                return NotFound();
            }

            var chatMessage = await _context.Chats
                .Include(c => c.IdentityReceiver)
                .Include(c => c.IdentitySender)
                .FirstOrDefaultAsync(m => m.ChatMessageId == id);
            if (chatMessage == null)
            {
                return NotFound();
            }

            return View(chatMessage);
        }

        // POST: ChatMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Chats == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Chats'  is null.");
            }
            var chatMessage = await _context.Chats.FindAsync(id);
            if (chatMessage != null)
            {
                _context.Chats.Remove(chatMessage);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChatMessageExists(int id)
        {
          return (_context.Chats?.Any(e => e.ChatMessageId == id)).GetValueOrDefault();
        }
    }
}
