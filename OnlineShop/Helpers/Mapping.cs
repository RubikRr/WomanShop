using OnlineShop.DB.Models;
using WomanShop.Areas.Admin.Models;
using WomanShop.Models;

namespace WomanShop.Helpers
{
    public static class Mapping
    {
        public static ProductViewModel ToProductViewModel(Product productModel)
        {
            if (productModel != null)
            {
                return new ProductViewModel {
                    Id = productModel.Id,
                    Name = productModel.Name,
                    Cost = productModel.Cost,
                    Description = productModel.Description,
                    ImageItemsPaths= productModel.ImageItems.Select(imageItem=>imageItem.ImagePath).ToList(),
                    ImagePath = productModel.ImagePath
                };
            }
            return null;

        }
        public static Product ToProductModel(ProductViewModel productViewModel)
        {
            if (productViewModel != null)
            {
                return new Product
                {
                    Id = productViewModel.Id,
                    Name = productViewModel.Name,
                    Cost = productViewModel.Cost,
                    Description = productViewModel.Description,
                    ImagePath = productViewModel.ImagePath
                };
            }
            return null;
        }
        public static List<ProductViewModel> ToProductsViewModel(List<Product> productsModel)
        {
            var ans = productsModel.Select(ToProductViewModel).ToList() ?? null;
            return ans;
        }

        public static CartItemViewModel ToCartItemViewModel(CartItem cartItemModel)
        {
            if (cartItemModel != null)
            {
                return new CartItemViewModel {
                    Product = ToProductViewModel(cartItemModel.Product),
                    Quantity = cartItemModel.Quantity
                };
            }
            return null;

        }
        public static List<CartItemViewModel> ToCartItemsViewModel(List<CartItem> cartItemsModel)
        {
            var ans = cartItemsModel?.Select(ToCartItemViewModel)?.ToList() ?? null;
            return ans;
        }

        public static CartViewModel ToCartViewModel(Cart cartModel)
        {
            if (cartModel != null)
            {
                return new CartViewModel
                {
                    Id = cartModel.Id,
                    Items = ToCartItemsViewModel(cartModel.Items),
                    UserId = cartModel.UserId
                };
            }
            return null;
        }

        public static UserDeliveryInfo ToUserDeliveryInfoModel(UserDeliveryInfoViewModel userInfo)
        {
            return new UserDeliveryInfo
            {
                Name= userInfo.Name,
                Address= userInfo.Address,
                Phone= userInfo.Phone
            };
        }
        public static UserDeliveryInfoViewModel ToUserDeliveryInfoViewModel(UserDeliveryInfo userInfo)
        {
            return new UserDeliveryInfoViewModel
            {
                Name = userInfo.Name,
                Address = userInfo.Address,
                Phone = userInfo.Phone
            };
        }
        public static OrderViewModel ToOrderViewModel(Order order)
        {
            return new OrderViewModel
            {
                Id= order.Id,
                DeliveryInfo=ToUserDeliveryInfoViewModel(order.DeliveryInfo),
                Date= order.Date,
                Items= ToCartItemsViewModel(order.Items),
                Status= (OrderStatusViewModel)(order.Status),
            };
        }

        public static List<OrderViewModel> ToOrdersViewModel(List<Order> orders)
        {
            return orders.Select(ToOrderViewModel).ToList() ??null;
        }

        public static UserViewModel ToUserViewModel(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.UserName,
                Phone = user.PhoneNumber,
                AvatarPath=user.AvatarPath
            };
        }

        public static EditUserByAdminViewModel ToEditUserbyAdminViewModel(User user)
        {
            return new EditUserByAdminViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.UserName,
                Phone = user.PhoneNumber,
            };
        }
        public static UserViewModel ToUserViewModel(User user,List<string> roles)
        {
            var newUser = ToUserViewModel(user);
            newUser.RoleName = string.Join(" ", roles);
            return newUser;
        }

        public static EditUserByAdminViewModel ToEditUserByAdminViewModel(User user, List<string> roles)
        {
            var newUser = ToEditUserbyAdminViewModel(user);
            newUser.RoleName = string.Join(" ", roles);
            return newUser;
        }
        public static List<UserViewModel> ToUsersViewModel(List<User> users)
        {
            return users.Select(ToUserViewModel).ToList() ?? null;
        }

    }
}
