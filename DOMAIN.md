# Domain Model

This document defines the **domain entities, relationships, and core concepts** used by the inheritance calculation engine.  
All Islamic logic must live in the Domain layer and be independent from MVC, React, or database code.

---

## Core Domain Philosophy

- The Domain layer contains **pure logic**
- No UI assumptions
- No database dependencies
- Fully testable
- Predictable and deterministic
- No framework coupling

---

## Primary Entity

### DeceasedPerson

Represents the individual whose estate is being divided.

```csharp
class DeceasedPerson
{
    public Gender Gender { get; set; }
}
```

#### Gender Enum

```csharp
enum Gender
{
    Male,
    Female
}
```

---

## Inheritance Case

### InheritanceCase

Central aggregate root containing all heirs and deceased information.

```csharp
class InheritanceCase
{
    public DeceasedPerson Deceased { get; set; }
    public List<Heir> Heirs { get; set; }
}
```

---

## Heir Base Class

Every inheritor inherits from this base class:

```csharp
abstract class Heir
{
    public RelationType Relation { get; }
    public int Count { get; set; } = 1;
    public ShareResult Result { get; set; }
}
```

---

## Supported Heirs (Phase 1)

### Direct Descendants

* Son
* Daughter
* Son of Son
* Daughter of Son

### Ascendants

* Father
* Mother
* Grandfather (paternal)
* Grandmother (maternal / paternal separately)

### Spouse

* Husband
* Wife

### Siblings

* Full Brother
* Full Sister
* Consanguine Brother
* Consanguine Sister
* Uterine Brother
* Uterine Sister

---

## RelationType Enum

```csharp
enum RelationType
{
    Son,
    Daughter,

    SonOfSon,
    DaughterOfSon,

    Father,
    Mother,
    Grandfather,
    GrandmotherMaternal,
    GrandmotherPaternal,

    Husband,
    Wife,

    FullBrother,
    FullSister,

    ConsanguineBrother,
    ConsanguineSister,

    UterineBrother,
    UterineSister
}
```

---

## Share Result (Output Model)

```csharp
class ShareResult
{
    public decimal Fraction { get; set; }
    public decimal Percentage { get; set; }
    public decimal Amount { get; set; }
    public string Explanation { get; set; }
}
```

---

## Fraction Representation

Prefer rational numbers internally:

```csharp
struct Fraction
{
    int Numerator;
    int Denominator;
}
```

All operations must normalize fractions.

---

## Heir Classification

### Fixed Share Heirs (Ashab al-Furudh)

Receive prescribed fractions unless blocked.

Examples:

* Wife: 1/4 or 1/8
* Husband: 1/2 or 1/4
* Mother: 1/6 or 1/3
* Daughter: 1/2 or 2/3

---

### Residuary Heirs (Asabah)

Receive remainder after fixed shares.

Examples:

* Son
* Father (sometimes)
* Full Brother (sometimes)

---

### Blocked Heirs (Mahjoob)

Blocked completely by stronger heirs.

Examples:

* Grandfather blocked by Father
* Siblings blocked by father/sons

---

## Domain Classification Enums

```csharp
enum HeirCategory
{
    FixedShare,
    Residuary,
    Both,
    Blocked
}
```

---

## Blocking Rules (Hijab)

Blocking must be modeled as rules:

### Example Rules:

* Father blocks grandfather
* Son blocks brother
* Son + daughter blocks grandson
* Mother blocks maternal grandmother
* Father blocks paternal grandfather

Create a Rule Engine:

```csharp
class BlockingRule
{
    RelationType Blocker;
    RelationType Blocked;
}
```

---

## Share Rule Representation

```csharp
class ShareRule
{
    public RelationType Heir;
    public Func<InheritanceCase, Fraction> Rule;
}
```

---

## Validity Rules

Domain must validate:

### Invalid Cases:

* More than 4 wives
* More than 1 husband
* Negative counts
* Father & grandfather simultaneously
* Grandparent without parent presence when invalid per rules

### Validation Model:

```csharp
class ValidationResult
{
    public bool IsValid;
    public List<string> Errors;
}
```

---

## Domain Services

### InheritanceEngine

Single entry point:

```csharp
class InheritanceEngine
{
    public CalculationResult Calculate(InheritanceCase input);
}
```

### CalculationResult

```csharp
class CalculationResult
{
    public List<Heir> FinalHeirs;
    public decimal TotalFraction;
    public bool RequiresScholarlyReview;
}
```

---

## Calculation Pipeline

1. Validate input
2. Apply blocking rules
3. Identify fixed share heirs
4. Allocate fixed shares
5. Calculate remainder
6. Distribute remainder to residuaries
7. Normalize fractions
8. Generate explanations
9. Produce output

---

## Domain Invariants

* Sum of shares MUST equal 1 (unless Awl or Radd)
* All fractions must be simplified
* No negative allocations
* No heir receives more than allowed

---

## Special Scenarios (Future Scope)

To support later:

* Awl (increase of denominator)
* Radd (return of remainder)
* Multiple jurisprudence engines
* Rare combinations

---

## Testing Strategy

Each heir relation:

* Unit test
* Blocking test
* Combination test

Example:

* Wife + Son → 1/8
* Daughter only → 1/2
* Two daughters → 2/3

---

## Next File

Proceed to:

`RULES.md` — define actual share rules for each heir.

```
