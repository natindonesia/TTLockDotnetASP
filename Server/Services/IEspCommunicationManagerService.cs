using Server.Models;
using Server.Net;
using Server.Net.Packets;

namespace Server.Services;
/**
 * Handle underlying communication with ESP32 devices
 */
public interface IEspCommunicationManagerService
{
    /**
     * Discover ESP32 devices on the network
     */
    Task<IEnumerable<IEspDevice>> DiscoverDevices();

    /**
     * Register event handler for all events
     */
    void RegisterEventHandler(EventHandler<Esp32Response> handler);

    /**
     * Send a command to a device
     */
    Task SendCommand(IEspDevice device, RpcRequest command);

    /**
     * Send a command to a device and wait for a response
     */
    Task<RpcResponse> SendCommandAndWaitForResponse(IEspDevice device, RpcRequest command);

}