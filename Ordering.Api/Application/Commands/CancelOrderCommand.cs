using System.Runtime.Serialization;
using MediatR;

namespace Ordering.Api.Application.Commands
{
    public class CancelOrderCommand : IRequest<bool>
    {

        [DataMember]
        public int OrderNumber { get; private set; }
        public CancelOrderCommand()
        {

        }
        public CancelOrderCommand(int orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }
}
