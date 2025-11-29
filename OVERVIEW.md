# System Responsibilities

## The System WILL:
- Validate input data
- Prevent impossible family scenarios
- Calculate inheritance shares
- Display result percentages and fractions
- Warn for contradictory states (e.g., more than 4 wives)

## The System WILL NOT:
- Issue legal rulings
- Interpret special or controversial fiqh opinions
- Replace scholars or legal processes
- Handle country-specific laws

---

## Core Features

### MUST HAVE (Phase 1)
- Deceased gender
- Heirs selection (checkboxes / counters)
- Auto calculation
- Display results clearly
- Validation and error feedback

### NICE TO HAVE (Future)
- Case saving
- Printable report (PDF)
- Explanation mode
- Jurisprudence mode switch (Hanafi, Shafi, etc.)
- Arabic/English language switch

---

## Calculation Scope

Initially we follow:
- Sunni inheritance rules (standard fiqh rules)
- One consistent jurisprudence for v1
- Clear rules, no opinion mixing

Later expansion possible.

---

## User Flow

1. User opens calculator
2. Selects deceased’s gender
3. Chooses surviving relatives
4. Enters counts (wives, sons, daughters, etc.)
5. Clicks "Calculate"
6. Results are displayed with explanation
7. Errors shown immediately if input is invalid

---

## Definition of Done

The application is considered complete when:

- Every valid input produces a correct output
- Invalid cases are blocked or warned
- Calculation logic is tested
- UI is simple and readable
- No UI logic contains fiqh logic
- Domain logic is isolated and testable

---

## Constraints

- Must run entirely from the server without frontend dependence
- Frontend is stateless
- No accounts needed in v1
- API-first design for calculation engine

---

## Success Criteria

- Works reliably across devices
- No incorrect shares
- No runtime exceptions
- Easy to verify by scholars
- Easy for developers to maintain

---

## Legal + Ethical Disclaimer

This application provides **educational reference calculations only**.

Users must consult qualified scholars or legal professionals before acting on results.

The developer holds no responsibility for misuse.

---

## Naming (Working Title)

> Islamic Inheritance Calculator  
> or  
> Mirath Calculator

---

## Next File

Proceed to:

`DOMAIN.md` — define Islamic heirs, relationships, and rules.
