using System;
using Basket.Api.Models;
using Microsoft.AspNetCore.Mvc;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace Basket.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : Controller
    {
        // GET
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(new CatalogItemRequest
            {
                Id = RandomizerFactory.GetRandomizer(new FieldOptionsInteger{Max=99, Min=1,UseNullValues = false}).Generate(),
                Name = RandomizerFactory.GetRandomizer(new FieldOptionsFullName()).Generate(),
                PictureUri = RandomizerFactory.GetRandomizer(new FieldOptionsIPv6Address()).Generate(),
                Price = Convert.ToDecimal(RandomizerFactory.GetRandomizer(new FieldOptionsInteger{Max=199, Min=100,UseNullValues = false}).Generate())
            });
        }
    }
}