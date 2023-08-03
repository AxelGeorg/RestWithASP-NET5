using Microsoft.EntityFrameworkCore;
using RestWithASPNETUdemy.Model;
using RestWithASPNETUdemy.Model.Base;
using RestWithASPNETUdemy.Model.Context;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace RestWithASPNETUdemy.Repository.Generic
{
    public class GenericRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected MySQLContext _context;

        private DbSet<T> _dabaSet;

        public GenericRepository(MySQLContext context)
        {
            _context = context;
            _dabaSet = _context.Set<T>();
        }

        public List<T> FindAll()
        {
            return _dabaSet.ToList();
        }

        public T FindById(long id)
        {
            return _dabaSet.SingleOrDefault(p => p.Id.Equals(id));
        }

        public T Create(T item)
        {
            try
            {
                _dabaSet.Add(item);
                _context.SaveChanges();

                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T Update(T item)
        {
            if (!Exists(item.Id))
                return null;

            T result = _dabaSet.SingleOrDefault(p => p.Id.Equals(item.Id));
            if (result == null)
                return null;

            try
            {
                _context.Entry(result).CurrentValues.SetValues(item);
                _context.SaveChanges();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Delete(long id)
        {
            T result = _dabaSet.SingleOrDefault(p => p.Id.Equals(id));
            if (result != null)
            {
                try
                {
                    _dabaSet.Remove(result);
                    _context.SaveChanges();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public bool Exists(long id)
        {
            return _dabaSet.Any(p => p.Id.Equals(id));
        }

        public List<T> FindWithPagedSearch(string query)
        {
            return _dabaSet.FromSqlRaw<T>(query).ToList();
        }

        public int GetCount(string query)
        {
            string result = "";
            using (DbConnection connection = _context.Database.GetDbConnection())
            {
                connection.Open();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    result = command.ExecuteScalar().ToString();
                }
            }

            return int.Parse(result);
        }
    }
}
