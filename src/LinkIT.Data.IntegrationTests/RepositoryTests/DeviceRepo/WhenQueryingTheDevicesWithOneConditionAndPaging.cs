﻿using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.DeviceRepo
{
	[TestClass]
	public class WhenQueryingTheDevicesWithOneConditionAndPaging
	{
		private List<DeviceDto> _devices;
		private DeviceRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			_sut = new DeviceRepository(ConnectionString.Get());

			_devices = new List<DeviceDto>()
			{
				new DeviceDto
				{
					Brand = "HP",
					Type = "AwesomeBook",
					Owner = "Unknown",
					Tag = "CRD-X-11111"
				},
				new DeviceDto
				{
					Brand = "Dell",
					Type = "Latitude 7290",
					Owner = "Unknown",
					Tag = "CRD-X-22222"
				},
				new DeviceDto
				{
					Brand = "Dell",
					Type = "Latitude 7490",
					Owner = "Unknown",
					Tag = "CRD-X-33333"
				},
				new DeviceDto
				{
					Brand = "Dell",
					Type = "Latitude 5590",
					Owner = "Unknown",
					Tag = "CRD-X-44444"
				},
				new DeviceDto
				{
					Brand = "Dell",
					Type = "Latitude 5590",
					Owner = "Unknown",
					Tag = "CRD-X-55555"
				}
			};

			_devices.ForEach(x => x.Id = _sut.Insert(x));
		}

		[TestMethod]
		public void ThenTheResultIsAsExpected()
		{
			var query = new DeviceQuery { Owner = "Unknown" };
			var pageInfo = new PageInfo(
				2, 
				2, 
				new OrderBy(DeviceRepository.TAG_COLUMN, Order.DESCENDING));
			var actual = _sut.PagedQuery(pageInfo, query);

			// Simulate the paging on the in-memory collection.
			var expected = _devices.OrderByDescending(x => x.Tag).Skip(2).Take(2).ToList();

			Assert.AreEqual(pageInfo, actual.PageInfo);
			Assert.AreEqual(5, actual.TotalCount);
			Assert.AreEqual(2, actual.Result.Count());
			foreach (var item in expected)
			{
				var actualDto = actual.Result.Single(x => x.Id == item.Id);
				Assert.AreEqual(item, actualDto);
			}
		}

		[TestCleanup]
		public void CleanUp()
		{
			_devices.ForEach(x => _sut.Delete(x.Id.Value));
		}
	}
}