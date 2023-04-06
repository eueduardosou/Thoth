using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Thoth.Core.Models;
using Thoth.Tests.Base;
using Thoth.Tests.Helpers;

namespace Thoth.Tests;

public class ThothJwtAuthorizationFilterTests: IntegrationTestBase<Program>
{
    private readonly string _token;

    public ThothJwtAuthorizationFilterTests() : base(arguments: new Dictionary<string, string>
    {
        {"auth", "UseThothJwtAuthorization"}
    })
    {
        _token = JwtGenerator.GenerateToken(new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new (ClaimTypes.Email, "thotest@thotest.thoth")
        });
    }

    [Theory]
    [MemberData(nameof(CreateValidDataGenerator))]
    public async Task Create_ShouldBeAuthorized(FeatureFlag featureFlag)
    {
        //Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var postContent = new StringContent(
            JsonConvert.SerializeObject(featureFlag), Encoding.UTF8, "application/json");

        //Act
        var response = await HttpClient.PostAsync($"/thoth-api/FeatureFlag?accessToken={_token}", postContent);

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task Dashboard_ShouldBeAuthorized()
    {
        //Act
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var response = await HttpClient.GetAsync($"/thoth/index.html?accessToken={_token}");

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidTokenDataGenerator))]
    public async Task Dashboard_ShouldBeForbidden(string token)
    {
        //Act
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await HttpClient.GetAsync($"/thoth?accessToken={token}");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    public static IEnumerable<object[]> CreateValidDataGenerator()
    {
        yield return new object[]
        {
            new FeatureFlag
            {
                Name = Guid.NewGuid().ToString(),
                Type = FeatureFlagsTypes.Boolean,
                Value = true
            }
        };
        yield return new object[]
        {
            new FeatureFlag
            {
                Name = Guid.NewGuid().ToString(),
                Type = FeatureFlagsTypes.PercentageFilter,
                FilterValue = "50",
                Value = true
            }
        };
    }

    public static IEnumerable<object[]> InvalidTokenDataGenerator()
    {
        yield return new object[] { JwtGenerator.GenerateToken(new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new (ClaimTypes.Email, "thotest@thotest.thoth")
        }, 1, "hhh", "tttt", new SigningCredentials
        (
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())),
            SecurityAlgorithms.HmacSha256Signature
        ))};

        yield return new object[] {""};
    }
}