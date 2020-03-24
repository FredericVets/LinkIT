using LinkIT.Data.Paging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;

namespace LinkIT.Data.UnitTests.PagingTests
{
	[TestClass]
	public class WhenConvertingAStringToAnOrderByInstance
	{
		private TypeConverter _sut;

		[TestInitialize]
		public void Setup()
		{
			_sut = TypeDescriptor.GetConverter(typeof(OrderBy));
		}

		[TestMethod]
		public void ThenTheConversionIsSuccessful()
		{
			var expected = new OrderBy("Id", Order.DESCENDING);
			var actual = _sut.ConvertFrom("-Id") as OrderBy;

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ThenTheConversionIsNotSuccessful()
		{
			Assert.ThrowsException<NotSupportedException>(() => _sut.ConvertFrom("*Id"));
		}
	}
}