using FastTechFoods.Orders.Application.Dtos;
using FastTechFoods.Orders.Application.Interfaces;
using FastTechFoods.Orders.Domain.Entities;
using FastTechFoods.Orders.Domain.Enums;

namespace FastTechFoods.Orders.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRabbitMqProducer _rabbitMqProducer;

        public OrderService(IRabbitMqProducer rabbitMqProducer)
        {
            _rabbitMqProducer = rabbitMqProducer;
        }

        public async Task<Guid> SendOrderQueueAsync(OrderDto orderDto)
        {
            try
            {
                Order order = new Order
                {
                    IdStore = orderDto.IdStore,
                    IdUser = orderDto.IdUser,
                    DeliveryType = orderDto.DeliveryType,
                    Items = orderDto.Items.Select(i => new Item(
                        id: i.Id,
                        menuItemId: i.MenuItemId,
                        name: i.Name,
                        description: i.Description,
                        price: i.Price,
                        amount: i.Amount,
                        category: i.Category,
                        notes: i.Notes)
                    )
                };

                await _rabbitMqProducer.SendMessageToQueue(order);

                return order.Id;
            }

            catch (Exception ex)
            {
                throw new Exception($"Falha na construção da entidade Order. {ex.Message}");
            }            
        }

        public async Task<Guid> SendOrderCancelQueueAsync(ChangeStatusDto pedido)
        {
            try
            {                

                await _rabbitMqProducer.SendMessageCancelToQueue(pedido);

                return pedido.OrderId;
            }

            catch (Exception ex)
            {
                throw new Exception($"Falha na construção da entidade Order. {ex.Message}");
            }
        }
    }
}