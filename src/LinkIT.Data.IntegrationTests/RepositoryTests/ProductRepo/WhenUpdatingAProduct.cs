using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;

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

			_expected = new ProductDto
			{
				CreatedBy = "user1",
				Brand = "HP",
				Type = "EliteBook"
			};

			_expected.Id = _sut.Insert(_expected);
		}

		[TestMethod]
		public void ThenTheDataIsUpdated()
		{
			_expected.Brand = "Dell";
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