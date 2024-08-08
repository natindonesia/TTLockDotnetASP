using Server.Models;
using Server.Net;
using Server.Net.Packets;

namespace Server.Services;

/**
 * Handle underlying communication with ESP32 devices
 */
public interface IEspCommunicationManagerService : IHostedService
{
    /**
     * Register event handler for all events
     */
    void RegisterEventHandler(EventHandler<Esp32Response> handler);

    /**
     * Send a command to a device, if null, broadcast to all devices
     */
    Task SendCommand(IEspDevice? device, RpcRequest command);

    /**
     * Send a command to a device and wait for a response, if device is null, broadcast to all devices
     */
    Task<RpcResponse> SendCommandAndWaitForResponse(IEspDevice? device, RpcRequest command);


    /**
     * Check if the service is connected to the ESP32 devices
     */
    bool IsConnected();
}