//using Xunit;
//using System.Net;
//using System.Net.Http;
//using System.Text.Json;
//using System.Collections.Generic;
//using FluentAssertions;
//using SmartDesk.Application.DTOs;
//using System.Threading.Tasks;
//using Microsoft.VisualStudio.TestPlatform.TestHost;

//namespace SmartDesk.API.Tests;

//public class TodoItemControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
//{
//    private readonly HttpClient _client;

//    public TodoItemControllerTests(CustomWebApplicationFactory<Program> factory)
//    {
//        _client = factory.CreateClient();
//    }

//    [Fact]
//    public async Task GetAll_ReturnsOkWithEmpty()
//    {
//        var resp = await _client.GetAsync("/api/todoitems");
//        resp.StatusCode.Should().Be(HttpStatusCode.OK);
//        var items = JsonSerializer.Deserialize<List<TodoItemDto>>(await resp.Content.ReadAsStringAsync(),
//            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
//        items.Should().BeEmpty();
//    }
//}
