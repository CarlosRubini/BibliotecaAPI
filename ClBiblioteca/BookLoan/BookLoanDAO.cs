using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClBiblioteca
{
	public class BookLoanDAO : DAO<BookFilter, BookLoan>
	{
		public BookLoanDAO(SqlConnection connection) : base(connection)
		{
		}

		public override string GetSqlSelect(BookFilter filter)
		{
			StringBuilder strSQL = new(@"SELECT BKL.id
											   ,BKL.bookId
											   ,BKL.borrowed
											   ,BKL.returnDate
											   ,BKL.returned
										  FROM BtlBookLoan BKL");
			if (filter.ID.HasValue)
			{
				strSQL.AppendLine($@" AND BKL.bookId = {filter.ID}");
			}
			return strSQL.ToString();
		}

		internal override BookLoan LoadObject(IDataReader dr)
		{
			return new BookLoan(true
							   ,id: dr.GetDouble(0)
							   ,bookId: dr.GetDouble(1)
							   ,borrowed: dr.GetDateTime(2)
							   ,returnDate: dr.GetDateTime(3)
							   ,returned: dr.GetDateTime(4));
		}

		public override void Save(BookLoan bookLoan, SqlTransaction transaction)
		{
			string strSQL;
			if (!bookLoan.Exists)
			{
				strSQL = @"INSERT INTO BtlBookLoan(bookId,borrowed,return,returned) 
						   VALUES(@bookId,@borrowed,@return,@returned)
						   Select Scope_Identity()";
				SqlCommand comando = new(strSQL, Connection, transaction);
				comando.Parameters.Add(new SqlParameter("@bookId", bookLoan.BookId));
				comando.Parameters.Add(new SqlParameter("@borrowed", bookLoan.Borrowed));
				comando.Parameters.Add(new SqlParameter("@return", bookLoan.ReturnDate));
				comando.Parameters.Add(new SqlParameter("@returned", bookLoan.Returned));
				bookLoan.Id = Convert.ToInt32(comando.ExecuteScalar());
				bookLoan.Exists = true;
			}
			else
			{
				strSQL = "UPDATE BtlBookLoan set return=@return,returned=@returned WHERE id=@id";
				SqlCommand comando = new(strSQL, Connection, transaction);
				comando.Parameters.Add(new SqlParameter("@id", bookLoan.Id));
				comando.Parameters.Add(new SqlParameter("@return", bookLoan.ReturnDate));
				comando.Parameters.Add(new SqlParameter("@returned", bookLoan.Returned));
				comando.ExecuteScalar();
			}
		}

		public override void Delete(BookLoan bookLoan, SqlTransaction transaction)
		{
			string strSQL = "DELETE FROM BtlBookLoan WHERE id=@id";
			SqlCommand comando = new(strSQL, Connection, transaction);
			comando.Parameters.Add(new SqlParameter("@id", bookLoan.Id));
			comando.ExecuteNonQuery();
		}
	}
}
