using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RaceDay.Tests;

public sealed class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public ApiTests(WebApplicationFactory<Program> factory) => _factory = factory;
    private HttpClient Client() => _factory.WithWebHostBuilder(_ => { }).CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true, AllowAutoRedirect = false });
    private static async Task Register(HttpClient c, string email, string role) { var r = await c.PostAsJsonAsync("/api/auth/register", new { fullName="Test User", email, password="StrongPass123!", role }); Assert.Equal(HttpStatusCode.Created, r.StatusCode); }
    private static async Task Login(HttpClient c, string email) { var r=await c.PostAsJsonAsync("/api/auth/login",new{email,password="StrongPass123!"}); Assert.Equal(HttpStatusCode.OK,r.StatusCode); }

    [Fact] public async Task RegisterRejectsShortPassword() { using var c=Client(); var r=await c.PostAsJsonAsync("/api/auth/register",new{fullName="A User",email="short@test.local",password="abc",role="Participant"}); Assert.Equal(HttpStatusCode.BadRequest,r.StatusCode); }
    [Fact] public async Task LoginRejectsInvalidCredentials() { using var c=Client(); var r=await c.PostAsJsonAsync("/api/auth/login",new{email="missing@test.local",password="wrongpassword"}); Assert.Equal(HttpStatusCode.Unauthorized,r.StatusCode); }
    [Fact] public async Task ProtectedProfileRejectsUnauthenticatedRequest() { using var c=Client(); var r=await c.GetAsync("/api/profile"); Assert.Equal(HttpStatusCode.Unauthorized,r.StatusCode); }
    [Fact] public async Task ParticipantCannotCreateEvent() { using var c=Client(); var email=$"participant-{Guid.NewGuid():N}@test.local"; await Register(c,email,"Participant"); await Login(c,email); var r=await c.PostAsJsonAsync("/api/events",new{name="Test Run",description="Test",date=DateTime.UtcNow.AddDays(10),location="Cape Town",distanceKm=5,eventType="Run"}); Assert.Equal(HttpStatusCode.Forbidden,r.StatusCode); }
    [Fact] public async Task OrganiserCanCreateEvent() { using var c=Client(); var email=$"organiser-{Guid.NewGuid():N}@test.local"; await Register(c,email,"Organiser"); await Login(c,email); var r=await c.PostAsJsonAsync("/api/events",new{name="Test Run",description="Test",date=DateTime.UtcNow.AddDays(10),location="Cape Town",distanceKm=5,eventType="Run"}); Assert.Equal(HttpStatusCode.Created,r.StatusCode); }
    [Fact] public async Task ParticipantCanEnrolInEventAndEnrolmentIsRecorded() { using var organiser=Client(); var oe=$"org-{Guid.NewGuid():N}@test.local"; await Register(organiser,oe,"Organiser"); await Login(organiser,oe); var er=await organiser.PostAsJsonAsync("/api/events",new{name="Community Run",description="5km",date=DateTime.UtcNow.AddDays(10),location="Cape Town",distanceKm=5,eventType="Run"}); var ev=await er.Content.ReadFromJsonAsync<EventDto>(); Assert.NotNull(ev); var cr=await organiser.PostAsJsonAsync($"/api/events/{ev!.id}/categories",new{name="Open 5km",categoryType="Distance"}); Assert.Equal(HttpStatusCode.Created,cr.StatusCode); var cat=await cr.Content.ReadFromJsonAsync<CategoryDto>(); Assert.NotNull(cat);
        using var participant=Client(); var pe=$"part-{Guid.NewGuid():N}@test.local"; await Register(participant,pe,"Participant"); await Login(participant,pe); var enr=await participant.PostAsJsonAsync("/api/enrolments",new{eventId=ev.id,categoryId=cat!.id}); Assert.Equal(HttpStatusCode.Created,enr.StatusCode); var mine=await participant.GetAsync("/api/enrolments/mine"); Assert.Equal(HttpStatusCode.OK,mine.StatusCode); var body=await mine.Content.ReadAsStringAsync(); Assert.Contains(ev.id.ToString(),body,StringComparison.OrdinalIgnoreCase); }
    private sealed record EventDto(Guid id); private sealed record CategoryDto(Guid id);
}
