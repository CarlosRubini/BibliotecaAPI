using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using ClBiblioteca;

namespace AppBibliotecaAPI.Controllers
{
	[ApiController]
	[Route("bookLoan")]
	public class BookLoanController : ControllerBase
	{
		private readonly IConfiguration Configuration;
		public BookLoanController(IConfiguration configuration)
		{
			this.Configuration = configuration;
		}

		[HttpGet]
		[Route("getLoans")]
		public IEnumerable<BookLoan> GetAll()
		{
			List<BookLoan> loans;
			using SqlConnection connection = new(Configuration.GetConnectionString("AzureConnection"));
			{
				connection.Open();
				loans = new BookLoanDAO(connection).FindAll(new BookFilter());
				connection.Close();
			}
			return loans;
		}

		[HttpPut]
		[Route("saveBookLoan")]
		public BookLoan Save(BookLoan loan)
		{
			ValidateLoan(loan);
			using SqlConnection connection = new(Configuration.GetConnectionString("AzureConnection"));
			{
				connection.Open();
				SqlTransaction tr = connection.BeginTransaction();
				try
				{
					new BookLoanDAO(connection).Save(loan, tr);
					tr.Commit();
				}
				catch (Exception ex)
				{
					tr.Rollback();
					throw new ApplicationException(ex.Message);
				}
				finally
				{
					tr.Dispose();
					connection.Close();
				}
			}
			return loan;
		}

		[HttpDelete]
		[Route("deleteBookLoan")]
		public bool Delete(BookLoan loan)
		{
			ValidateLoan(loan);
			using SqlConnection connection = new(Configuration.GetConnectionString("AzureConnection"));
			{
				connection.Open();
				SqlTransaction tr = connection.BeginTransaction();
				try
				{
					new BookLoanDAO(connection).Delete(loan, tr);
					tr.Commit();
				}
				catch (Exception ex)
				{
					tr.Rollback();
					throw new ApplicationException(ex.Message);
				}
				finally
				{
					tr.Dispose();
					connection.Close();
				}
			}
			return true;
		}

		private static void ValidateLoan(BookLoan loan)
		{
			if (loan == null) throw new ArgumentNullException(loan!.ToString());
			if (loan.BookId == 0) throw new ArgumentNullException(loan.ToString());
			if (loan.Borrowed == DateTime.MinValue) throw new ArgumentNullException(loan.ToString());
		}
	}
}
