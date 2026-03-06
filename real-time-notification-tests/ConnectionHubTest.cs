using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using real_time_notification.Api.Hubs;
using real_time_notification.Application.Interfaces;
using Xunit;

namespace TestProject1;

public class ConnectionHubTest
{
    private readonly Mock<IHubCallerClients> _mockClients = new();
    private readonly Mock<IClientProxy> _mockClientProxy = new();
    private readonly Mock<IGroupManager> _mockGroupManager = new();
    private readonly Mock<IPresenceService> _mockPresenceService = new();
    private readonly Mock<ILogger<ConnectionHub>> _mockLogger = new();
    private readonly ConnectionHub _hub;
    
    public ConnectionHubTest()
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        _hub = new ConnectionHub(_mockLogger.Object, _mockPresenceService.Object)
        {
            Context = Mock.Of<HubCallerContext>(c =>
                c.User == user &&
                c.ConnectionId == "conn-abc-123"
            ),
            Groups = _mockGroupManager.Object,
            Clients = _mockClients.Object
        };
    }

    [Fact]
    public async Task GetRoom_ShouldAlwaysReturnSameRoom_RegardlessOfOrder()
    {
        int targetUserId = 5;
        string expected = "chat_1_5";
        
        _mockClients.Setup(c => c.Group(expected)).Returns(_mockClientProxy.Object);
        await _hub.SendMessage(targetUserId, "Olá");
        _mockClients.Verify(c => c.Group(expected), Times.Once);
    }

    [Fact]
    public async Task JoinPrivateChat_ShouldAddUserToCorrectRoom()
    {
        int targetUserId = 2;
        string expectedRoomName = "chat_1_2";

        await _hub.JoinPrivateChat(targetUserId);
        
        _mockGroupManager.Verify(
            g=> g.AddToGroupAsync(
                "conn-abc-123",
                expectedRoomName,
                default
                ),
            Times.Once,
            "User should be added to the correct room 'chat_1_2'"
            );
    }
}