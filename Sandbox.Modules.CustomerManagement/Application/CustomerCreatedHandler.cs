using Sandbox.SharedKernel.Messages;
using Microsoft.Extensions.Logging;

namespace Sandbox.Modules.CustomerManagement.Application;

public class CustomerCreatedHandler
{
    public void Handle(CustomerCreated message, ILogger<CustomerCreatedHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(logger);

        logger.LogInformation("Customer {CustomerId} created: {FirstName} {LastName}", message.Id, message.FirstName, message.LastName);
    }
}
