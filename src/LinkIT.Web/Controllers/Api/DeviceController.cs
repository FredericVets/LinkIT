using LinkIT.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
    // See docs at https://www.tutorialsteacher.com/webapi/web-api-tutorials
    public class DeviceController : ApiController
    {
        // GET api/device
        public IEnumerable<Device> Get()
        {
            return new Device[]
            {
                new Device
                {
                    Id = Guid.NewGuid(),
                    Tag = "CRD-L-07140",
                    Owner = "u0122713",
                    Brand = "Dell",
                    Type = "Latitude 7390"
                },
                new Device
                {
                    Id = Guid.NewGuid(),
                    Tag = "CRD-L-07654",
                    Owner = "u0122713",
                    Brand = "HP",
                    Type = "EliteBook 750 G6"
                },
            };
        }

        // example : GET api/values/5
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var device = new Device
            {
                Id = id,
                Tag = "CRD-L-07140",
                Owner = "u0122713",
                Brand = "Dell",
                Type = "Latitude 7390"
            };

            return Ok(device);
        }

        // POST api/values
        // 2 following attributes are the default behaviour.
        // Create the new Device.
        [HttpPost]
        public void Post(Device value)
        {
        }

        // PUT api/values/5
        // Fully updates the Device.
        public IHttpActionResult Put(Device value)
        {
            if (value.Id == Guid.Empty)
                return NotFound();

            return Ok();
        }

        // DELETE api/values/5
        // You can also use the void return type.
        // Then the client will receive status code 204 as result of a DELETE request.
        public void Delete(Guid id)
        {
        }

        // Partial update of the Device.
        public void Patch(Device value)
        {
        }
    }
}