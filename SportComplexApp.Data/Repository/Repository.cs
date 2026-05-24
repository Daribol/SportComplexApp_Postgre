using Microsoft.EntityFrameworkCore;
using SportComplexApp.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly SportComplexDbContext context;
        private readonly DbSet<T> dbSet;

        public Repository(SportComplexDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        public T GetById(int id)
        {
            T entity = this.dbSet
                .Find(id);

            return entity;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            T entity = await this.dbSet
                .FindAsync(id);

            return entity;
        }

        public T FirstOrDefault(Func<T, bool> predicate)
        {
            T entity = this.dbSet
                .FirstOrDefault(predicate);

            return entity;
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            T entity = await this.dbSet
                .FirstOrDefaultAsync(predicate);

            return entity;
        }

        public IEnumerable<T> GetAll()
        {
            return this.dbSet.ToArray();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await this.dbSet.ToArrayAsync();
        }

        public IQueryable<T> GetAllAttached()
        {
            return this.dbSet.AsQueryable();
        }

        public void Add(T item)
        {
            this.dbSet.Add(item);
            this.context.SaveChanges();
        }

        public async Task AddAsync(T item)
        {
            await this.dbSet.AddAsync(item);
            await this.context.SaveChangesAsync();
        }

        public void AddRange(T[] items)
        {
            this.dbSet.AddRange(items);
            this.context.SaveChanges();
        }

        public async Task AddRangeAsync(T[] items)
        {
            await this.dbSet.AddRangeAsync(items);
            await this.context.SaveChangesAsync();
        }

        public bool Delete(T entity)
        {
            this.dbSet.Remove(entity);
            this.context.SaveChanges();

            return true;
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            this.dbSet.Remove(entity);
            await this.context.SaveChangesAsync();

            return true;
        }

        public bool Update(T item)
        {
            try
            {
                this.dbSet.Attach(item);
                this.context.Entry(item).State = EntityState.Modified;
                this.context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(T item)
        {
            try
            {
                this.dbSet.Attach(item);
                this.context.Entry(item).State = EntityState.Modified;
                await this.context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
