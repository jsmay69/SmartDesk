//using Xunit;
//using System.Net;
//using System.Net.Http;
//using System.Text;
//using System.Text.Json;
//using FluentAssertions;
//using SmartDesk.Application.DTOs;
//using System.Collections.Generic;
//using Microsoft.VisualStudio.TestPlatform.TestHost;
//using System.Threading.Tasks;

//namespace SmartDesk.API.Tests;

//public class TodoItemControllerAdditionalTests : IClassFixture<CustomWebApplicationFactory<Program>>
//{
//    private readonly HttpClient _client;

//    public TodoItemControllerAdditionalTests(CustomWebApplicationFactory<Program> factory)
//    {
//        _client = factory.CreateClient();
//    }

//    [Fact]
//    public async Task LifecycleAndParse()
//    {
//        // Create
//        var dto = new TodoItemDto { Title = "Life", Description = "Cycle", Priority = "High" };
//        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
//        var createResp = await _client.PostAsync("/api/todoitems", content);
//        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
//        var created = JsonSerializer.Deserialize<TodoItemDto>(await createResp.Content.ReadAsStringAsync(),
//            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
//        created.Should().NotBeNull();

//        // Update
//        created.Title = "Updated";
//        content = new StringContent(JsonSerializer.Serialize(created), Encoding.UTF8, "application/json");
//        var updateResp = await _client.PutAsync($"/api/todoitems/{created.Id}", content);
//        updateResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

//        // Delete
//        var deleteResp = await _client.DeleteAsync($"/api/todoitems/{created.Id}");
//        deleteResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

//        // Parse
//        var raw = "Test parsing agent";
//        var parseContent = new StringContent(JsonSerializer.Serialize(raw), Encoding.UTF8, "application/json");
//        var parseResp = await _client.PostAsync("/api/todoitems/parse", parseContent);
//        parseResp.StatusCode.Should().Be(HttpStatusCode.OK);
//        var parsed = JsonSerializer.Deserialize<TodoItemDto>(await parseResp.Content.ReadAsStringAsync(),
//            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
//        parsed.Should().NotBeNull().And.Subject.As<TodoItemDto>().Title.Should().Contain("Test");
//    }
//}
