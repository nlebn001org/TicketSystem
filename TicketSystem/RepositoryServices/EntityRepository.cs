using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketSystem.Web.Models;
using TicketSystem.Web.LogServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace TicketSystem.Web.RepositoryServices
{
    public class EntityRepository<T> : IRepository<T> where T : class, IEntity
    {
        readonly ILogService log;
        readonly Func<SystemDbContext> contextFactory;

        public EntityRepository(Func<SystemDbContext> contextFactory, ILogService log)
        {
            this.log = new ConsoleLogService();
            this.contextFactory = contextFactory;
            this.log = log;
        }

        public virtual async void DeleteAsync(int id)
        {
            using (SystemDbContext db = contextFactory())
            {
                T item = await db.Set<T>().FindAsync(id);
                if (item == null) return;

                db.Set<T>().Remove(item);

                BeforeDelete();
                await db.SaveChangesAsync();
                AfterDelete();

                string entityTypeName = item.GetType().Name;
                log?.LogMessage($"Deleted {entityTypeName} with id {id}");
            }
        }

        public virtual async Task<IEnumerable<T>> getAllAsync()
        {
            using (SystemDbContext db = contextFactory())
            {
                return await db.Set<T>().ToListAsync();
            }
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            using (SystemDbContext db = contextFactory())
            {
                return await db.Set<T>().FindAsync(id);
            }

        }

        public virtual async void SaveAsync(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            using (SystemDbContext db = contextFactory())
            {
                bool itemExists = await db.Set<T>().AnyAsync(x => x.Id == item.Id);
                db.Entry(item).State = itemExists ? EntityState.Modified : EntityState.Added;

                BeforeSave();
                await db.SaveChangesAsync();
                AfterSave();
            }
            string entityTypeName = item.GetType().Name;
            log?.LogMessage($"Saved {entityTypeName} with id {item.Id}");
        }

        protected virtual void BeforeDelete() { }
        protected virtual void AfterDelete() { }
        protected virtual void BeforeSave() { }
        protected virtual void AfterSave() { }
    }
}
