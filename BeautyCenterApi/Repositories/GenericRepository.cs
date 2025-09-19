using Microsoft.EntityFrameworkCore;
using BeautyCenterApi.Data;
using BeautyCenterApi.Interfaces;
using BeautyCenterApi.Services;
using System.Linq.Expressions;
using System.Reflection;

namespace BeautyCenterApi.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly BeautyCenterDbContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly ITenantService _tenantService;

        public GenericRepository(BeautyCenterDbContext context, ITenantService tenantService)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _tenantService = tenantService;
        }

        protected IQueryable<T> ApplyTenantFilter(IQueryable<T> query)
        {
            // SuperAdmin ise tenant filtresi uygulama
            if (_tenantService.IsSuperAdmin())
                return query;

            var currentTenantId = _tenantService.GetCurrentTenantId();
            if (!currentTenantId.HasValue)
                return query.Where(x => false); // Tenant bilgisi yoksa hiçbir kayıt döndürme

            // T tipinde TenantId property'si varsa filtre uygula
            var tenantIdProperty = typeof(T).GetProperty("TenantId");
            if (tenantIdProperty != null)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, tenantIdProperty);
                var constant = Expression.Constant(currentTenantId.Value);
                var equal = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

                return query.Where(lambda);
            }

            return query;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query.Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            // TenantId property'si varsa ve değer atanmamışsa mevcut tenant'ı ata
            var tenantIdProperty = typeof(T).GetProperty("TenantId");
            if (tenantIdProperty != null && !_tenantService.IsSuperAdmin())
            {
                var currentTenantId = _tenantService.GetCurrentTenantId();
                if (currentTenantId.HasValue)
                {
                    var currentValue = tenantIdProperty.GetValue(entity);
                    if (currentValue == null || (int)currentValue == 0)
                    {
                        tenantIdProperty.SetValue(entity, currentTenantId.Value);
                    }
                }
            }

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.FindAsync(id) != null;
        }

        public async Task<int> CountAsync()
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query.CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query.CountAsync(predicate);
        }
    }
}