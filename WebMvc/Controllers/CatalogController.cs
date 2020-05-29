using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Basket.Api.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.ViewModels.Pagination;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.ViewModels.CatalogViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Serilog;

namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    public class CatalogController : Controller
    {
        private ICatalogService _catalogSvc;
        private readonly IEventBus _eventBus;

        public CatalogController(ICatalogService catalogSvc, IEventBus eventBus )
        {
            _catalogSvc = catalogSvc;
            _eventBus = eventBus;
        }

        public async Task<IActionResult> Ping(int? BrandFilterApplied, int? TypesFilterApplied, int? page,
            [FromQuery] string errorMsg)
        {
            Log.Information(" ------------- Catalog Ping ------------- ");
            
            var eventMessage = new PingIntegrationEvent();

            
            _eventBus.Publish(eventMessage);
            
            Log.Logger.Information(" ------------- Catalog Ping - After Publish ------------- ");
            
            
            var itemsPage = 10;
            var catalog = await _catalogSvc.GetCatalogItems(page ?? 0, itemsPage, BrandFilterApplied, TypesFilterApplied);
            var vm = new IndexViewModel()
            {
                CatalogItems = catalog.Data,
                Brands = await _catalogSvc.GetBrands(),
                Types = await _catalogSvc.GetTypes(),
                BrandFilterApplied = BrandFilterApplied ?? 0,
                TypesFilterApplied = TypesFilterApplied ?? 0,
                PaginationInfo = new PaginationInfo()
                {
                    ActualPage = page ?? 0,
                    ItemsPerPage = catalog.Data.Count,
                    TotalItems = catalog.Count, 
                    TotalPages = (int)Math.Ceiling(((decimal)catalog.Count / itemsPage))
                }
            };
            
            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";
            
            ViewBag.BasketInoperativeMsg = errorMsg;

            return View(vm);
        }
        
        public async Task<IActionResult> Index(int? BrandFilterApplied, int? TypesFilterApplied, int? page,
            [FromQuery] string errorMsg)
        {
            Log.Information(" ------------- Catalog Index ------------- ");
            var itemsPage = 10;
            var catalog = await _catalogSvc.GetCatalogItems(page ?? 0, itemsPage, BrandFilterApplied, TypesFilterApplied);
            var vm = new IndexViewModel()
            {
                CatalogItems = catalog.Data,
                Brands = await _catalogSvc.GetBrands(),
                Types = await _catalogSvc.GetTypes(),
                BrandFilterApplied = BrandFilterApplied ?? 0,
                TypesFilterApplied = TypesFilterApplied ?? 0,
                PaginationInfo = new PaginationInfo()
                {
                    ActualPage = page ?? 0,
                    ItemsPerPage = catalog.Data.Count,
                    TotalItems = catalog.Count, 
                    TotalPages = (int)Math.Ceiling(((decimal)catalog.Count / itemsPage))
                }
            };
            
            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";
            
            ViewBag.BasketInoperativeMsg = errorMsg;

            return View(vm);
        }
    }
}