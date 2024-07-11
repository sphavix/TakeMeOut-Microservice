using Dinner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dinner.Core.Repositories
{
    public interface IDinnerRepository
    {
        IQueryable<Lunch> Lunches { get; }
        IQueryable<Rsvp> Rsvp { get; }

        Task<Lunch> GetLunchByIdAsync(Guid id);
        Task<List<Lunch>> GetLunchesAsync(DateTime? startDate, DateTime? endDate, string userName, string searchQuery, string sort, bool descending, int? pageIndex, int? pageSize);
        Task<List<Lunch>> GetPopularLunchAsync();
        Task<Lunch> CreateLunchAsync(Lunch lunch);
        Task<bool> UpdateLunchAsync(Lunch lunch);
        Task DeleteLunchAsync(Guid id);
        int GetLunchesCount();
        Task<Rsvp> CreateRsvpAsync(Lunch lunch, string userName);
        Task DeleteRsvpAsync(Lunch lunch, string userName);
    }
}
