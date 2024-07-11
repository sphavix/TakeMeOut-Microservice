using Dinner.Core.Entities;
using Dinner.Core.Repositories;
using Dinner.Infrastructure.DataStore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dinner.Infrastructure.Repositories
{
    public class DinnerRepository : IDinnerRepository
    {
        private readonly ApplicationDbContext _context;
        public DinnerRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IQueryable<Lunch> Lunches => _context.Lunches;
        public IQueryable<Rsvp> Rsvp => _context.Rsvps;

        public async Task<List<Lunch>> GetLunchesAsync(DateTime? startDate, DateTime? endDate, string userName, string searchQuery, string sort, bool descending, int? pageIndex, int? pageSize)
        {
            var query = _context.Lunches.AsQueryable();

            if(!string.IsNullOrWhiteSpace(userName))
            {
                query = query.Where(l => string.Equals(l.UserName == userName, StringComparison.OrdinalIgnoreCase));
            }

            if(startDate.HasValue)
            {
                query = query.Where(s => s.EventDate >= startDate.Value);
            }
            else
            {
                query = query.Where(d => d.EventDate >= DateTime.UtcNow);
            }

            if(endDate.HasValue)
            {
                query = query.Where(s => s.EventDate <= endDate.Value);
            }

            if(!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(d => d.Title
                .IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) != -1 ||
                d.Description.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) != -1);
            }

            query = ApplyDinnerSort(query, sort, descending);

            if(pageIndex.HasValue && pageSize.HasValue)
            {
                query = query.Skip((pageIndex.Value -1) * pageSize.Value).Take(pageSize.Value);
            }

            return await query.ToListAsync();
            
        }

        public async Task<Lunch> GetLunchByIdAsync(Guid id)
        {
            return await _context.Lunches.Include(l => l.Rsvps)
                .SingleOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<Lunch>> GetPopularLunchAsync()
        {
            return await _context.Lunches.Include(d => d.Rsvps).OrderByDescending(d => d.Rsvps.Count)
                .Take(5).ToListAsync();
        }

        public async Task<Lunch> CreateLunchAsync(Lunch lunch)
        {
            var rsvp = new Rsvp
            {
                UserName = lunch.UserName
            };

            lunch.Rsvps = new List<Rsvp> { rsvp };

            _context.Add(lunch);
            _context.Add(rsvp);
            await _context.SaveChangesAsync();

            return lunch;
        }

        public async Task<bool> UpdateLunchAsync(Lunch lunch)
        {
            _context.Update(lunch);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task DeleteLunchAsync(Guid id)
        {
            var lunch = await GetLunchByIdAsync(id);
            if(lunch != null)
            {
                foreach(Rsvp rsvp in lunch.Rsvps)
                {
                    _context.Rsvps.Remove(rsvp);
                }

                _context.Lunches.Remove(lunch);
                await _context.SaveChangesAsync();
            }
        }

        public virtual int GetLunchesCount()
        {
            return _context.Lunches.Where(d => d.EventDate >= DateTime.Now).Count();
        }

        public async Task<Rsvp> CreateRsvpAsync(Lunch lunch, string userName)
        {
            Rsvp rsvp = null;
            if(lunch != null)
            {
                if(lunch.IsUserRegistered(userName))
                {
                    rsvp = lunch.Rsvps.SingleOrDefault(r => string.Equals(r.UserName, userName, StringComparison.OrdinalIgnoreCase))!;
                }
                else
                {
                    rsvp = new Rsvp
                    {
                        UserName = userName
                    };

                    lunch.Rsvps.Add(rsvp);
                    _context.Add(rsvp);
                    await _context.SaveChangesAsync();
                }
            }
            return rsvp!;
        }

        public virtual async Task DeleteRsvpAsync(Lunch lunch, string userName)
        {
            var rsvp = lunch?.Rsvps.SingleOrDefault(r => string.Equals(r.UserName, userName, StringComparison.OrdinalIgnoreCase));
            if (rsvp != null)
            {
                _context.Rsvps.Remove(rsvp);
                await _context.SaveChangesAsync();
            };
        }

        private IQueryable<Lunch> ApplyDinnerSort(IQueryable<Lunch> query, string sort, bool descending)
        {
            // Default to sort by dinner Id
            query = descending ? query.OrderByDescending(d => d.Id) : query.OrderBy(d => d.Id);

            if (string.Equals(sort, "Title", StringComparison.OrdinalIgnoreCase))
            {
                query = descending ? query.OrderByDescending(d => d.Title) : query.OrderBy(d => d.Title);
            }
            else if (string.Equals(sort, "EventDate", StringComparison.OrdinalIgnoreCase))
            {
                query = descending ? query.OrderByDescending(d => d.EventDate) : query.OrderBy(d => d.EventDate);
            }
            else if (string.Equals(sort, "UserName", StringComparison.OrdinalIgnoreCase))
            {
                query = descending ? query.OrderByDescending(d => d.UserName) : query.OrderBy(d => d.UserName);
            }

            return query;
        }
    }
}
