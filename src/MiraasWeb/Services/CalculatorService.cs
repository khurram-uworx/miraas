namespace MiraasWeb.Services;

using MiraasWeb.Controllers;
using MiraasWeb.Domain;
using MiraasWeb.Models;

public class CalculatorService
{
    readonly CalculationEngine inheritanceEngine = new();

    Heir? createHeir(string relationTypeName, int count) => relationTypeName switch
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
        nameof(RelationType.Husband) => new Husband(count),
        nameof(RelationType.Wife) => new Wife(count),
        nameof(RelationType.FullBrother) => new FullBrother(count),
        nameof(RelationType.FullSister) => new FullSister(count),
        nameof(RelationType.ConsanguineBrother) => new ConsanguineBrother(count),
        nameof(RelationType.ConsanguineSister) => new ConsanguineSister(count),
        nameof(RelationType.UterineBrother) => new UterineBrother(count),
        nameof(RelationType.UterineSister) => new UterineSister(count),
        _ => null
    };

    CalculationResponseDto mapResultToDto(CalculationResult result, decimal? estateValue)
    {
        var response = new CalculationResponseDto
        {
            Success = result.IsSuccessful,
            ErrorMessage = result.ErrorMessage,
            RequiresScholarlyReview = result.RequiresScholarlyReview,
            TotalFraction = result.TotalFraction.ToString(),
            Warnings = result.Warnings
        };

        if (!result.IsSuccessful)
            return response;

        try
        {
            response.TotalPercentage = result.TotalFraction.ToPercentage();
        }
        catch {  /* can throw divide by zero */ }

        foreach (var heir in result.Heirs)
        {
            var shareDto = new HeirShareDto
            {
                Relation = heir.RelationTypeToString(),
                Count = heir.Count,
                Fraction = heir.Result.Fraction.ToString(),
                Percentage = heir.Result.Fraction.ToPercentage(),
                Explanation = heir.Result.Explanation
            };

            if (estateValue.HasValue && estateValue.Value > 0)
                shareDto.Amount = estateValue.Value * (heir.Result.Fraction.ToDecimal());

            response.Shares.Add(shareDto);
        }

        return response;
    }

    public CalculationResponseDto Calculate(CalculationRequestDto request)
    {
        try
        {
            if (request?.Heirs == null || request.Heirs.Count == 0)
                return new CalculationResponseDto
                {
                    Success = false,
                    ErrorMessage = "No heirs specified."
                };

            var gender = (GenderType)request.DeceasedGender;
            var deceased = new DeceasedPerson(gender);

            var inheritanceCase = new InheritanceCase(deceased);
            inheritanceCase.EstateValue = request.EstateValue;

            foreach (var heirEntry in request.Heirs)
            {
                if (heirEntry.Value <= 0)
                    continue;

                var heir = createHeir(heirEntry.Key, heirEntry.Value);
                if (heir != null)
                    inheritanceCase.AddHeir(heir);
            }

            var calculationResult = inheritanceEngine.Calculate(inheritanceCase);
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
