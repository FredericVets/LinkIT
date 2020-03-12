using System;
using System.Configuration;
using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.ProductRepo
{
	[TestClass]
	public class WhenUpdatingAProduct
	{
		private ProductDto _expected;
		private ProductRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			var conStr = ConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString;
			_sut = new ProductRepository(conStr);

			var created = DateTime.Now.AddDays(-1);
			DateTimeProvider.SetDateTime(created);

			_expected = new ProductDto
			{
				CreatedBy = "user1",
				Brand = "HP",
				Type = "EliteBook"
			};

			_expected.Id = _sut.Insert(_expected);
			_expected.CreationDate = created;
		}

		[TestMethod]
		public void ThenTheDataIsInserted()
		{
			var modified = DateTime.Now;
			DateTimeProvider.SetDateTime(modified);

			_expected.ModificationDate = modified;
			_expected.ModifiedBy = "user2";

			_sut.Update(_expected);

			var actual = _sut.GetById(_expected.Id.Value);

			Assert.IsNotNull(actual);
			Assert.AreEqual(_expected, actual);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_sut.Delete(_expected.Id.Value);
		}
	}
}