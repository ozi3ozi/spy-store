using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SpyStore.Dal.EfStructures;
using SpyStore.Dal.Exceptions;
using SpyStore.Models.Entities.Base;

namespace SpyStore.Dal.Repos.Base
{
    //EntityBase constrains the type to EntityBase and new() limits the types to classes that have a parameterless constructor.
    public class RepoBase<T> : IRepo<T> where T : EntityBase, new()
    {
        //The repo needs an instance of the StoreContext class in order to operate on the database and entities in the object model.
        //This will be passed in through dependency injection for typical use.
        public DbSet<T> Table { get; }
        public StoreContext Context { get; }
        private readonly bool _disposeContext;
        public bool HasChanges => Context.ChangeTracker.HasChanges();
        public (string Schema, string TableName) TableSchemaAndName
        {
            get
            {
                var metaData = Context.Model.FindEntityType(typeof(T).FullName).SqlServer();
                return (metaData.Schema, metaData.TableName);
            }
        }

        public RepoBase(StoreContext context)
        {
            Context = context;
            Table = Context.Set<T>();
        }

        protected RepoBase(DbContextOptions<StoreContext> options) : this(new StoreContext(options))
        {
            _disposeContext = true;
        }

        public virtual void Dispose()
        {
            if (_disposeContext)
            {
                Context.Dispose();
            }
        }

        public int SaveChanges()
        {
            try
            {
                return Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new SpyStoreConcurrencyException("A concurrency error happenend", ex);
            }
            catch (RetryLimitExceededException ex)
            {
                throw new SpyStoreRetryLimitExceededException("There is a problem with your connection", ex);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlException)
                {
                    if (sqlException.Message.Contains("FOREIGN KEY constraint", StringComparison.OrdinalIgnoreCase))
                    {
                        if (sqlException.Message.Contains("Table \"Store.Products\", column 'Id'", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new SpyStoreInvalidProductException($"Invalid Product Id\r\n{ex.Message}", ex);
                        }
                        if (sqlException.Message.Contains("Table \"Store.Customers\", column 'Id'", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new SpyStoreInvalidCustomerException($"Invalid Customer Id\r\n{ex.Message}", ex);
                        }
                    }
                }
                throw new SpyStoreException("An error occured updating the database", ex);
            }
            catch (Exception ex)
            {
                throw new SpyStoreException("An error occured updating the database", ex);
            }
        }

        //The persist parameter sets whether the repo executes SaveChanges when the Create/Update/Delete method is called, 
        //or just adds the changes to the ChangeTracker.
        public virtual int Add(T entity, bool persist = true)
        {
            Table.Add(entity);
            return persist ? SaveChanges() : 0;
        }

        public virtual int AddRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.AddRange(entities);
            return persist ? SaveChanges() : 0;
        }

        public virtual int Update(T entity, bool persist = true)
        {
            Table.Update(entity);
            return persist ? SaveChanges() : 0;
        }

        public virtual int UpdateRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.UpdateRange(entities);
            return persist ? SaveChanges() : 0;
        }

        public virtual int Delete(T entity, bool persist = true)
        {
            Table.Remove(entity);
            return persist ? SaveChanges() : 0;
        }

        public virtual int DeleteRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.RemoveRange(entities);
            return persist ? SaveChanges() : 0;
        }

        public T Find(int? id) => Table.Find(id);
        // The FindAsNoTracking method demonstrates retrieving a record but not adding it to the ChangeTracker using AsNoTracking. 
        //Note that this method uses the Where method and not the Find method, so it does not search the ChangeTracker for the 
        //instance; it always queries the database. 
        public T FindAsNoTracking(int id) => Table.Where(e => e.Id == id).AsNoTracking().FirstOrDefault();
        public T FindIgnoreQueryFilters(int id) => Table.IgnoreQueryFilters().FirstOrDefault(e => e.Id == id);
        public virtual IEnumerable<T> GetAll() => Table;
        public virtual IEnumerable<T> GetAll(Expression<Func<T, object>> orderBy) => Table.OrderBy(orderBy);
        //The GetRange method is used for chunking the data from the database. The first parameter is an IQueryable<T>, while the 
        //next two dictate how many records to skip and how many to take
        public virtual IEnumerable<T> GetRange(IQueryable<T> query, int skip, int take) => query.Skip(skip).Take(take);

    }
}
