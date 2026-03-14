using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClientApi.Models;

public class Client
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Phone { get; set; }
    public string Email { get; set; } = null!;
    public decimal Discount { get; set; }
    public bool Verified { get; set; }
}

public class ClientRequestDto
{
    [Required(ErrorMessage = "FullName is required"), StringLength(200, ErrorMessage = "FullName must be up to 200 characters long")]
    public string FullName { get; set; } = null!;

    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must contain 10 characters")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Email is required"), EmailAddress(ErrorMessage = "Email must be e-mail address")]
    public string Email { get; set; } = null!;

    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100"), DefaultValue(0.00)]
    public decimal Discount { get; set; } = 0.00M;

    [AllowedValues([true, false], ErrorMessage = "Value must be on of nexts: true, false"), DefaultValue(false)]
    public bool Verified { get; set; } = false;
}

public class ClientPartialUpdateRequestDto
{
    [StringLength(200, ErrorMessage = "FullName must be up to 200 characters long")]
    public string? FullName { get; set; }

    [JsonPropertyName("phone")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must contain 10 characters")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Email must be e-mail address")]
    public string? Email { get; set; }

    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100"), DefaultValue(0.00)]
    public decimal? Discount { get; set; }

    [AllowedValues([true, false, null], ErrorMessage = "Value must be on of nexts: true, false, null")]
    public bool? Verified { get; set; }
}

public class ClientResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Phone { get; set; }
    public string Email { get; set; } = null!;
    public decimal Discount { get; set; }
    public bool Verified { get; set; }
}