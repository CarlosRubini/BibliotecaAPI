using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClBiblioteca
{
    public abstract class DAO<F, T>
    {
        public virtual SqlConnection Connection { get; set; }

        public DAO(SqlConnection connection)
        {
            Connection = connection;
        }

        public virtual void Validate(F filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
        }

        public abstract string GetSqlSelect(F filter);

        public virtual List<T> FindAll(F filter)
        {
            var itens = new List<T>();
            IDbCommand dbCommand = Connection.CreateCommand();
            dbCommand.CommandText = GetSqlSelect(filter);

            using (IDataReader dr = dbCommand.ExecuteReader())
            {
                while (dr.Read())
                {
                    itens.Add(LoadObject(dr));
                }
            }
            return itens;
        }

        public virtual T FindOne(F filter)
        {
            return FindAll(filter).FirstOrDefault();
        }

        internal abstract T LoadObject(IDataReader dr);

        public abstract void Save(T obj, SqlTransaction transaction);

        internal virtual void Save(List<T> objs)
        {
            SqlTransaction tr = Connection.BeginTransaction();
            try
            {

                foreach (T obj in objs)
                {
                    Save(obj, tr);
                }

                tr.Commit();
            }
            catch (Exception)
            {
                tr.Rollback();
                throw;
            }
        }

        public abstract void Delete(T obj, SqlTransaction transaction);

        internal virtual void Delete(List<T> objs,SqlTransaction transaction)
        {
            try
            {
                foreach (T obj in objs)
                {
                    Delete(obj, transaction);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
