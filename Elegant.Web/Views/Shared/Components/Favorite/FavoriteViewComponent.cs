﻿using Elegant.DAL;
using Elegant.DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Elegant.Web.Views.Shared.Components.Favorite
{
    public class FavoriteViewComponent : ViewComponent
    {
        private IFavoritesStorage favoritesStorage;

        public FavoriteViewComponent(IFavoritesStorage _favoritesStorage)
        {
            favoritesStorage = _favoritesStorage;
        }

        public IViewComponentResult Invoke()
        {
            var userFavoriteProductsCount = favoritesStorage.GetAllProducts(Constants.UserId).Count();
            if (userFavoriteProductsCount != null && userFavoriteProductsCount != 0)
            {
                return View("Favorite", userFavoriteProductsCount.ToString());
            }
            return View("Favorite", "");
        }
    }
}
