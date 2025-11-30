namespace MiraasWeb.Services;

using MiraasWeb.Domain;
using MiraasWeb.Domain.Dtos;

/// <summary>
/// Service that orchestrates Islamic inheritance calculations.
/// Bridges the API layer with the domain logic.
/// </summary>
public class CalculatorService
{
    readonly InheritanceEngine inheritanceEngine;

    public CalculatorService()
    {
        inheritanceEngine = new InheritanceEngine();
    }

    /// <summary>
    /// Creates a Heir instance from a relation type string and count.
    /// </summary>
    Heir? createHeir(string relationTypeName, int count)
    {
        return relationTypeName switch
        {
            nameof(RelationType.Son) => new Son(count),
            nameof(RelationType.Daughter) => new Daughter(count),
            nameof(RelationType.SonOfSon) => new SonOfSon(count),
            nameof(RelationType.DaughterOfSon) => new DaughterOfSon(count),
            nameof(RelationType.Father) => new Father(),
            nameof(RelationType.Mother) => new Mother(),
            nameof(RelationType.Grandfather) => new Grandfather(),
            nameof(RelationType.GrandmotherMaternal) => new GrandmotherMaternal(),
            nameof(RelationType.GrandmotherPaternal) => new GrandmotherPaternal(),
            nameof(RelationType.Husband) => new Husband(),
            nameof(RelationType.Wife) => new Wife(count),
            nameof(RelationType.FullBrother) => new FullBrother(count),
            nameof(RelationType.FullSister) => new FullSister(count),
            nameof(RelationType.ConsanguineBrother) => new ConsanguineBrother(count),
            nameof(RelationType.ConsanguineSister) => new ConsanguineSister(count),
            nameof(RelationType.UterineBrother) => new UterineBrother(count),
            nameof(RelationType.UterineSister) => new UterineSister(count),
            _ => null
        };
    }

    /// <summary>
    /// Maps a domain CalculationResult to a response DTO.
    /// </summary>
    CalculationResponseDto mapResultToDto(CalculationResult result, decimal? estateValue)
    {
        var response = new CalculationResponseDto
        {
            Success = result.IsSuccessful,
            ErrorMessage = result.ErrorMessage,
            RequiresScholarlyReview = result.RequiresScholarlyReview,
            TotalFraction = result.TotalFraction.ToString(),
            TotalPercentage = result.TotalFraction.ToPercentage(),
            Warnings = result.Warnings
        };

        if (!result.IsSuccessful)
        {
            return response;
        }

        // Map heirs to DTOs
        foreach (var heir in result.Heirs)
        {
            var shareDto = new HeirShareDto
            {
                Relation = heir.Relation.ToString(),
                Count = heir.Count,
                Fraction = heir.Result.Fraction.ToString(),
                Percentage = heir.Result.Fraction.ToPercentage(),
                Explanation = heir.Result.Explanation
            };

            // Calculate monetary amount if estate value provided
            if (estateValue.HasValue && estateValue.Value > 0)
            {
                shareDto.Amount = estateValue.Value * (heir.Result.Fraction.ToDecimal());
            }

            response.Shares.Add(shareDto);
        }

        return response;
    }

    /// <summary>
    /// Calculates inheritance shares based on the request DTO.
    /// </summary>
    public CalculationResponseDto Calculate(CalculationRequestDto request)
    {
        try
        {
            // Validate request
            if (request?.Heirs == null || request.Heirs.Count == 0)
            {
                return new CalculationResponseDto
                {
                    Success = false,
                    ErrorMessage = "No heirs specified."
                };
            }

            // Create deceased person
            var gender = (Gender)request.DeceasedGender;
            var deceased = new DeceasedPerson(gender);

            // Create inheritance case
            var inheritanceCase = new InheritanceCase(deceased);
            inheritanceCase.EstateValue = request.EstateValue;

            // Add heirs from request
            foreach (var heirEntry in request.Heirs)
            {
                if (heirEntry.Value <= 0)
                    continue;

                var heir = createHeir(heirEntry.Key, heirEntry.Value);
                if (heir != null)
                {
                    inheritanceCase.AddHeir(heir);
                }
            }

            // Calculate using domain engine
            var calculationResult = inheritanceEngine.Calculate(inheritanceCase);

            // Map to response DTO
            return mapResultToDto(calculationResult, request.EstateValue);
        }
        catch (Exception ex)
        {
            return new CalculationResponseDto
            {
                Success = false,
                ErrorMessage = $"Calculation error: {ex.Message}"
            };
        }
    }

}
